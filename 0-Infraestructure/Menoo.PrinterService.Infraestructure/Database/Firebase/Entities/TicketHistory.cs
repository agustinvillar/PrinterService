﻿using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Menoo.PrinterService.Infraestructure.Database.Firebase.Entities
{
    [FirestoreData]
    public class TicketHistory
    {
        [FirestoreProperty("printEvent")]
        [JsonProperty("printEvent")]
        public string PrintEvent { get; set; }

        [FirestoreProperty("dayCreatedAt")]
        [JsonProperty("dayCreatedAt")]
        public string DayCreatedAt { get; set; }

        [FirestoreProperty("createdAt")]
        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [FirestoreProperty("entityId")]
        [JsonProperty("entityId")]
        public List<string> EntityId { get; set; }

        [FirestoreProperty("ticketImage")]
        [JsonProperty("ticketImage")]
        public string TicketImage { get; set; }
    }
}
