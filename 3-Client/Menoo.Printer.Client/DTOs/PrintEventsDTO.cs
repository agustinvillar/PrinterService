﻿using System;

namespace Menoo.PrinterService.Client.DTOs
{
    public class PrintEventsDTO
    {
        public Guid Id { get; set; }

        public string DisplayName { get; set; }

        public string Value { get; set; }
    }
}