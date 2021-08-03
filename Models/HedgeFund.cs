using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FantasyWealth.Models
{
    public class HedgeFund
    {

        public string Id { get; set; }
        public string Name { get; set; }
        public List<Company> Companies { get; set; }
        public List<HedgeFund_Companies> HedgeFund_Companies { get; set; }
    }
}
