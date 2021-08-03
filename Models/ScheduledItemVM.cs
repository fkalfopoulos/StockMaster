using System;
using System.ComponentModel.DataAnnotations;

namespace FantasyWealth.Models
{
    public class ScheduledItemVM
    {
        public int SymbolId { get; set; }
        [StringLength(15)]
        public string Symbol { get; set; }
        public decimal CurrentPrice{get;set;}
    }
}