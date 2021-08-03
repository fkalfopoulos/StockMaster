using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FantasyWealth.Models
{
    public class Company
    {
        
        public string Id { get; set; }
        public string CompanyName { get; set; }     
        public string Symbol { get; set; }
        public string website { get; set; }
        public string description { get; set; }
        public string Industry { get; set; }
        public string CEO { get; set; }
        public decimal PeRatio { get; set; }

        public DateTime FirstQ { get; set; }
        public DateTime SecondQ { get; set; }
        public DateTime ThirdQ { get; set; }
        public DateTime FourthQ { get; set; }


     
        public List<HedgeFund_Companies>HedgeFund_Companies { get; set; }
    }
}
