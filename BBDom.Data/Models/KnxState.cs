using System;
using System.ComponentModel.DataAnnotations;

namespace BBDom.Data.Models
{
    public class KnxState
    {
        [Key]
        public long UTCTicks { get; set; }
        public DateTime UTCTimestamp { get; set; }
        
        public string Address { get; set; }
        public double? State { get; set; }
        public StateSource StateSource { get; set; }
    }

    public enum StateSource
    {
        EVENT = 0,
        STATUS = 1,
    }
}
