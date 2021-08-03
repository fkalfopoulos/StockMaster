using System;
using System.Collections.Generic;

namespace FantasyWealth.Models
{
    public class DashboardVM
    {
        public IEnumerable<Wealth> Wealths { get; set; }
        public IEnumerable<Trade> Trades { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }
    }
}