using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FantasyWealth.Models
{
    public class HedgeFund_Companies
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [ForeignKey("CompanyId")]
        public Company Company { get; set; }
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }

        [ForeignKey("HedgeFundId")]
        public HedgeFund Fund { get; set; }
        public string FundId { get; set; }
        public string FundName { get; set; }



    }
}
