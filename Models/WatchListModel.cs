using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FantasyWealth.Models
{
    public class WatchListModel
    {
        
            public List<WatchList> MyWatchLists { get; set; }
            public List<Company> My_Companies { get; set; }

            public List <CompanyPriceChange> My_Commies { get; set; }
        
    }

    public class CompanyPriceChange
    {
        public Company Company { get; set; }
        public decimal Price { get; set; }
        public decimal Change { get; set; }
    }
}
