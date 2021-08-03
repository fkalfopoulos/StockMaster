using System;
using System.Collections.Generic;

namespace FantasyWealth.Models
{
    public class CompanyVM
    {
        public string symbol{get;set;}
        public string logo{get;set;}
        public string companyName {get;set;}
        public string exchange{get;set;}
        public string industry{get;set;}
        public string website{get;set;}
        public string description{get;set;}
        public string CEO{get;set;}
        public string issueType{get;set;}
        public string sector{get;set;}
        public string city {get;set;}
        public string state{get;set;}
        public string country{get;set;}
        public decimal marketcap {get;set;}
        public decimal avg10Volume { get;set;}
        public decimal avg30Volume { get;set;}
        public string yearprice_High {get;set;}
        public string yearprice_Low {get;set;}
        public string nextDividendDate { get;set;}
        public string dividendYield { get;set;}
        public string exDividendDate { get;set;}
        public string day200MovingAvg { get;set;}
        public string day50MovingAvg { get;set;}
        public decimal peRatio { get;set;}     
        public decimal price { get;set;}     
        public decimal change { get;set;}     
        public string divident {get;set;}       
        public string nextEarningsDate { get; set; }        

        public List<string> tags { get; set; }

    }

    public class WallViewModel
    {
        public string symbol { get; set; }
        public string logo { get; set; }
        public string companyName { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string day200MovingAvg { get; set; }
        public string day50MovingAvg { get; set; }
        public decimal peRatio { get; set; }      
        public string nextEarningsDate { get; set; }
        public string website { get; set; }       
        public string CEO { get; set; }

        public string marketCap { get; set; }
        public string avg10Volume { get; set; }
        public string avg30Volume { get; set; }
        public List<string> Tags { get; set; }       
        public string Tag { get; set; }       
        public List<FundName> fundNames { get; set; }

        public List<Company> Positive_Strike { get; set; }
        public List<Company> Negative_Strike { get; set; }
    }
 }