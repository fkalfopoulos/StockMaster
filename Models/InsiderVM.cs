using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FantasyWealth.Models
{
    public class InsiderVM
    {

        public string fullName { get; set; }
        public int totalBought { get; set; }
        public int totalSold { get; set; }
        public string source { get; set; }
        public string entityProperName { get; set; }
        public int adjHolding { get; set; }
        
       
    }
}
