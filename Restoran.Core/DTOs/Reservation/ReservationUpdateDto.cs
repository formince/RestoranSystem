﻿using Restoran.Core.Statics.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restoran.Core.DTOs.Reservation
{
    public class ReservationUpdateDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int NumberOfGuests { get; set; }
        public ReservationStatus Status { get; set; } 
        public int TableId { get; set; }
    }
}
