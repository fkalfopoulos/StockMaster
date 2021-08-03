using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using FantasyWealth.Models;
using FantasyWealth.Areas.Identity.Data;

namespace FantasyWealth.Models
{
   

    public class WatchList_Companies
    {
        public string Id { get; set; }
        public Company Company { get; set; }
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public WatchList WatchList { get; set; }
        public string WatchListId { get; set; }
        public string WatchListName { get; set; }
        public User User { get; set; }
    }

    public class WatchList
    {
        public WatchList()
        {
            this.UpdatedDate = DateTime.Now;
        }
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
       
        public DateTime CreationDate { get; set; }
        public DateTime UpdatedDate { get; set; }       
        public User User { get; set; }
        public List<WatchList_Companies> Companies { get; set; }
    }
}