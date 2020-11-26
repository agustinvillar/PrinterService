using Google.Cloud.Firestore;
using Menoo.PrinterService.Business.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Business
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
        /// Crea el objeto de impresión o ticket.
        /// </summary>
        /// <returns>Objeto de impresión.</returns>
        public static Ticket CreateInstanceOfTicket()
        {
            return new Ticket()
            {
                PrintedAt = null,
                Expired = false,
                Data = "<!DOCTYPE html><html><body><div class='logoImg'> <img src='\\assets\\img\\Menoo_Logo-Final_color-3.png'> </div>",
                Printed = false,
            };
        }

        /// <summary>
        /// Convierte un JSON en un objeto tipado.
        /// </summary>
        /// <typeparam name="T">Clase a ser transformada.</typeparam>
        /// <param name="dict">json como diccionario (Es la forma como se obtiene el objeto desde firebase)</param>
        /// <returns>Objeto.</returns>
        public static T GetObject<T>(this Dictionary<string, object> dict)
        {
            var json = JsonConvert.SerializeObject(dict, Newtonsoft.Json.Formatting.Indented);
            var result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }

        /// <summary>
        /// Lista la información de los restaurantes.
        /// </summary>
        /// <param name="db">Instancia de la base de datos de firebase.</param>
        /// <returns>Listado de restaurantes.</returns>
        public static async Task<List<Store>> GetStores(FirestoreDb db)
        {
            var snapshot = await db.Collection("stores").GetSnapshotAsync();
            return snapshot.Documents.Select(d => d.ConvertTo<Store>()).ToList();
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
    }
}
