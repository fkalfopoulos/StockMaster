using System;
using System.ComponentModel.DataAnnotations;
using FantasyWealth.Areas.Identity.Data;
using System.Collections.Generic;

namespace FantasyWealth.Models
{
    public class Trade
    {
        public Trade()
        {
            this.CreationDate = DateTime.Now;
        }
        [Key]
        public int Id { get; set; }
        [StringLength(450)]
        public string UserId { get; set; }
        [Required]
        [Display(Name="Ticker Symbol")]
        public int SymbolId { get; set; }
        [Display(Name="Share")]
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public TradeAction Action { get; set; }
        [Display(Name="Date")]
        public DateTime CreationDate { get; set; }
        public string Comment { get; set; }
        public TradeStatus Status { get; set; }
        public string Reserved { get; set; }
        //relationships  between the following entities
        public User User { get; set; }
        public List<Transaction> Transactions { get; set; }
         public WatchList Symbol  { get; set; }

    }
}