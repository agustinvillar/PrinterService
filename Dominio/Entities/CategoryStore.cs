﻿using Google.Cloud.Firestore;

namespace Menoo.PrinterService.Business
{
    [FirestoreData]
    public class CategoryStore
    {
        [FirestoreProperty("id")]
        public string Id { get; set; }

        [FirestoreProperty("name")]
        public string Name { get; set; }
    }
}
