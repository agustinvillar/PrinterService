﻿using System;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema.Entities
{
    public class PrinterEvents
    {
        public Guid Id { get; set; }

        public string DisplayName { get; set; }

        public string Value { get; set; }
    }
}
