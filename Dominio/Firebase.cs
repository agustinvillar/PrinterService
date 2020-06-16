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
using System.Runtime.CompilerServices;
using System.Threading;

namespace Dominio
{
    public static class Firebase
    {
        private static string _printerName;
        private static FirestoreDb _db;
        private static bool _clean;

        private const string TAKE_AWAY = "TAKEAWAY";
        private const string RESERVA = "RESERVA";
        private const string MESAS = "MESAS";

        private static FirestoreDb AccessDatabaseProduction()
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
        private static FirestoreDb AccessDatabaseTESTING()
        {
            var credential = GoogleCredential.FromJson("{'type':'service_account'," +
                                                       "'project_id':'menootest-9fa5a'," +
                                                       "'private_key_id':'26339e76106194376a318dcb8d8e588b5cef6a78'," +
                                                       "'private_key':'-----BEGIN PRIVATE KEY-----\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDoE0F5Tw5Y/223\nJ5Kpvlg7jbkAwPzy97OMV+AHFOvm8/+ZemQBxIWof+cbS654R0R9HGilFXUF4v/b\nKdchnQN7ZLKAtL16CMEQgBTWTDytbaZWEexWOt7w8itzeQ1+jvKSKgoMvOu5/k+O\nPLo6UyIQQQFcWoBW8NVeM8cx1FsACTjJBUIh6CXDUFS8ze+hbMQEmnhLscD3ZMJR\n+nhJSj2O6GVPAn34DTZRxv9LupqfGzZF2JJLz8tCtSs+aNSyUYIrVV+jYVUQ93xJ\nsufkdn4MAFJ6fVgw8/3BPYjwpSsuYUbshbuc20xJSYjZxoCUngM7Yn03GVsqO/Rp\nYya87AEJAgMBAAECggEAAmjkxqYv/3OTh5HVH4cW8nNbxuq6FanFxwDIljo84taI\nwma3cB9Cxgeh9jIYey4+Q1BOs9wfrXJ4dqWeEr7HIPpgMh7uUryRiKLT1I/RF3nq\nfr1L529QDk9tbRMGNVi1oxflp9E0X2eJGvB9fIqNcX7DTVqxN3XjuUkvWdCbK0po\nKS6Vly1qgpBW5vlyKq4IbAeTDh2uoja352G6G0Yjjczo8lBQeflXQ+A/VQhex/I9\nszCYiW6ZdwVlwxN2zXbpkzy0C0U2rkrXy5dzv3pMlX1IaIuqKlI6ehUQXwNHsPvA\n8/4PuUSbSGcuQLLcpg8FVO6a92EteZyoPY7Y1UltQQKBgQD05bLd5eqoBCaGDqBu\nsFzZGZvZZaAIFfl2Helz7QopF9HZynytXS7jvyFAGsYVNZ0/hltRxOCmp1YLZtBI\nseV9SAFdF+ShGbfTokyYRa3qTM4jcEvfmJiSW5Q0IMEDG+UqB5JmKoRd+YQLzieB\nviEXUXkxXpFranck9b1ZSMXZcQKBgQDymL5BttCnUuBGMSZ9slnNLNR0gVka6xFX\n0+ZSy+M5iwo5zKdgA7UKEuzE6rgGI7Mj6tQRTeO1NUljALQ+KAePdZAYX8BvrzLV\n0mF7GNc5ulsKDJxrPtzn/RBtDDangLr2DcMZggP2BRxUpmG7t5OIzeiVBW6ZHZRW\nPICZrTuVGQKBgQCYmFfvtEeXEZ7/gTWuQu5XyIE34P7qiua6FsFUnqrqGBGGZ4lw\nbNO+zWVmkEhFBvdIkets9AQXU8VlrVazNUYN3kQbQbwQNfo5QLQBXcmUaO85Xcup\nM2g+KhoasR4TVdphaf5q8qsv8z24LWioi1QLN5UQkiCCkgBTY1vsuk+twQKBgExs\nvhMpqpXrz+eM+FlE5HF0nAGP9ig6wZ3vjXGr9YtdN/15cYkX4eKoj5qBbzPP71Fz\nWxeQeBnQDax4vk+OgMM7AAgNsiv8/4DI5BjJfJQdFy0VR/mpNiKHYLNZ06X1MfDt\n6PaSNPk+Juyr9cITVREV/R1lNrBZ1y9LpB/FqS2RAoGBAJIItGLuubQajkcS3uLX\nC169DyaBlzxH1SHrdtAaZM66BtViD5uvNrJW6AsX33yNVAtpCoPP6XPtavIrQ4Rm\nZh2QjblrYDuNGEFNuKv1kVDUS5oHkw1Al87IwT5badRnEm12hm3X92kRMu0AZORk\n4XtL2zupPadZXhzo1bBILIfh\n-----END PRIVATE KEY-----\n'," +
                                                       "'client_email':'menootest-9fa5a@appspot.gserviceaccount.com'," +
                                                       "'client_id': '108009182778403042502'," +
                                                       "'auth_uri':'https://accounts.google.com/o/oauth2/auth'," +
                                                       "'token_uri':'https://oauth2.googleapis.com/token'," +
                                                       "'auth_provider_x509_cert_url': 'https://www.googleapis.com/oauth2/v1/certs'," +
                                                       "'client_x509_cert_url':'https://www.googleapis.com/robot/v1/metadata/x509/menootest-9fa5a%40appspot.gserviceaccount.com'}");
            var channelCredentials = credential.ToChannelCredentials();
            var channel = new Channel(FirestoreClient.DefaultEndpoint.ToString(), channelCredentials);
            var client = FirestoreClient.Create(channel);
            return FirestoreDb.Create("menootest-9fa5a", client);
        }
        private static void Init()
        {
            _printerName = ConfigurationManager.AppSettings["PrinterName"];
            if (_db == null)
                _db = AccessDatabaseTESTING();
        }
        private static void StartListen(string storeId)
        {
            BookingsListen(storeId);
            TableOpeningFamilyListen(storeId);
            OrderFamilyListen(storeId);
            TableOpeningsListen(storeId);
        }
        public static Task RunAsync(bool clean)
        {
            return Task.Run(() =>
            {
                _clean = clean;
                Init();
                var storeId = ConfigurationManager.AppSettings["StoreID"];

                if (string.IsNullOrEmpty(storeId))
                    throw new Exception("No hay storeId");

                StartListen(storeId);
            });
        }

        #region CLOSE TABLE OPENING FAMILY
        private static void TableOpeningsListen(string storeId)
        {
            var date = (DateTime.UtcNow.AddDays(-3) - new DateTime(1970, 1, 1)).TotalSeconds;

            _db.Collection("tableOpeningFamily")
               .WhereEqualTo("storeId", storeId)
               .WhereEqualTo("closed", true)
               .WhereGreaterThanOrEqualTo("openedAtNumber", date)
               .Listen(TableOpeningsCallback);
        }
        private static Action<QuerySnapshot> TableOpeningsCallback = (snapshot) =>
        {
            try
            {
                foreach (var document in snapshot.Documents)
                {
                    TableOpeningFamily to = document.ConvertTo<TableOpeningFamily>();
                    if (to.Closed && !to.ClosedPrinted)
                    {
                        SetTableOpeningFamilyPrintedAsync(document.Id, TableOpeningFamily.PRINTED_EVENT.CLOSING);
                        PrintCloseTableFamily(to);
                    }
                }
            }
            catch (Exception ex)
            {
                LogErrorAsync(ex.Message);
            }
        };
        private static Task<Google.Cloud.Firestore.WriteResult> SetTableOpeningFamilyPrintedAsync(string doc, TableOpeningFamily.PRINTED_EVENT printEvent)
        {
            if (printEvent == TableOpeningFamily.PRINTED_EVENT.CLOSING)
                return _db.Collection("tableOpeningFamily").Document(doc).UpdateAsync("closedPrinted", true);
            else if (printEvent == TableOpeningFamily.PRINTED_EVENT.OPENING)
                return _db.Collection("tableOpeningFamily").Document(doc).UpdateAsync("openPrinted", true);
            else
                throw new Exception("No se actualizo el estado impreso de la mesa.");
        }
        private static void PrintCloseTableFamily(TableOpeningFamily tableOpening)
        {
            if (_clean)
                return;

            PrintDocument pd = new PrintDocument();
            pd.PrinterSettings.PrinterName = _printerName;
            pd.PrintPage += (sender, args) => PrintCloseTableFamily(tableOpening, sender, args);
            pd.Print();
        }
        private static void PrintCloseTableFamily(TableOpeningFamily tableOpening, object sender, PrintPageEventArgs args)
        {
            var y = PrintLogo(args);
            if (tableOpening.Closed)
            {
                y = PrintTitle(args, "Mesa cerrada", y);
                y = PrintText(args, "Número de mesa: " + tableOpening.TableNumberId, y);
                y = PrintText(args, "Fecha: " + tableOpening.ClosedAt, y);
                y = PrintText(args, "Total: $ " + tableOpening.TotalToPay, y);
            }
        }
        #endregion

        #region ORDER FAMILY
        private static void PrintOrder(Orders orden, object sender, PrintPageEventArgs args)
        {
            double calculatedTotal = 0;
            string text = string.Empty;
            string comment = string.Empty;
            string options = string.Empty;

            foreach (var item in orden.Items)
            {
                if (item.Options != null)
                    foreach (var option in item.Options)
                        if (option != null)
                            options += option.Name + " - ";

                text += $"{item.Name} x{item.Quantity} {options} $ {item.SubTotal} {Environment.NewLine}";
                calculatedTotal = calculatedTotal + item.SubTotal * item.Quantity;
                comment += item.GuestComment + " - ";
            }

            if (orden != null && orden.GuestComment != null)
                comment += orden.GuestComment;

            bool orderOk = orden != null && orden.OrderType != null;

            if (orderOk && orden.OrderType.ToUpper().Trim() == TAKE_AWAY)
            {
                var task = GetTakeAwayComments(orden.TableOpeningFamilyId);
                task.Wait();
                comment += task.Result;
            }

            if (orderOk && orden.OrderType.ToUpper().Trim() == MESAS)
            {
                text = text.Substring(0, text.Length - 2); //Saco el \n
                var y = PrintLogo(args);
                y = PrintTitle(args, "Nueva órden de mesa", y);
                y = PrintText(args, $"Cliente: {orden.UserName}", y);
                y = PrintText(args, text, y);
                y = PrintText(args, $"Observaciones: {comment}", y);
                y = PrintText(args, $"Servir en mesa: {orden.Address}", y);
            }
            else if (orderOk && orden.OrderType.ToUpper().Trim() == RESERVA)
            {
                var y = PrintLogo(args);
                y = PrintTitle(args, "Nueva órden de reserva", y);
                y = PrintText(args, $"Cliente: {orden.UserName}", y);
                y = PrintText(args, text, y);
                y = PrintText(args, $"Observaciones: {comment}", y);
                y = PrintText(args, $"Servir en mesa: {orden.Address}", y);
                y = PrintText(args, $"Total: $ {calculatedTotal}", y);
            }
            else if (orderOk && orden.OrderType.ToUpper().Trim() == TAKE_AWAY)
            {
                var y = PrintLogo(args);
                y = PrintTitle(args, "Nuevo Take Away", y);
                y = PrintText(args, $"Cliente: {orden.UserName}", y);
                y = PrintText(args, text, y);
                y = PrintText(args, $"Observaciones: {comment}", y);
                y = PrintText(args, $"Hora del retiro: {orden.TakeAwayHour}", y);
                y = PrintText(args, $"Total: $ {calculatedTotal}", y);
            }
        }
        private static void OrderFamilyListen(string storeId)
        {
            _db.Collection("orderFamily")
               .WhereEqualTo("storeId", storeId)
               .OrderByDescending("createdAt")
               .Limit(1)
               .Listen(OrderFamilyListenCallback);
        }
        private static Action<QuerySnapshot> OrderFamilyListenCallback = (snapshot) =>
        {
            try
            {
                var document = snapshot.Documents.Single();
                Orders orden = document.ConvertTo<Orders>();
                if (!orden.Printed)
                {
                    SetOrderFamilyPrintedAsync(document.Id);
                    PrintOrder(orden);
                }
            }
            catch (Exception ex)
            {
                LogErrorAsync(ex.Message);
            }
        };
        private static Task<Google.Cloud.Firestore.WriteResult> SetOrderFamilyPrintedAsync(string doc)
        {
            return _db.Collection("orderFamily").Document(doc).UpdateAsync("printed", true);
        }
        private static void PrintOrder(Orders order)
        {
            if (_clean)
                return;

            PrintDocument pd = new PrintDocument();
            pd.PrinterSettings.PrinterName = _printerName;
            pd.PrintPage += (sender, args) => PrintOrder(order, sender, args);
            pd.Print();
        }
        #endregion

        #region OPEN TABLEOPENING
        private static void TableOpeningFamilyListen(string storeId)
        {
            _db.Collection("tableOpeningFamily")
               .WhereEqualTo("storeId", storeId)
               .OrderByDescending("openedAtNumber")
               .Limit(1).Listen(TableOpeningFamilyCallback);
        }
        private static Action<QuerySnapshot> TableOpeningFamilyCallback = (snapshot) =>
        {
            try
            {
                var document = snapshot.Documents.Single();
                TableOpeningFamily to = document.ConvertTo<TableOpeningFamily>();
                if (!to.Closed && !to.OpenPrinted)
                {
                    SetTableOpeningFamilyPrintedAsync(document.Id, TableOpeningFamily.PRINTED_EVENT.OPENING);
                    PrintLastTableOpening(to);
                }
            }
            catch (Exception ex)
            {
                LogErrorAsync(ex.Message);
            }
        };
        private static void PrintLastTableOpening(TableOpeningFamily to)
        {
            if (_clean)
                return;

            PrintDocument pd = new PrintDocument();
            pd.PrinterSettings.PrinterName = _printerName;
            pd.PrintPage += (sender, args) => PrintLastTableOpening(to, sender, args);
            pd.Print();
        }
        private static void PrintLastTableOpening(TableOpeningFamily tableOpening, object sender, PrintPageEventArgs args)
        {
            var y = PrintLogo(args);
            y = PrintTitle(args, "Apertura de mesa", y);
            y = PrintText(args, "Número de mesa: " + tableOpening.TableNumberId, y);
            y = PrintText(args, "Fecha: " + tableOpening.OpenedAt, y);
            y = PrintText(args, "Comentarios: " + tableOpening.BookingObservations, y);
            y = PrintText(args, "Número de Personas: " + tableOpening.ActiveGuestQuantity, y);
        }
        #endregion

        #region BOOKINGS
        private static void PrintBooking(Booking booking, User user)
        {
            if (_clean)
                return;

            PrintDocument pd = new PrintDocument();
            pd.PrinterSettings.PrinterName = ConfigurationManager.AppSettings["PrinterName"];
            pd.PrintPage += (sender, args) => PrintBooking(booking, user, sender, args);
            pd.Print();
        }
        private static void BookingsListen(string storeId)
        {
            //When new booking is created
            _db.Collection("bookings")
               .WhereEqualTo("store.id", storeId)
               .OrderByDescending("updatedAt")
               .Limit(1)
               .Listen(BookingsAceptedCallback);

            //When booking is cancelled
            _db.Collection("bookings")
               .WhereEqualTo("store.id", storeId)
               .WhereEqualTo("bookingState", "cancelada")
               .Listen(BookingsCancelCallback);
        }
        private static Action<QuerySnapshot> BookingsCancelCallback = async (snapshot) =>
        {
            try
            {
                foreach (var document in snapshot.Documents)
                {
                    Booking booking = document.ConvertTo<Booking>();
                    User user = null;
                    var snapshotUser = await _db.Collection("customers").Document(booking.UserId).GetSnapshotAsync();
                    if (snapshotUser.Exists)
                        user = snapshotUser.ConvertTo<User>();
                    if (!booking.PrintedCancelled)
                    {
                        await SetBookingPrintedAsync(document.Id, Booking.PRINT_TYPE.CANCELLED);
                        PrintBooking(booking, user);
                    }
                }
            }
            catch (Exception ex)
            {
                _ = LogErrorAsync(ex.Message);
            }
        };
        private static Action<QuerySnapshot> BookingsAceptedCallback = async (snapshot) =>
        {
            try
            {
                var document = snapshot.Single();
                Booking booking = document.ConvertTo<Booking>();
                User user = null;
                var snapshotUser = await _db.Collection("customers").Document(booking.UserId).GetSnapshotAsync();
               
                if (snapshotUser.Exists)
                    user = snapshotUser.ConvertTo<User>();

                if (booking != null && !booking.PrintedAccepted && booking.BookingNumber.ToString().Length == 8)
                {
                    await SetBookingPrintedAsync(document.Id, Booking.PRINT_TYPE.ACCEPTED);
                    PrintBooking(booking, user);
                }
            }
            catch (Exception ex)
            {
                _ = LogErrorAsync(ex.Message);
            }
        };
        private static Task<Google.Cloud.Firestore.WriteResult> SetBookingPrintedAsync(string doc, Booking.PRINT_TYPE type)
        {
            return _db.Collection("bookings")
                      .Document(doc)
                      .UpdateAsync(type == Booking.PRINT_TYPE.ACCEPTED ? "printedAccepted"
                                                                       : "printedCancelled", true);
        }
        private static void PrintBooking(Booking booking, User user, object sender, PrintPageEventArgs ev)
        {
            var y = PrintLogo(ev);

            if (booking.BookingState == "cancelada")
            {
                y = PrintTitle(ev, "Reserva cancelada", y);
                y = PrintText(ev, "Nro: " + booking.BookingNumber, y);
                y = PrintText(ev, "Fecha: " + booking.BookingDate, y);
                y = PrintText(ev, "Cliente: " + user.Name, y);
            }
            else
            {
                y = PrintTitle(ev, "Nueva reserva", y);
                y = PrintText(ev, "Nro: " + booking.BookingNumber, y);
                y = PrintText(ev, "Cantidad de personas: " + booking.GuestQuantity, y);
                y = PrintText(ev, "Fecha: " + booking.BookingDate, y);
                y = PrintText(ev, "Cliente: " + user.Name, y);
                y = PrintText(ev, "Comentario: " + booking.BookingObservations, y);
            }
        }
        #endregion

        private static Task<string> GetTakeAwayComments(string takeAwayOpeningId)
        {
            return Task.Run(async () =>
            {
                if (string.IsNullOrEmpty(takeAwayOpeningId))
                    return string.Empty;

                var snapshot = await _db.Collection("takeAwayOpenings").Document(takeAwayOpeningId).GetSnapshotAsync();
                var takeAway = snapshot.ToDictionary();

                if (takeAway.ContainsKey("observations"))
                    return takeAway["observations"].ToString();

                return string.Empty;
            });
        }
        public static Task<List<Store>> GetStores()
        {
            return Task.Run(async () =>
            {
                Init();

                List<Store> stores = new List<Store>();
                var snapshot = await _db.Collection("stores").GetSnapshotAsync();
                foreach (var doc in snapshot.Documents)
                {
                    var store = doc.ToDictionary();
                    var storeName = store.ContainsKey("name") ? store["name"].ToString() : string.Empty; ;
                    stores.Add(new Store { Id = doc.Id, Name = storeName });
                }
                return stores;
            });
        }
        private static int PrintLogo(PrintPageEventArgs e)
        {
            Image img = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "\\logo.png");
            Point loc = new Point(50, 0);
            img = new Bitmap(img, new System.Drawing.Size(img.Width / 5, img.Height / 5));
            e.Graphics.DrawImage(img, loc);
            return img.Height + 10;
        }
        private static int PrintTitle(PrintPageEventArgs e, string title, int y)
        {
            Font font = new Font("Arial", 13, FontStyle.Bold);
            Font variationsFont = new Font("Arial", 9, FontStyle.Italic);
            e.Graphics.DrawString(title, font, Brushes.Black, 50, y);
            return y + 30;  //Posicion actual mas el salto de linea.
        }
        private static int PrintText(PrintPageEventArgs e, string line, int y)
        {
            Font font = new Font("Arial", 11);
            SizeF sizeF = e.Graphics.MeasureString(line, font, 300);
            e.Graphics.DrawString(line, font, Brushes.Black, new RectangleF(new PointF(0, y), sizeF));
            return y + (int)sizeF.Height; //Posicion actual mas el salto de linea.
        }

        #region LOG
        private static Task LogErrorAsync(string error)
        {
            return Task.Run(() =>
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
                }
                finally
                {
                    if (sw != null)
                        sw.Close();
                    if (fs != null)
                        fs.Close();
                }
            });
        }
        #endregion
    }
}
