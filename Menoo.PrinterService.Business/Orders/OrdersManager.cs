using Google.Cloud.Firestore;
using Menoo.PrinterService.Business.Entities;
using System;
using System.Linq;

namespace Menoo.PrinterService.Business.Orders
{
    /// <summary>
    /// Maneja los eventos de la colección orders
    /// </summary>
    public class OrdersManager
    {
        private readonly FirestoreDb _db;

        public OrdersManager(FirestoreDb db)
        {
            _db = db;
        }

        /// <summary>
        /// Escucha los eventos disparados en la colección orders y orderFamily.
        /// </summary>
        public void Listen()
        {
            _db.Collection("orderFamily")
               .OrderByDescending("incremental")
               .Limit(1)
               .Listen(OnOrderFamily);

            _db.Collection("orders")
                .WhereEqualTo("status", "cancelado")
                .Listen(OnCancelled);
        }

        #region events

        /// <summary>
        /// Orden creada en la colección 'orderFamily'
        /// </summary>
        private void OnOrderFamily(QuerySnapshot snapshot) 
        {
            try
            {
                var document = snapshot.Documents.Single();
                var order = document.ConvertTo<Entities.Orders>();
                order.Store = Utils.GetStores(_db, order.StoreId).GetAwaiter().GetResult();
                var dic = snapshot.Documents.Single().ToDictionary();
                order.Id = document.Id;
                if (order.Printed) 
                {
                    return;
                }
                Utils.SetOrderPrintedAsync(_db, "orderFamily", document.Id).GetAwaiter().GetResult();
                Utils.SaveTicketAsync(_db, order).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Utils.LogError(ex.Message);
            }
        }

        /// <summary>
        /// Orden cancelada en la colección 'orders'
        /// </summary>
        /// <param name="snapshot"></param>
        private void OnCancelled(QuerySnapshot snapshot) 
        {
            try
            {
                if (snapshot.Documents == null || snapshot.Documents.Count == 0)
                {
                    return;
                }
                var document = snapshot.Documents.OrderByDescending(o => o.CreateTime).FirstOrDefault();
                if (document.IsPrinted())
                {
                    return;
                }
                var order = document.GetOrderData();
                Utils.SetOrderPrintedAsync(_db, "orders", document.Id).GetAwaiter().GetResult();
                Utils.SaveTicketAsync(_db, order).GetAwaiter().GetResult();
            }
            catch (Exception ex) 
            {
                Utils.LogError(ex.Message);
            }
        }
        #endregion
    }
}
