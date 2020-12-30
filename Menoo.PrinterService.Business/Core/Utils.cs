using Google.Cloud.Firestore;
using Menoo.PrinterService.Business.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Business.Core
{
    /// <summary>
    /// Contiene métodos de uso común.
    /// </summary>
    public static class Utils
    {
        public static string BeforeAt(string date, int minutes)
        {
            bool isValidDate = DateTime.TryParseExact(date, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result);

            //Cuando es mesa cerrada, llega con otro formato.
            if (!isValidDate)
            {
                DateTime.TryParseExact(date, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
            }
            return result.AddMinutes(minutes).ToString("yyyy/MM/dd HH:mm");
        }

        /// <summary>
        /// Convierte un JSON en un objeto tipado.
        /// </summary>
        /// <typeparam name="T">Clase a ser transformada.</typeparam>
        /// <param name="element">json como diccionario (Es la forma como se obtiene el objeto desde firebase)</param>
        /// <returns>Objeto.</returns>
        public static T GetObject<T>(this object element) 
        {
            var json = JsonConvert.SerializeObject(element, Newtonsoft.Json.Formatting.Indented);
            var result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }

        public static string GetTime(string dateTime)
        {
            var splitDateTime = dateTime.Split(' ');
            return splitDateTime[1];
        }

        /// <summary>
        /// Lista la información de los restaurantes.
        /// </summary>
        /// <param name="db">Instancia de la base de datos de firebase.</param>
        /// <returns>Listado de restaurantes.</returns>
        public static async Task<List<Store>> GetStores(FirestoreDb db)
        {
            var stores = new List<Store>();
            var snapshot = await db.Collection("stores").GetSnapshotAsync();
            foreach (var item in snapshot.Documents)
            {
                var storeData = item.ToDictionary();
                var storeObject = storeData.GetObject<Store>();
                storeObject.StoreId = item.Id;
                stores.Add(storeObject);
            }
            return stores;
        }

        /// <summary>
        /// Permite obtener la información completa de un restaurante.
        /// </summary>
        /// <param name="db">Instancia de la base de datos de firebase.</param>
        /// <param name="storeId">Identificador del restaurante.</param>
        /// <returns>Datos del restaurante buscado.</returns>
        public static async Task<Store> GetStores(FirestoreDb db, string storeId)
        {
            var stores = await GetStores(db);
            return stores.SingleOrDefault(s => s != null && !string.IsNullOrEmpty(s.StoreId) && s.StoreId == storeId);
        }

        /// <summary>
        /// Crea un archivo de log simple.
        /// </summary>
        /// <param name="error">Entrada a hacer traza.</param>
        public static void LogError(string error)
        {
            FileStream fs = null;
            StreamWriter sw = null;
            try
            {
                fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "Log.txt", FileMode.Append);
                sw = new StreamWriter(fs);

                sw.WriteLine();
                sw.WriteLine("***************************************************************");
                sw.WriteLine($"Fecha {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}");
                sw.WriteLine(error);
                sw.WriteLine("***************************************************************");
            }
            catch
            {
                // ignored
            }
            finally
            {
                sw?.Close();
                fs?.Close();
            }
        }

        /// <summary>
        /// Guarda el ticket en la colección 'print'.
        /// </summary>
        /// <param name="db">Instancia de la base de firebase.</param>
        /// <param name="ticket">Elemento o ticket a ser almacenado.</param>
        public static async Task SaveTicketAsync<T>(FirestoreDb db, T ticket) where T : class 
        {
            await db.Collection("print").AddAsync(ticket);
        }

        /// <summary>
        /// Método para colocar una orden o pedido como impreso.
        /// </summary>
        /// <param name="db">Instancia de firebase.</param>
        /// <param name="collection">Nombre de la colección.</param>
        /// <param name="doc">Documento a ser actualizado.</param>
        public static async Task<WriteResult> SetOrderPrintedAsync(FirestoreDb db, string collection, string doc)
        {
            var result = await db.Collection(collection).Document(doc).UpdateAsync("printed", true);
            return result;
        }

        /// <summary>Permite obtener un documento por su identificador.</summary>
        /// <param name="db">Objeto de base de datos de firebase.</param>
        /// <param name="collection">Colección a ser empleada.</param>
        /// <param name="documentId">Identificador del documento.</param>
        /// <returns>Documento</returns>
        public static async Task<DocumentSnapshot> GetDocument(FirestoreDb db, string collection, string documentId) 
        {
            var result = await db.Collection(collection).Document(documentId).GetSnapshotAsync();
            return result;
        }

        /// <summary>
        /// Obtiene la plantilla de ticket según su nombre.
        /// </summary>
        /// <param name="fileTemplate">Nombre de la plantilla</param>
        /// <returns>Cadena de texto con el html del ticket.</returns>
        public static string GetTicketTemplate(string fileTemplate) 
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", fileTemplate + ".html");
            string template = File.ReadAllText(path);
            return template;
        } 
    }
}
