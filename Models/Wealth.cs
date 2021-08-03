using System;
using System.ComponentModel.DataAnnotations;
using FantasyWealth.Areas.Identity.Data;

namespace FantasyWealth.Models
{
    public class Wealth
    {
         public Wealth()
        {
            this.UpdatedDate = DateTime.Now;
        }
        [Key]
        public int Id { get; set; }
        [StringLength(450)]
        public string UserId { get; set; }
        [Required]
        public int SymbolId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        //relationships  between the following entities
        public User User { get; set; }
        public WatchList Symbol  { get; set; }
    }
}