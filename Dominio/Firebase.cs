using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using Google.Cloud.Firestore;
using Google.Apis.Auth.OAuth2;
using Grpc.Auth;
using Grpc.Core;
using Google.Cloud.Firestore.V1;
using System.Configuration;
using System.Drawing.Printing;

namespace Dominio
{
    public class Firebase
    {
        private static Firebase instancia;

        public static Firebase Instancia
        {
            get
            {
                if (instancia == null)
                    instancia = new Firebase();
                return instancia;
            }
        }

        private Firebase() { }

        private static Font printFont;
        private static Orders orden;
        private static TableOpeningFamily tableop;
        private static Booking book;
        private static User usuario;
        private static FirestoreChangeListener listener;
        private static FirestoreChangeListener tableOpeningListener;
        private static FirestoreChangeListener bookingListener;
        private static FirestoreChangeListener userListener;
        public FirestoreChangeListener Listener
        {
            get { return listener; }
        }


        private static FirestoreDb AccessDatabase()
        {
            var credential = GoogleCredential.FromJson("{'type':'service_account'," +
                                                       "'project_id':'comemosya'," +
                                                       "'private_key_id':'98e9f92511db6794ae6be15a7a49ce3907eaa8e6'," +
                                                       "'private_key':'-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCmEyeBmUrnocfB\ng5+J+cTV1BBqrcRI35d/zYFVVjNQRQN2L6pTbx/8qFuQyvvesBs9XByL/XMnZ/8s\nI2iSO0tUd3by3CqrZZoPtVvAewDnVHQt59OWFzZxbwa2JvGNzSF5/NKYhsMNyhJp\nsjvJ2iLFazL0JCpOaJlNnI0rWpCYo7gHuQCvGQGnEzcFj5TZBVJFdhjn+/k0EpUD\nWBeV/axRXds42XF8k5+o01y+bkvkNEdgrgFGZQco2oALQLi7V1d0cGPfwqwZyGfm\nXJ1HfxqtXxeusRzi8MegBUPn6khXpgGxNu1UbyTPOI8NpBG/AlnqiUrumOYM1ECx\nlaqGvp0JAgMBAAECggEAEfpsmk1klBj5wY+MybBb4DyH5u6G+08WLNgLxlfRsAC6\nmFSeYxAadbzpha4NsFye5Eh4kCiIquVJlMcEAuGPnPYeTrnqFhomgIkU5MxEC6bj\nOVV5fKvoYUUARF+Ion3IgBbQGwIqsy9TCTVp3scCIN5DGrYgDNMqtmHKT/1KvjU8\n/RhXN+sf544fvVOGwiEbZNwhtYyT5bSAJTKtczB/hQH6LTpMWxclwgziD7x5Nt/M\n8NAsK2Qg0rDjlGEdMo376/fraTOhclze5CYnHSXf3xOgIfHlmpaEb7y3CkgjYUbM\nexYEBc2CvTMbsCibTJPokgP7PBYtOa9fIxTRCasRYQKBgQDqPc6+WXDlbH/cClYW\nhjIZCyP78u4XKtVbO9kG0AubbJW/0StX5gIKPylun0go2qjfjI83Dc1IjMMy/NzH\n4qgLoTpEkhMnhCd+zI6LHyciwwtQC/xXZWa2kRYQqyR4gBZXgvJV5uDb43pq2i78\nL5aDfPgoqlCjwNBDQhngECm5qQKBgQC1gF1dGpRyUWNyvoW0Hl+TGGkDxNKU1qPZ\nN34tFxTf6hcw9kwP6akDDODsEKtNBYea/bmjmIGCYjSHLDvN0jb3kkjbyoP5HexQ\n9A9T55nXyytMyUYkRzcsLfciD8rhW/WOQ+HV7UgeYTIvnKKVLU1i/Xd4JwM5ZM5a\ngYXF0WekYQKBgQCBhXeiDTa9xUbV1ulPPxjIfD6DfApmyQp8jhUtDTC92kbbb791\ntPr/y3kPcAeof2/NXJ18JaeTLDJrKSKzbALbm2TqsZLh0NM968IN70XmlM7WjioT\n8T/gR01aHifmcXzpGsEA+s7vB1OTbd15GJ8zSZC2e6ZnRaBi8FP6bzWDMQKBgGZg\n1MkaoBdnr0ffDf4Oj+yh/UJh+EJ6XAu/kI2QknbHTXORyk/DhlExJ4Ig2O9mKhqT\n+e28rXjFOknw+n7bj6PQQQaxUgXoCg+Tyz2RyyZ89Jyof8cg4I8sElWFQPQjcfxg\nb/fCk0aHns5adR7eYeNvg78jil8KbJeCrdlqiCKhAoGAZdRWXO3wKfI+pqRLKHp+\nyi+8UUd2NGI9S3XzJaFuw8+OknySfSrr6J2r99P0O0uqXgkc9O7/adqxtgWuvt/y\nuBy5Nl+yXcwkBbLmVTYqBOIisGdYahADngM7rzclJgegn5dj3EOf1+QrbJYvJUTK\nNc4sHmBI2dqDR8oQXGIl+xE=\n-----END PRIVATE KEY-----\n'," +
                                                       "'client_email':'comemosya@appspot.gserviceaccount.com'," +
                                                       "'client_id': ''," +
                                                       "'auth_uri':'https://accounts.google.com/o/oauth2/auth'," +
                                                       "'token_uri':'https://oauth2.googleapis.com/token'," +
                                                       "'auth_provider_x509_cert_url': 'https://www.googleapis.com/oauth2/v1/certs'," +
                                                       "'client_x509_cert_url':'https://www.googleapis.com/robot/v1/metadata/x509/comemosya%40appspot.gserviceaccount.com'}");
            var channelCredentials = credential.ToChannelCredentials();
            var channel = new Channel(FirestoreClient.DefaultEndpoint.ToString(), channelCredentials);
            var client = FirestoreClient.Create(channel);
            return FirestoreDb.Create("comemosya", client);
        }

        public async Task RefreshListener(string storeId)
        {
            await listener.StopAsync();
            await this.RefreshCallback(storeId);
        }

        private async Task RefreshCallback(string storeId)
        {
            var db = AccessDatabase();
            listener = db.Collection("orderFamily").WhereEqualTo("storeId", storeId).OrderByDescending("createdAt").Limit(1).Listen(async snapshot =>
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        if (snapshot.Count > 0)
                        {
                            foreach (var orders in snapshot)
                            {
                                var ordenes = orders.ConvertTo<Orders>();
                                orden = ordenes;
                            }
                            if(orden.Printed == false)
                            {
                                printFont = new Font("Arial", 13);
                                PrintDocument pd = new PrintDocument();
                                pd.PrinterSettings.PrinterName = ConfigurationManager.AppSettings["PrinterName"];
                                pd.PrintPage += new PrintPageEventHandler
                                   (pd_PrintPage);
                                pd.Print();
                                var docRef = db.Collection("orderFamily").WhereEqualTo("tableOpeningId", orden.TableOpeningId);
                                var querySnapshot = await docRef.GetSnapshotAsync();
                                if (querySnapshot.Count > 0)
                                {
                                    var docref2 = querySnapshot.Documents[0].Reference;
                                    await docref2.UpdateAsync("printed", true);
                                    var alalaa = await docref2.UpdateAsync("printed", true);
                                    Console.WriteLine(alalaa);
                                }
                            }
                        }
                        else
                        {
                            string error = $"{orden.OrderType} no se imprimió Fecha {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}";
                            this.LogError(error);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.LogError(ex.Message);
                    }
                });
            });
            tableOpeningListener = db.Collection("tableOpeningFamily").WhereEqualTo("storeId", storeId).OrderByDescending("createdAt").Limit(1).Listen(async snapshot =>
           {
               await Task.Run(async () =>
               {
                   try
                   {
                       if (snapshot.Count > 0)
                       {
                           foreach (var tableops in snapshot)
                           {
                               var tableopenings = tableops.ConvertTo<TableOpeningFamily>();
                               tableop = tableopenings;
                           }
                           if(tableop.Printed == false)
                           {
                               printFont = new Font("Arial", 13);
                               PrintDocument pd = new PrintDocument();
                               pd.PrinterSettings.PrinterName = ConfigurationManager.AppSettings["PrinterName"];
                               pd.PrintPage += new PrintPageEventHandler
                                  (pd_PrintPageTableOpenings);
                               pd.Print();
                               var docRef = db.Collection("tableOpeningFamily").WhereEqualTo("createdAt", tableop.CreatedAt);
                               var querySnapshot = await docRef.GetSnapshotAsync();
                               if (querySnapshot.Count > 0)
                               {
                                   var docref2 = querySnapshot.Documents[0].Reference;
                                   tableop.Printed = true;
                                   await docref2.SetAsync(tableop);
                               }
                           }
                       }
                       else
                       {
                           string error = $"{tableop.TableNumberId} no se imprimió Fecha {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}";
                           this.LogError(error);
                       }
                   }
                   catch (Exception ex)
                   {
                       this.LogError(ex.Message);
                   }
               });
           });
            bookingListener = db.Collection("bookings").WhereEqualTo("store.id", storeId).OrderByDescending("updatedAt").Limit(1).Listen(async snapshot =>
            {
                await Task.Run(() =>
                {
                    try
                    {
                        if (snapshot.Count > 0)
                        {
                            foreach (var booking in snapshot)
                            {
                                var books = booking.ConvertTo<Booking>();
                                book = books;
                            }

                          var snapshotusu = db.Collection("customers").Document(book.UserId).GetSnapshotAsync().Result;
                            if (snapshotusu.Exists)
                            {
                                var usus = snapshotusu.ConvertTo<User>();
                                usuario = usus;
                            }
                            if(book.Printed == false)
                            {
                                printFont = new Font("Arial", 13);
                                PrintDocument pd = new PrintDocument();
                                pd.PrinterSettings.PrinterName = ConfigurationManager.AppSettings["PrinterName"];
                                pd.PrintPage += new PrintPageEventHandler
                                   (pd_PrintPageBookings);
                                pd.Print();
                            }
                        }
                        else
                        {
                            string error = $"{book.BookingNumber} no se imprimió Fecha {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}";
                            this.LogError(error);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.LogError(ex.Message);
                    }
                });
            });
            await Task.CompletedTask;
        }

        public async Task RunListenOrders()
        {
            var storeId = ConfigurationManager.AppSettings["StoreID"];
            await this.RefreshCallback(storeId);
        }

        private void LogError(string error)
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
                if (orden != null && orden.Items != null)
                    foreach (var item in orden.Items)
                        sw.WriteLine($"{item.Name} X {item.Quantity} $ {(item.Price * item.Quantity)}");
                sw.WriteLine(error);
                sw.WriteLine("***************************************************************");
            }
            catch
            {
            }
            finally
            {
                if (sw != null)
                    sw.Close();
                if (fs != null)
                    fs.Close();
            }
        }

        private static void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {
            var text = string.Empty;

            foreach (var item in orden.Items)
                text += $"{item.Name} x{item.Quantity} $ {(item.Price * item.Quantity)}\n";


            if (orden != null && orden.OrderType != null)
            {
                if (orden.OrderType.ToUpper().Trim() == "MESAS")
                {
                    float yPos;
                    float xPos = 35;
                    int count = 0;
                    float leftMargin = ev.MarginBounds.Left;
                    float topMargin = ev.MarginBounds.Top;
                    string line = $"Orden de Mesa\n\nUsuario: {orden.UserName}\n\n{text}\n\nNumero de Mesa: {orden.Address}\n\nTotal: {orden.Total};";

                    yPos = topMargin + (count *
                       printFont.GetHeight(ev.Graphics));
                    ev.Graphics.DrawString(line, printFont, Brushes.Black,
                       xPos, yPos, new StringFormat());
                }
                else if (orden.OrderType.ToUpper().Trim() == "RESERVA")
                {
                    float linesPerPage = 0;
                    float yPos = 0;
                    float xPos = 35;
                    int count = 0;
                    float leftMargin = ev.MarginBounds.Left;
                    float topMargin = ev.MarginBounds.Top;
                    string line = $"Orden de Reserva\n\nUsuario: {orden.UserName}\n\n{text}\n\nNumero de Mesa: {orden.Address}\n\nTotal: {orden.Total};";

                    linesPerPage = ev.MarginBounds.Height /
                       printFont.GetHeight(ev.Graphics);

                    yPos = topMargin + (count *
                       printFont.GetHeight(ev.Graphics));
                    ev.Graphics.DrawString(line, printFont, Brushes.Black,
                       xPos, yPos, new StringFormat());
                }
                else if (orden.OrderType.ToUpper().Trim() == "TAKEAWAY")
                {
                    float linesPerPage = 0;
                    float yPos = 0;
                    float xPos = 35;
                    int count = 0;
                    float leftMargin = ev.MarginBounds.Left;
                    float topMargin = ev.MarginBounds.Top;
                    string line = $"Orden de TakeAway\n\nUsuario: {orden.UserName}\n\n{text}\n\nNumero de Mesa: {orden.Address}\n\nTotal: {orden.Total};";

                    linesPerPage = ev.MarginBounds.Height /
                       printFont.GetHeight(ev.Graphics);

                    yPos = topMargin + (count *
                       printFont.GetHeight(ev.Graphics));
                    ev.Graphics.DrawString(line, printFont, Brushes.Black,
                       xPos, yPos, new StringFormat());
                }
            }
        }
        private static void pd_PrintPageTableOpenings(object sender, PrintPageEventArgs ev)
        {
            if (tableop.Closed)
            {
                float yPos;
                float xPos = 20;
                int count = 0;
                float leftMargin = ev.MarginBounds.Left;
                float topMargin = ev.MarginBounds.Top;
                string line = "Mesa Cerrada" + "\n" + "\n" + "Número de Mesa: " + tableop.TableNumberId + "\n" + "\n" + "Cerrada el: " + tableop.ClosedAt;

                yPos = topMargin + (count *
                   printFont.GetHeight(ev.Graphics));

                ev.Graphics.DrawString(line, printFont, Brushes.Black,
                   xPos, yPos, new StringFormat());
            }
            else
            {
                float yPos;
                float xPos = 20;
                int count = 0;
                float leftMargin = ev.MarginBounds.Left;
                float topMargin = ev.MarginBounds.Top;
                string line = "Nueva Mesa Abierta" + "\n" + "\n" + "Número de Mesa: " + tableop.TableNumberId + "\n" + "\n" + "Fecha de Apertura: " + tableop.OpenedAt + "\n" + "\n" + "Numero de Personas: " + tableop.GuestQuantity;

                yPos = topMargin + (count *
                   printFont.GetHeight(ev.Graphics));

                ev.Graphics.DrawString(line, printFont, Brushes.Black,
                   xPos, yPos, new StringFormat());
            }
        }
        private static void pd_PrintPageBookings(object sender, PrintPageEventArgs ev)
        {
            float yPos;
            float xPos = 20;
            int count = 0;
            float leftMargin = ev.MarginBounds.Left;
            float topMargin = ev.MarginBounds.Top;
            string line = "Reserva Creada" + "\n" + "\n" + "Cantidad de Personas: " + book.GuestQuantity + "\n" + "\n" + "Fecha de Reserva: " + book.BookingDate + "\n" + "\n" + "Nombre de Usuario: " + usuario.Name;

            yPos = topMargin + (count *
               printFont.GetHeight(ev.Graphics));

            ev.Graphics.DrawString(line, printFont, Brushes.Black,
               xPos, yPos, new StringFormat());
        }
    }
}
