using System;
using System.Collections.Generic;

namespace FantasyWealth.Models
{
    public class ChartVM
    {
        public string date { get; set; }
        public decimal open { get; set; }
        public decimal high { get; set; }
        public decimal low { get; set; }
        public decimal close { get; set; }
        public int volume { get; set; }
        public decimal unadjustedClose { get; set; }
        public int unadjustedVolume { get; set; }
        public string change { get; set; }
        public decimal changePercent { get; set; }
        public decimal vwap { get; set; }
        public string label { get; set; }
        public decimal changeOverTime { get; set; }
    }

    public class EarningsInfo
    {
        public string Earnings { get; set; }
        public string CEO { get; set; }
        public string Website { get; set; }
        public string Description { get; set; }
        public decimal PeRatio { get; set; }
        public string Industry { get; set; }
    }

    public class CompanyInfo
    {
        public string Earnings { get; set; }
        public string CEO { get; set; }
        public string Website { get; set; }
        public string Description { get; set; }
        public decimal PeRatio { get; set; }
         
    }
}