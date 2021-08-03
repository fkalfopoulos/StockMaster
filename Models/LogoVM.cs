using System;
using System.Collections;
using System.Collections.Generic;

namespace FantasyWealth.Models
{
    public class LogoVM
    {
        public string url{get;set;}
    }

    public class ValuePairs
    {
       public IDictionary<string,decimal> GetValues { get; set; }
        public int GreenDays { get; set; }
        public int RedDays { get; set; }
        public decimal Percentage { get; set; }
        public decimal NegativeAv { get; set; }
        public decimal PositiveAv { get; set; }        
        public string CompanyDesc { get; set; }
        public decimal GainOrLoss { get; set; }
        public decimal BestDay { get; set; }
        public string BestDayDate { get; set; }
        public string WorstDayDate { get; set; }
        public decimal WorstDay { get; set; }

        public List<Company>EarningCompanies { get; set; }
    }

    public class PriceModel
    {
        public string Symbol { get; set; }
        public string MonthId { get; set; }
        public decimal bestDayPrice { get; set; }
        public string bestDayDate { get; set; }
    }

    public class BestModel
    {
        public decimal bestDayPrice { get; set; }
        public string bestDayDate { get; set; }
    }
}