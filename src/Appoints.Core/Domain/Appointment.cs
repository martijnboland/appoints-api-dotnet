﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Appoints.Core.Domain
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public DateTime StartDateAndTime { get; set; }

        [Required]
        public DateTime EndDateAndTime { get; set; }

        public int Duration
        {
            get { return (int) (EndDateAndTime - StartDateAndTime).TotalMinutes; }
        }

        public string Remarks { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}