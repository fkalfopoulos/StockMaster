using System;
using System.ComponentModel.DataAnnotations;
using FantasyWealth.Areas.Identity.Data;

namespace FantasyWealth.Models
{
    public class Transaction
    {
         public Transaction()
        {
            this.CreationDate = DateTime.Now;
        }
        [Key]
        public int Id { get; set; }
        [StringLength(450)]
        public string UserId { get; set; }
        [Display(Name="Trade Id")]
        public int? TradeId { get; set; }
        [Display(Name="Transaction Type")]
        public TransactionType TransactionType { get; set; }
        [Display(Name="From")]
        public Account FromAccount{get;set;}
        [Display(Name="To")]
        public Account ToAccount{get;set;}
        [Display(Name="Total Amount")]
        public decimal TotalAmount { get; set; }
        public string Comment { get; set; }
        [Display(Name="Date")]
        public DateTime CreationDate { get; set; }
        public bool Reconciled {get;set;}

        //relationships  between the following entities
        public User User { get; set; }
        public Trade Trade { get; set; }
    }
}