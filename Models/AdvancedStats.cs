using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FantasyWealth.Models
{
    public class AdvancedStatsModel
    {
        public string totalCash { get; set; }
        public string revenue { get; set; }
        public string currentDebt { get; set; }
        public string enterpriseValue { get; set; }
        public decimal revenuePerShare { get; set; }
        
        public decimal enterpriseValueToRevenue { get; set; }       
        public decimal debtToEquity { get; set; }
        public decimal profitMargin { get; set; }
        public decimal priceToSales { get; set; }
        public decimal priceToBook { get; set; }
        public decimal? pegRatio { get; set; }
        public decimal? forwardPERatio { get; set; }
        public decimal? peHigh { get; set; }
        public decimal? peLow { get; set; } 
        public string grossProfit { get; set; }
        public string totalRevenue { get; set; }
        public string week52lowDate { get; set; }
        public string week52highDate { get; set; }
        public string EBITDA { get; set; }   //EBITDA – Earnings Before Interest, Taxes, Depreciation, and Amortization 

    }

    public class JsonAdvancedModel
    {
        public decimal totalCash { get; set; }
        public decimal revenue { get; set; }
        public decimal currentDebt { get; set; }
        public decimal enterpriseValue { get; set; }
        public decimal revenuePerShare { get; set; }

        public decimal enterpriseValueToRevenue { get; set; }
        public decimal debtToEquity { get; set; }
        public decimal profitMargin { get; set; }
        public decimal priceToSales { get; set; }
        public decimal priceToBook { get; set; }
        public decimal? forwardPERatio { get; set; }
        public decimal? peHigh { get; set; }
        public decimal? pegRatio { get; set; }
        public decimal? peLow { get; set; }
        public decimal grossProfit { get; set; }
        public decimal totalRevenue { get; set; }
        public string week52lowDate { get; set; }
        public string week52highDate { get; set; }
        public decimal EBITDA { get; set; }   //EBITDA – Earnings Before Interest, Taxes, Depreciation, and Amortization  
    }

    public class EconomicDataModel
    {
        public string currentAssets { get; set; }
        public string totalAssets { get; set; }
        public string currentCash { get; set; }
        public string inventory { get; set; }
        public string longTermDebt { get; set; }
        public string minorityInterest { get; set; }
        public string longTermInvestments { get; set; }
        public string receivables { get; set; }
        public string reportDate { get; set; } 

        public List<string> Peers { get; set; }
    }

    public class EconomicData
    {
        public List<JsonEconomicData> balancesheet { get; set; }
    }
    public class PeerData
        {
            public List<string> Peers { get; set; }
        }

        public class JsonEconomicData
    {
        public decimal currentAssets { get; set; }
        public decimal currentCash { get; set; }
        public decimal totalAssets { get; set; }
        public decimal inventory { get; set; }
        public decimal longTermDebt { get; set; }
        public decimal minorityInterest { get; set; }
        public decimal longTermInvestments { get; set; }
        public decimal receivables { get; set; }
        public string reportDate { get; set; }
    }
}
