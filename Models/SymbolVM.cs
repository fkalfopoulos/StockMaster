using System;

namespace FantasyWealth.Models
{
    public class SymbolVM
    {
        public string symbol{get;set;}
        public string name{get;set;}
        public string date{get;set;}
        public string isEnabled{get;set;}
    }

    public class FundName
    {
        public string Name { get; set; }
        public string GoogleUrl { get; set; }
    }

    public class Crypto
    {
        public string symbol { get; set; }
        public string price { get; set; }
        public string date { get; set; }
        public string isEnabled { get; set; }
    }

}