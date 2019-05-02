using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoachApp.Models
{
    public class Test
    {
        public string Id { get; set; }
        public TestType Type { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Date { get; set; }
        public ICollection<Participant> Participants { get; set; }
    }
}
