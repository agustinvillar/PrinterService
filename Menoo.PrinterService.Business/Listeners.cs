using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Grpc.Auth;
using Grpc.Core;
using Menoo.PrinterService.Business.Bookings;
using System;
using System.Configuration;

namespace Menoo.PrinterService.Business
{
    /// <summary>Eventos al realizar cambios en alguna de los documentos en FireBase.</summary>
    public class Listeners
    {
        private readonly FirestoreDb _db;

        private readonly BookingManager _bookingManager;

        public Listeners() 
        {
            _db = GetInstanceDb();
            _bookingManager = new BookingManager(_db);
        }

        public void Load() 
        {
            _bookingManager.Listen();
            TableOpeningFamily();
            OrderFamily();
            TableOpenings();
            OrdersCancelled();
        }

        #region listeners
        private void OrderFamily()
        {
            throw new NotImplementedException();
        }

        private void OrdersCancelled()
        {
            throw new NotImplementedException();
        }

        private void TableOpeningFamily()
        {
            throw new NotImplementedException();
        }

        private void TableOpenings()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region private methods

        /// <summary>Instancia de firebase en produción.</summary>
        /// <returns>Objeto gestor de la base de datos.</returns>
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

        /// <summary>Instancia de firebase en testing.</summary>
        /// <returns>Objeto gestor de la base de datos.</returns>
        private static FirestoreDb AccessDatabaseTesting()
        {
            string jsonCredentials = "{'type':'service_account'," +
                                                       "'project_id':'menootest-9fa5a'," +
                                                       "'private_key_id':'26339e76106194376a318dcb8d8e588b5cef6a78'," +
                                                       "'private_key':'-----BEGIN PRIVATE KEY-----\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDoE0F5Tw5Y/223\nJ5Kpvlg7jbkAwPzy97OMV+AHFOvm8/+ZemQBxIWof+cbS654R0R9HGilFXUF4v/b\nKdchnQN7ZLKAtL16CMEQgBTWTDytbaZWEexWOt7w8itzeQ1+jvKSKgoMvOu5/k+O\nPLo6UyIQQQFcWoBW8NVeM8cx1FsACTjJBUIh6CXDUFS8ze+hbMQEmnhLscD3ZMJR\n+nhJSj2O6GVPAn34DTZRxv9LupqfGzZF2JJLz8tCtSs+aNSyUYIrVV+jYVUQ93xJ\nsufkdn4MAFJ6fVgw8/3BPYjwpSsuYUbshbuc20xJSYjZxoCUngM7Yn03GVsqO/Rp\nYya87AEJAgMBAAECggEAAmjkxqYv/3OTh5HVH4cW8nNbxuq6FanFxwDIljo84taI\nwma3cB9Cxgeh9jIYey4+Q1BOs9wfrXJ4dqWeEr7HIPpgMh7uUryRiKLT1I/RF3nq\nfr1L529QDk9tbRMGNVi1oxflp9E0X2eJGvB9fIqNcX7DTVqxN3XjuUkvWdCbK0po\nKS6Vly1qgpBW5vlyKq4IbAeTDh2uoja352G6G0Yjjczo8lBQeflXQ+A/VQhex/I9\nszCYiW6ZdwVlwxN2zXbpkzy0C0U2rkrXy5dzv3pMlX1IaIuqKlI6ehUQXwNHsPvA\n8/4PuUSbSGcuQLLcpg8FVO6a92EteZyoPY7Y1UltQQKBgQD05bLd5eqoBCaGDqBu\nsFzZGZvZZaAIFfl2Helz7QopF9HZynytXS7jvyFAGsYVNZ0/hltRxOCmp1YLZtBI\nseV9SAFdF+ShGbfTokyYRa3qTM4jcEvfmJiSW5Q0IMEDG+UqB5JmKoRd+YQLzieB\nviEXUXkxXpFranck9b1ZSMXZcQKBgQDymL5BttCnUuBGMSZ9slnNLNR0gVka6xFX\n0+ZSy+M5iwo5zKdgA7UKEuzE6rgGI7Mj6tQRTeO1NUljALQ+KAePdZAYX8BvrzLV\n0mF7GNc5ulsKDJxrPtzn/RBtDDangLr2DcMZggP2BRxUpmG7t5OIzeiVBW6ZHZRW\nPICZrTuVGQKBgQCYmFfvtEeXEZ7/gTWuQu5XyIE34P7qiua6FsFUnqrqGBGGZ4lw\nbNO+zWVmkEhFBvdIkets9AQXU8VlrVazNUYN3kQbQbwQNfo5QLQBXcmUaO85Xcup\nM2g+KhoasR4TVdphaf5q8qsv8z24LWioi1QLN5UQkiCCkgBTY1vsuk+twQKBgExs\nvhMpqpXrz+eM+FlE5HF0nAGP9ig6wZ3vjXGr9YtdN/15cYkX4eKoj5qBbzPP71Fz\nWxeQeBnQDax4vk+OgMM7AAgNsiv8/4DI5BjJfJQdFy0VR/mpNiKHYLNZ06X1MfDt\n6PaSNPk+Juyr9cITVREV/R1lNrBZ1y9LpB/FqS2RAoGBAJIItGLuubQajkcS3uLX\nC169DyaBlzxH1SHrdtAaZM66BtViD5uvNrJW6AsX33yNVAtpCoPP6XPtavIrQ4Rm\nZh2QjblrYDuNGEFNuKv1kVDUS5oHkw1Al87IwT5badRnEm12hm3X92kRMu0AZORk\n4XtL2zupPadZXhzo1bBILIfh\n-----END PRIVATE KEY-----\n'," +
                                                       "'client_email':'menootest-9fa5a@appspot.gserviceaccount.com'," +
                                                       "'client_id': '108009182778403042502'," +
                                                       "'auth_uri':'https://accounts.google.com/o/oauth2/auth'," +
                                                       "'token_uri':'https://oauth2.googleapis.com/token'," +
                                                       "'auth_provider_x509_cert_url': 'https://www.googleapis.com/oauth2/v1/certs'," +
                                                       "'client_x509_cert_url':'https://www.googleapis.com/robot/v1/metadata/x509/menootest-9fa5a%40appspot.gserviceaccount.com'}";
            var credential = GoogleCredential.FromJson(jsonCredentials);
            var channelCredentials = credential.ToChannelCredentials();
            var channel = new Channel(FirestoreClient.DefaultEndpoint.ToString(), channelCredentials);
            var client = FirestoreClient.Create(channel);
            return FirestoreDb.Create("menootest-9fa5a", client);
        }

        /// <summary>
        /// Obtiene la instancia acorde a la configuración vía appSettings.
        /// <para>NOTA: Se recomienda cambiar a false la configuración 'isDebug' a false cuando se publique en producción.</para>
        /// </summary>
        /// <returns>Objeto gestor de la base de datos.</returns>
        private FirestoreDb GetInstanceDb()
        {
            bool isDebug = Convert.ToBoolean(string.Empty + ConfigurationManager.AppSettings["isDebug"]);
            if (isDebug)
            {
                return AccessDatabaseTesting();
            }
            return AccessDatabaseProduction();
        }

        #endregion
    }
}
