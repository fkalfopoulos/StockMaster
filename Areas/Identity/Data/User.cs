using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using FantasyWealth.Models;

namespace FantasyWealth.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the FantasyWealthUser class
    public class User : IdentityUser
    {
        public User()
        {
            this.RegistrationDate = DateTime.Now;
        }
        //Adding custom user data
        [Required]
        [StringLength(150)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(150)]
        public string LastName { get; set; }
        public DateTime RegistrationDate { get; set; }
        public decimal CashBalanceAmount { get; set; }
        //relationships  between the following entities
        public List<Transaction> Transactions { get; set; }
        public List<Trade> Trades { get; set; }
        public List<Wealth> Wealths { get; set; }
        public List<Offer> Offers { get; set; }
        public List<WatchList> WatchLists { get; set; }
    }
}
