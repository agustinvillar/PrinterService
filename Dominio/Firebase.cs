using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Google.Cloud.Firestore;
using Google.Apis.Auth.OAuth2;
using Grpc.Auth;
using Grpc.Core;
using Google.Cloud.Firestore.V1;
using System.Configuration;
using static Dominio.Ticket;
using Google.Apis.Util;
using Google.Protobuf.WellKnownTypes;
using System.Runtime.CompilerServices;
using System.Globalization;

namespace Dominio
{
    public static class Firebase
    {
        #region Attributes
        private static FirestoreDb _db;
        private static bool _clean;

        private const string TAKE_AWAY = "TAKEAWAY";
        private const string RESERVA = "RESERVA";
        private const string MESAS = "MESAS";

        #endregion

        #region FirstMethods
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
            if (_db == null)
                _db = AccessDatabaseTESTING();
        }
        private static void StartListen()
        {
            BookingsListen();
            TableOpeningFamilyListen();
            OrderFamilyListen();
            TableOpeningsListen();
        }
        public static Task RunAsync(bool clean)
        {
            return Task.Run(() =>
            {
                _clean = clean;
                Init();
                StartListen();
            });
        }
        #endregion

        #region CLOSE TABLE OPENING
        private static void TableOpeningsListen()
        {
            var date = (DateTime.UtcNow.AddDays(-3) - new DateTime(1970, 1, 1)).TotalSeconds;

            _db.Collection("tableOpeningFamily")
               .WhereEqualTo("closed", true)
               .WhereGreaterThanOrEqualTo("openedAtNumber", date)
               .Listen(TableOpeningsCallback);
        }
        private static Action<QuerySnapshot> TableOpeningsCallback = (snapshot) =>
        {
            try
            {
                var docs = snapshot.Documents.Select(d =>
                {
                    var dic = d.ToDictionary();
                    var toFamily = d.ConvertTo<TableOpeningFamily>();
                    //var tableOpenings = (List<object>)dic["tableOpenings"];
                    //for (int i = 0; i < tableOpenings.Count; i++)
                    //{
                    //    Dictionary<string, object> tableOpeningsParsed = (Dictionary<string, object>)tableOpenings.ElementAt(i);
                    //    var ordersParsed = (List<object>)tableOpeningsParsed["orders"] ?? new List<object>();
                    //    if (ordersParsed.Count > 0)
                    //    {
                    //        for (int j = 0; j < ordersParsed.Count; j++)
                    //        {
                    //            Dictionary<string, object> order = (Dictionary<string, object>)ordersParsed.ElementAt(j);
                    //            var address = order["address"];
                    //            toFamily.TableOpenings[i].Orders[j].Address = address.ToString();
                    //        }
                    //    }
                    //}
                    foreach (var to in toFamily.TableOpenings)
                    {
                        to.CulteryPrice = dic.ContainsKey("culteryPrice") ? int.Parse(dic["culteryPrice"].ToString()) : 0;
                    }
                    toFamily.TotalToPay = dic.ContainsKey("totalToPay") && dic["totalToPay"] != null ? double.Parse(dic["totalToPay"].ToString()) : 0;
                    toFamily.TotalToPayWithPropina = dic.ContainsKey("totalToPayWithPropina") && dic["totalToPayWithPropina"] != null
                        ? double.Parse(dic["totalToPayWithPropina"].ToString()) : 0;

                    return toFamily;
                });
                foreach (var tableOpeningFamily in docs)
                {
                    if (tableOpeningFamily.Closed && !tableOpeningFamily.ClosedPrinter)
                    {
                        SetTableOpeningFamilyPrintedAsync(tableOpeningFamily.Id, TableOpeningFamily.PRINTED_EVENT.CLOSING);
                        SaveCloseTableOpeningFamily(tableOpeningFamily);
                    }
                }
            }
            catch (Exception ex)
            {
                LogErrorAsync(ex.Message);
            }
        };
        private static Task SaveCloseTableOpeningFamily(TableOpeningFamily tableOpening)
        {
            return Task.Run(async () =>
            {
                var tableOpeningFamilyAlreadyExists = await TableOpeningFamilyAlreadyExists(tableOpening.Id);
                if (tableOpeningFamilyAlreadyExists)
                    return Task.CompletedTask;

                var stores = GetStores();
                var store = stores.Result.Find(s => s.StoreId.Equals(tableOpening.StoreId));
                if (AllowPrint(store))
                {
                    var payment = GetPayment(tableOpening.TableOpenings.FirstOrDefault().Id);

                    Ticket ticket = CreateInstanceOfTicket();
                    ticket.TicketType = TicketTypeEnum.CLOSE_TABLE.ToString();

                    if (tableOpening.Closed)
                    {
                        var title = SetTitleForCloseTable(tableOpening);
                        var tableNumber = "Número de mesa: " + tableOpening.TableNumberId;
                        var orden = "<b>--Pedidos: </b>";
                        foreach (var to in tableOpening.TableOpenings)
                        {
                            orden += "<p>" + to.UserName + "</p>";
                            foreach (var order in to.Orders)
                            {
                                foreach (var item in order.Items)
                                {
                                    orden += "<p>" + GetTime(order.MadeAt) + " " + item.Name + " x " + item.Quantity + " unidades $" + item.Price + "</p>";
                                    if (item.Options != null)
                                    {
                                        foreach (var option in item.Options)
                                            if (!option.Equals(null) && !option.Equals(""))
                                                orden += "<p>" + option.Name + " " + option.Price + "</p>";
                                    }
                                }
                            }
                            orden += "<p>Cubiertos: " + to.CulteryPrice + "</p>";
                            if (to.Discounts != null && to.Discounts.Length != 0)
                                foreach (var discount in to.Discounts)
                                    orden += "<p>" + discount.Name + " - " + discount.Amount + "</p>";
                            if (!to.Surcharge.Equals(null))
                                orden += "<p>Adicional por servicio: $" + to.Surcharge + "</p>";
                            orden += "<p>Subtotal consumido: $" + to.TotalToPay + "</p>";
                        }
                        orden += "<p>TOTAL consumido: $" + tableOpening.TotalToPay + "</p>";
                        var date = "Fecha: " + tableOpening.ClosedAt;
                        var paymentMethod = "";
                        if (payment != null)
                            paymentMethod = "Método de pago: " + payment.PaymentMethod + " - " + payment.PaymentType;

                        ticket.Data += "<h1>" + title + "</h1><h3><p>" + tableNumber + "</p><p>" + date + "</p><p>" + orden + "</p><p>"
                        + paymentMethod + "</p></h3></body></html>";
                    }
                    ticket.StoreId = tableOpening.StoreId;
                    ticket.TableOpeningFamilyId = tableOpening.Id;
                    ticket.PrintBefore = BeforeAt(tableOpening.ClosedAt, 10);
                    ticket.Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
                    return _db.Collection("print").AddAsync(ticket);
                }
                return Task.CompletedTask;
            });
        }
        private static string SetTitleForCloseTable(TableOpeningFamily tableOpening)
        {
            string title;
            if (tableOpening.Pending == true)
                title = "Mesa abandonada";
            else
                title = "Mesa cerrada";
            return title;
        }
        private static Payment GetPayment(string tableOpeningId)
        {
            DateTime to = DateTime.Now;
            DateTime from = to.AddMinutes(-5);
            long fromUnix = new DateTimeOffset(from).ToUnixTimeMilliseconds();
            long toUnix = new DateTimeOffset(to.AddHours(23).AddMinutes(59).AddSeconds(59)).ToUnixTimeMilliseconds();
            var task = _db.Collection("payments").WhereGreaterThanOrEqualTo("paymentDate", fromUnix)
                                                     .WhereLessThanOrEqualTo("paymentDate", toUnix)
                                                     .GetSnapshotAsync();

            task.Wait();
            var documentSnapshots = task.Result;
            var payments = documentSnapshots.Documents.Select(d =>
            {
                var p = d.ConvertTo<Payment>();
                var dic = d.ToDictionary();
                p.PaymentType = string.Empty;
                p.PaymentMethod = string.Empty;
                if (dic.ContainsKey("tableOpening"))
                {
                    p.TableOpeningId = (dic["tableOpening"] as Dictionary<string, object>)["id"].ToString();
                    p.PaymentMethod = dic["payMethod"] != null ? dic["payMethod"].ToString() : "";
                    p.PaymentType = dic["paymentType"] != null ? dic["paymentType"].ToString() : "";
                }
                return p;
            }).ToList();
            return payments.FirstOrDefault(p => p.TableOpeningId == tableOpeningId);
        }
        #endregion

        #region OPEN TABLE OPENING
        private static void TableOpeningFamilyListen()
        {
            _db.Collection("tableOpeningFamily")
               .OrderByDescending("openedAtNumber")
               .Limit(1)
               .Listen(TableOpeningFamilyCallback);
        }
        private static Action<QuerySnapshot> TableOpeningFamilyCallback = (snapshot) =>
        {
            try
            {
                var document = snapshot.Documents.Single();
                TableOpeningFamily tableOpeningFamily = document.ConvertTo<TableOpeningFamily>();
                if (!tableOpeningFamily.Closed && !tableOpeningFamily.OpenPrinted)
                {
                    SetTableOpeningFamilyPrintedAsync(document.Id, TableOpeningFamily.PRINTED_EVENT.OPENING);
                    SaveOpenTableOpeningFamily(tableOpeningFamily);
                }
            }
            catch (Exception ex)
            {
                LogErrorAsync(ex.Message);
            }
        };
        private static void SaveOpenTableOpeningFamily(TableOpeningFamily tableOpeningFamily)
        {
            var stores = GetStores();
            var store = stores.Result.Find(s => s.StoreId.Equals(tableOpeningFamily.StoreId));
            if (AllowPrint(store))
            {
                if (_clean)
                    return;
                Ticket ticket = CreateInstanceOfTicket();
                ticket.TicketType = TicketTypeEnum.OPEN_TABLE.ToString();

                var title = "Apertura de mesa";
                var tableNumber = "Número de mesa: " + tableOpeningFamily.TableNumberId;
                var date = "Fecha: " + tableOpeningFamily.OpenedAt;

                ticket.Data += "<h1>" + title + "</h1><h3><p>" + tableNumber + "</p><p>" + date + "</p></h3></body></html>";

                ticket.PrintBefore = BeforeAt(tableOpeningFamily.OpenedAt, 10);
                ticket.StoreId = tableOpeningFamily.StoreId;
                ticket.Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
                ticket.TableOpeningFamilyId = tableOpeningFamily.Id;
                _db.Collection("print").AddAsync(ticket);
            }
        }
        #endregion

        #region ORDERS
        private static void OrderFamilyListen()
        {
            _db.Collection("orderFamily")
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
                var dic = snapshot.Documents.Single().ToDictionary();
                var items = (List<object>)dic["items"];
                orden.Address = dic.ContainsKey("address") && dic["address"] != null ? dic["address"].ToString() : " ";

                for (int i = 0; i < items.Count; i++)
                {
                    Dictionary<string, object> itemParsed = (Dictionary<string, object>)items.ElementAt(i);
                    var itemOptions = (List<object>)itemParsed["options"] ?? new List<object>();
                    if (itemOptions.Count > 0)
                    {
                        for (int j = 0; j < itemOptions.Count; j++)
                        {
                            Dictionary<string, object> optionParsed = (Dictionary<string, object>)itemOptions.ElementAt(j);
                            var price = optionParsed["price"];
                            var name = optionParsed["name"];
                            orden.Items[i].Options[j].Name = name.ToString();
                            orden.Items[i].Options[j].Price = price.ToString();
                        }
                    }


                    if (!orden.Printed)
                    {
                        SaveOrder(orden);
                    }
                }
            }
            catch (Exception ex)
            {
                LogErrorAsync(ex.Message);
            }
        };
        private static void SaveOrder(Orders order)
        {
            var stores = GetStores();
            var store = stores.Result.Find(s => s.StoreId.Equals(order.StoreId));
            if (AllowPrint(store))
            {
                string comment = string.Empty;
                Ticket ticket = CreateInstanceOfTicket();
                List<string> lines = CreateComments(order);

                bool isOrderOk = order != null && order.OrderType != null;
                if (IsTakeAway(order, isOrderOk))
                    ticket.PrintBefore = BeforeAt(order.OrderDate, -5);
                else
                {
                    ticket.PrintBefore = BeforeAt(order.OrderDate, 30);
                    ticket.TableOpeningFamilyId = order.TableOpeningFamilyId;
                }
                string line = CreateHTMLFromLines(lines);
                CreateOrderTicket(order, isOrderOk, ticket, line);
                ticket.StoreId = order.StoreId;
                ticket.Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
                _db.Collection("print").AddAsync(ticket);
            }
        }
        private static string CreateHTMLFromLines(List<string> lines)
        {
            string res = string.Empty;
            foreach (var line in lines)
            {
                res += "<p>" + line + "</p>";
            }
            return res;
        }
        private static void CreateOrderTicket(Orders order, bool isOrderOk, Ticket ticket, string line)
        {
            ticket.TicketType = TicketTypeEnum.ORDER.ToString();
            string title, client;

            if (isOrderOk && order.OrderType.ToUpper().Trim() == MESAS)
            {
                title = "Nueva orden de mesa";
                client = "Cliente: " + order.UserName;
                var table = "Servir en mesa: " + order.Address;
                ticket.Data += "<h1>" + title + "</h1><h3>" + client + line + table + "</h3></body></html>";
            }
            else if (isOrderOk && order.OrderType.ToUpper().Trim() == RESERVA)
            {
                title = "Nueva orden de reserva";
                client = "Cliente: " + order.UserName;
                var table = "Servir en mesa: " + order.Address;
                ticket.Data += "<h1>" + title + "</h1><h3>" + client + line + table + "</h3></body></html>";
            }
            else if (isOrderOk && order.OrderType.ToUpper().Trim() == TAKE_AWAY)
            {
                title = "Nuevo Take Away";
                client = "Cliente: " + order.UserName;
                var time = "Hora de retiro: " + order.TakeAwayHour;
                ticket.Data += "<h1>" + title + "</h1><h3>" + client + line + time + "</h3></body></html>";
            }
        }
        private static List<string> CreateComments(Orders order)
        {
            List<string> lines = new List<string>();
            double total = 0;
            foreach (var i in order.Items)
            {
                if (order.Items.Length == 0)
                    break;
                else
                {
                    lines.Add("<b>--" + i.Name + "</b>" + " x" + i.Quantity);
                    if (i.Options != null)
                    {
                        foreach (var option in i.Options)
                        {
                            if (!option.Equals(null) && !option.Equals(""))
                            {
                                lines.Add(option.Name);
                            }
                        }
                    }
                    lines.Add("Precio: $" + i.SubTotal);
                    total += i.SubTotal * i.Quantity;
                    lines.Add("Comentario: " + i.GuestComment);
                }
            }
            lines.Add("Total: $" + total);
            return lines;
        }
        private static bool IsTakeAway(Orders order, bool orderOk)
        {
            if (orderOk && order.OrderType.ToUpper().Trim() == TAKE_AWAY)
                return true;
            return false;
        }
        #endregion

        #region BOOKINGS
        private static void BookingsListen()
        {
            //When new booking is created
            _db.Collection("bookings")
               .OrderByDescending("updatedAt")
               .Limit(1)
               .Listen(BookingsAcceptedCallback);

            //When booking is cancelled
            _db.Collection("bookings")
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
                        SaveCancelledBooking(booking, user);
                    }
                }
            }
            catch (Exception ex)
            {
                _ = LogErrorAsync(ex.Message);
            }
        };
        private static Action<QuerySnapshot> BookingsAcceptedCallback = async (snapshot) =>
        {
            try
            {
                var document = snapshot.Single();
                Booking booking = document.ConvertTo<Booking>();
                User user = null;
                var d = document.ToDictionary();
                var snapshotUser = await _db.Collection("customers").Document(booking.UserId).GetSnapshotAsync();

                if (snapshotUser.Exists)
                    user = snapshotUser.ConvertTo<User>();

                if (booking != null && !booking.PrintedAccepted)
                {
                    await SetBookingPrintedAsync(document.Id, Booking.PRINT_TYPE.ACCEPTED);
                    SaveAcceptedBooking(booking, user);
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
        private static void SaveCancelledBooking(Booking booking, User user)
        {
            var stores = GetStores();
            var store = stores.Result.Find(s => s.StoreId.Equals(booking.Store.StoreId));

            if (AllowPrint(store))
            {
                var ticket = CreateInstanceOfTicket();
                ticket.TicketType = TicketTypeEnum.CANCELLED_BOOKING.ToString();
                if (booking.BookingState.Equals("cancelada"))
                {
                    var title = "Reserva cancelada";
                    var nroReserva = "Nro: " + booking.BookingNumber;
                    var fecha = "Fecha: " + booking.BookingDate;
                    string cliente = string.Empty;
                    if (user != null)
                        cliente = "Cliente: " + user.Name;
                    ticket.Data += "<h1>" + title + "</h1><h3><p>" + nroReserva + "</p><p>" + fecha + "</p><p>" + cliente + "</p></h3></body></html>";
                }
                ticket.PrintBefore = BeforeAt(booking.BookingDate, -10);
                ticket.StoreId = booking.Store.StoreId;
                ticket.Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
                _db.Collection("print").AddAsync(ticket);
            }
        }
        private static void SaveAcceptedBooking(Booking booking, User user)
        {
            var stores = GetStores();
            var store = stores.Result.Find(s => s.StoreId.Equals(booking.Store.StoreId));
            if (AllowPrint(store))
            {
                var ticket = CreateInstanceOfTicket();
                ticket.TicketType = TicketTypeEnum.NEW_BOOKING.ToString();
                if (booking.BookingState.Equals("aceptada"))
                {
                    var title = "Nueva reserva";
                    var nroReserva = "Nro: " + booking.BookingNumber;
                    var fecha = "Fecha: " + booking.BookingDate;
                    var cantPersonas = "Cantidad de personas: " + booking.GuestQuantity;
                    var cliente = "Cliente: " + user.Name;
                    ticket.Data += "<h1>" + title + "</h1><h3><p>" + nroReserva + "</p><p>" + fecha + "</p><p>" + cantPersonas + "</p><p>" + cliente + "</p></h3></body></html>";
                }
                ticket.PrintBefore = BeforeAt(booking.BookingDate, -10);
                ticket.StoreId = booking.Store.StoreId;
                ticket.Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
                _db.Collection("print").AddAsync(ticket);
            }

        }
        #endregion

        #region FinalMethods
        private static bool AllowPrint(Store store)
        {
            return store != null && store.AllowPrinting != null && store.AllowPrinting.Value;
        }
        private static string BeforeAt(string date, int minutes)
        {
            DateTime result;

            bool ok = DateTime.TryParseExact(date, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);

            if (!ok) //Cuando es mesa cerrada, llega con otro formato.
                DateTime.TryParseExact(date, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);

            return result.AddMinutes(minutes).ToString("yyyy/MM/dd HH:mm");
        }
        private static Ticket CreateInstanceOfTicket()
        {
            return new Ticket()
            {
                PrintedAt = null,
                Expired = false,
                Data = "<!DOCTYPE html><html><body><div class='logoImg'> <img src='\\assets\\img\\Menoo_Logo-Final_color-3.png'> </div>",
                Printed = false,
            };
        }
        private static string GetTime(string dateTime)
        {
            var splitDateTime = dateTime.Split(' ');
            return splitDateTime[1];
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
                    var storeName = store.ContainsKey("name") ? store["name"].ToString() : string.Empty;
                    var allowPrinting = store.ContainsKey("allowPrinting") ? store["allowPrinting"] : false;
                    if (allowPrinting == null) allowPrinting = false;
                    stores.Add(new Store { StoreId = doc.Id, Name = storeName, AllowPrinting = (bool)allowPrinting });
                }
                return stores;
            });
        }
        private static async Task<bool> TableOpeningFamilyAlreadyExists(string id)
        {
            if (string.IsNullOrEmpty(id))
                return true;
            var query = await _db.Collection("print").WhereEqualTo("tableOpeningFamilyId", id).GetSnapshotAsync();
            return await Task.FromResult(query.Documents.Count == 1);
        }
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
        private static Task<Google.Cloud.Firestore.WriteResult> SetTableOpeningFamilyPrintedAsync(string doc, TableOpeningFamily.PRINTED_EVENT printEvent)
        {
            if (printEvent == TableOpeningFamily.PRINTED_EVENT.CLOSING)
                return _db.Collection("tableOpeningFamily").Document(doc).UpdateAsync("closedPrinted", true);
            else if (printEvent == TableOpeningFamily.PRINTED_EVENT.OPENING)
                return _db.Collection("tableOpeningFamily").Document(doc).UpdateAsync("openPrinted", true);
            else
                throw new Exception("No se actualizo el estado impreso de la mesa.");
        }
        #endregion
    }
}
