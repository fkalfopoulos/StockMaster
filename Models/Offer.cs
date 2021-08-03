using System;
using System.ComponentModel.DataAnnotations;
using FantasyWealth.Areas.Identity.Data;

namespace FantasyWealth.Models
{
    public class Offer
    {
        public Offer()
        {
            this.CreationDate = DateTime.Now;
        }
        [Key]
        public int Id { get; set; }
        [StringLength(450)]
        public string UserId { get; set; }
        [Required]
        public int SymbolId { get; set; }
        [Display(Name = "Price")]
        public decimal CurrentPrice { get; set; }
        [Display(Name = "Counteroffer")]
        public decimal OfferPrice { get; set; }
        public TradeAction Action { get; set; }
        [Display(Name = "Expiration")]
        public DateTime ExpirationDate { get; set; }
        [Display(Name = "Date")]
        public DateTime CreationDate { get; set; }
        [Display(Name = "Update")]
        public DateTime UpdatedDate { get; set; }
        public bool Expired { get; set; }


        //relationships  between the following entities
        public User User { get; set; }

        public WatchList WatchList {get; set;}
    }
}