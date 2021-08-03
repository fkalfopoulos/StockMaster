using FantasyWealth.Areas.Identity.Data;
using FantasyWealth.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FantasyWealth.Utilities
{
    public class iexTrading
    {
        private readonly IConfiguration _config;
        private readonly ILogger<iexTrading> _logger;
        private readonly Repository _repo;
        private readonly StockMasterDbContext _db;
        public iexTrading(IConfiguration config, ILogger<iexTrading> log)
        {
            _config = config;
            _logger = log;           
        }
        public static string Token()
        {
            return "pk_71c702cab38743c29e5c2f4b5ad5cefc";          
        }       
        public static string marketCap(string symbol)
        {           
            string apiUrl = $"https://cloud.iexapis.com/stable/stock/{symbol}/stats?token={Token()}";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(apiUrl);
                    HttpResponseMessage result = client.GetAsync(apiUrl).GetAwaiter().GetResult();
                    if (result.IsSuccessStatusCode)
                    {
                        var model = result.Content.ReadAsAsync<CompanyVM>().GetAwaiter().GetResult();
                        var marketcap = model.marketcap;
                        return marketcap.ToKMB();
                    }
                    else
                    {
                        return "zero";
                    }
                }
            }
            catch(Exception ex)
            {
                  return "zero";
            }           
        } 

        public static decimal GetPrice(string symbol)
        {
            var apiUrl = $"https://cloud.iexapis.com/stable/stock/{symbol}/price?token={iexTrading.Token()}";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(apiUrl);
                    HttpResponseMessage result = client.GetAsync(apiUrl).GetAwaiter().GetResult();
                    if (result.IsSuccessStatusCode)
                    {
                        var model = result.Content.ReadAsAsync<decimal>().GetAwaiter().GetResult();
                        var price = model;
                        return model;                       
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public static decimal getChange(string symbol)
        {
            var apiUrl = $"https://cloud.iexapis.com/stable/stock/{symbol}/chart/1m?token={iexTrading.Token()}";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(apiUrl);
                    HttpResponseMessage result = client.GetAsync(apiUrl).GetAwaiter().GetResult();
                    if (result.IsSuccessStatusCode)
                    {
                        List<ChartVM> lChart = result.Content.ReadAsAsync<List<ChartVM>>().GetAwaiter().GetResult();
                        List<decimal> changesPerc = lChart.Select(u => u.changePercent).ToList();
                        decimal change = changesPerc.Last();
                        return change;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public static ValuePairs getSymbolPrice(string Symbol, string MonthId)
        {
            try
            {
                var apiUrl = $"https://cloud.iexapis.com/stable/stock/{Symbol}/chart/{MonthId}m?token={iexTrading.Token()}";
                apiUrl = string.Format(apiUrl, Symbol);
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(apiUrl);
                    HttpResponseMessage result = client.GetAsync(apiUrl).GetAwaiter().GetResult();
                    if (result.IsSuccessStatusCode)
                    {
                        List<ChartVM> lChart = result.Content.ReadAsAsync<List<ChartVM>>().GetAwaiter().GetResult();
                        var initialValue = lChart.Select(u => u.close).First();
                        var recentValue = lChart.Select(u => u.close).Last();
                        List<decimal> values = lChart.Select(u => u.changePercent).ToList();

                        var gainOrloss = (recentValue - initialValue) / initialValue * 100;
                        List<string> keys = lChart.Select(u => u.label).ToList();
                        int total = keys.Count;
                        List<string> changes = lChart.Select(u => u.change).ToList();
                        int redCounter = 0;
                        foreach (var c in changes)
                        {
                            if (c.Contains("-"))
                            {
                                redCounter++;
                            }
                        }
                        int greenDays = total - redCounter;
                        List<decimal> percentagesValues = new List<decimal>();
                        List<decimal> positiveValues = new List<decimal>();
                        List<decimal> negativeValues = new List<decimal>();
                        for (int i = 0; i < values.Count; i++)
                        {
                            decimal r = values[i];
                            r = r * 100;
                            percentagesValues.Add(r);
                        }
                        foreach (var c in percentagesValues)
                        {
                            if (c > 0) { positiveValues.Add(c); }
                            else { negativeValues.Add(c); }
                        }

                        List<decimal> newlistValues = Enumerable.Reverse(percentagesValues).Reverse().ToList();
                        List<string> newlistKeys = Enumerable.Reverse(keys).Reverse().ToList();
                        var percentage = greenDays * 100 / total;
                        decimal Positiveaverage = positiveValues.Average();
                        decimal NegativeAverage = negativeValues.Average();

                        Dictionary<string, decimal> dictionary = newlistKeys.Zip(newlistValues, (k, v) => new { Key = k, Value = v })
                         .ToDictionary(x => x.Key, x => x.Value);

                        decimal bestDay = positiveValues.Select(u => u).Max();
                        decimal worstDay = negativeValues.Select(u => u).Min();
                        string Bestdate = dictionary.Where(u => u.Value == bestDay).Select(u => u.Key).Single();
                        string worstDate = dictionary.Where(u => u.Value == worstDay).Select(u => u.Key).Single();

                        ValuePairs valuePairs = new ValuePairs()
                        {
                            GetValues = dictionary,
                            GreenDays = greenDays,
                            RedDays = redCounter,
                            Percentage = percentage,
                            NegativeAv = NegativeAverage,
                            PositiveAv = Positiveaverage,
                            GainOrLoss = gainOrloss,
                            BestDay = bestDay,
                            WorstDay = worstDay,
                            BestDayDate = Bestdate,
                            WorstDayDate = worstDate
                        };
                        return valuePairs;
                    }
                    else
                    {
                        return null;
                    }
                }
            }catch(Exception ex)
            {
                return null;
            }         
        }

        public static string earnings(string symbol)
        {
            string apiUrl = $"https://cloud.iexapis.com/stable/stock/{symbol}/stats?token={Token()}";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(apiUrl);
                    HttpResponseMessage result = client.GetAsync(apiUrl).GetAwaiter().GetResult();
                    if (result.IsSuccessStatusCode)
                    {
                        var model = result.Content.ReadAsAsync<CompanyVM>().GetAwaiter().GetResult();
                        return model.nextEarningsDate.ToString();
                    } 
                        return "zero"; 
                }
            }
            catch (Exception ex)
            {
                return "zero";
            }
        }

        public static LogoVM getSymbolLogo(string symbol)
        {
            var apiUrl = $"https://cloud.iexapis.com/stable/stock/{symbol}/logo?token={Token()}";
            apiUrl = string.Format(apiUrl, symbol);
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.BaseAddress = new Uri(apiUrl);
                HttpResponseMessage result = client.GetAsync(apiUrl).GetAwaiter().GetResult();
                if (result.IsSuccessStatusCode)
                {
                    var LogoUrl = result.Content.ReadAsAsync<LogoVM>().GetAwaiter().GetResult();
                    return LogoUrl;
                }
                else
                {
                    return null;
                }
            }
        }
        public async Task<CompanyVM> GetModel(string url, HttpClient client)
        { 
            using (var response = await client.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<CompanyVM>();
                }
                else
                {
                    Console.WriteLine("Internal server Error");
                }
            }
            return null;
        }

  
        public static AdvancedStatsModel getAdvanced(string symbol)
        {
            var apiUrl = $"https://cloud.iexapis.com/stable/stock/{symbol}/advanced-stats?token={Token()}";
            JsonAdvancedModel company = new JsonAdvancedModel();
            try
            {
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.BaseAddress = new Uri(apiUrl);
                        HttpResponseMessage result = client.GetAsync(apiUrl).GetAwaiter().GetResult();
                        if (result.IsSuccessStatusCode)
                        {
                            JsonAdvancedModel data = result.Content.ReadAsAsync<JsonAdvancedModel>().GetAwaiter().GetResult();
                            company = data;
                        }
                    }

                    AdvancedStatsModel model = new AdvancedStatsModel();
                    model.totalCash = company.totalCash.ToKMB();
                    model.currentDebt = company.currentDebt.ToKMB();
                    model.revenue = company.revenue.ToKMB();
                    model.grossProfit = company.grossProfit.ToKMB();
                    model.grossProfit = company.grossProfit.ToKMB();
                    model.totalRevenue = company.totalRevenue.ToKMB();
                    model.EBITDA = company.EBITDA.ToKMB();
                    model.revenuePerShare = company.revenuePerShare;
                    model.priceToSales = company.priceToSales;
                    model.forwardPERatio = company.forwardPERatio;
                    model.week52highDate = company.week52highDate;
                    model.week52lowDate = company.week52lowDate;
                    model.forwardPERatio = company.forwardPERatio;
                    model.pegRatio = company.pegRatio;
                    model.peHigh = company.peHigh;
                    model.peLow = company.peLow;
                    return model;
                } 
            }
            catch (Exception ex)
            {
                AdvancedStatsModel model = new AdvancedStatsModel();
                return model;
            }
        }

        public  static async Task<WallViewModel> getSymbolCompany(string symbol)
        {
            CompanyVM company = new CompanyVM();
         

            var apiUrl = $"https://cloud.iexapis.com/stable/stock/{symbol}/company?token={Token()}";
            var apiUrl2 = $"https://cloud.iexapis.com/stable/stock/{symbol}/stats?token={Token()}";
            var logoUrl = $"https://cloud.iexapis.com/stable/stock/{symbol}/logo?token={Token()}";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(apiUrl);
                    HttpResponseMessage result = client.GetAsync(apiUrl).GetAwaiter().GetResult();
                    if (result.IsSuccessStatusCode)
                    {
                        CompanyVM data = result.Content.ReadAsAsync<CompanyVM>().GetAwaiter().GetResult();
                        company = data;
                    }
                }
                using (HttpClient client2 = new HttpClient())
                {
                    client2.BaseAddress = new Uri(logoUrl);
                    HttpResponseMessage result2 = client2.GetAsync(logoUrl).GetAwaiter().GetResult();
                    if (result2.IsSuccessStatusCode)
                    {
                        LogoVM data = result2.Content.ReadAsAsync<LogoVM>().GetAwaiter().GetResult();
                        company.logo = data.url;
                    }
                }
                using (HttpClient client3 = new HttpClient())
                {
                    HttpResponseMessage result3 = client3.GetAsync(apiUrl2).GetAwaiter().GetResult();
                    if (result3.IsSuccessStatusCode)
                    {
                        CompanyVM data = result3.Content.ReadAsAsync<CompanyVM>().GetAwaiter().GetResult();
                        company.peRatio = data.peRatio;
                        company.avg10Volume = data.avg10Volume;
                        company.avg30Volume = data.avg30Volume;
                        if (data.peRatio != 0)
                        {
                            company.peRatio = data.peRatio;
                        }
                        if (data.nextEarningsDate != null)
                        {
                            company.nextEarningsDate = data.nextEarningsDate;
                        }
                    }
                }
                List<Company> CompaniesToAdd = new List<Company>();
                 
                WallViewModel model = new WallViewModel();
                model.avg10Volume = company.avg10Volume.ToKMB();
                model.avg30Volume = company.avg30Volume.ToKMB();
                model.logo = company.logo;
                model.companyName = company.companyName; 
                model.CEO = company.CEO.WikiCreator(); 
                model.website = company.website;
                model.city = company.city;
                model.country = company.country;
                model.nextEarningsDate = company.nextEarningsDate;
                model.peRatio = company.peRatio;
                model.symbol = company.symbol;
                model.Tags = company.tags;
                model.fundNames = getFunds(symbol);

                return model;
            }
            catch (Exception ex)
            {
                return null;
            }           
        }
        public static List<ChartVM> getSymbolChart(string symbol)
        {
            var apiUrl = $"https://cloud.iexapis.com/stable/stock/{symbol}/chart/3m?token={Token()}"; 
            apiUrl = string.Format(apiUrl, symbol);
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.BaseAddress = new Uri(apiUrl);
                HttpResponseMessage result = client.GetAsync(apiUrl).GetAwaiter().GetResult();
                if (result.IsSuccessStatusCode)
                {
                    List<ChartVM> lChart = result.Content.ReadAsAsync<List<ChartVM>>().GetAwaiter().GetResult(); 
                    return lChart;
                }
                else
                {
                    return null;
                }
            }
        }
        public static async Task<EconomicDataModel> getFinancialAsync(string symbol)
        {
            var apiUrl = $"https://cloud.iexapis.com/stable/stock/{symbol}/balance-sheet?token={Token()}";

            EconomicData company = new EconomicData();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(apiUrl);
                    HttpResponseMessage result = client.GetAsync(apiUrl).GetAwaiter().GetResult();
                    if (result.IsSuccessStatusCode)
                    {
                        var data = result.Content.ReadAsAsync<EconomicData>().GetAwaiter().GetResult();
                        company = data;
                    }
                    EconomicDataModel model = new EconomicDataModel();

                    if(company.balancesheet != null)
                    {
                        foreach (var s in company.balancesheet)
                        {
                            model.currentAssets = s.currentAssets.ToKMB();
                            model.currentCash = s.currentCash.ToKMB();
                            model.inventory = s.inventory.ToKMB();
                            model.longTermDebt = s.longTermDebt.ToKMB();
                            model.longTermInvestments = s.longTermInvestments.ToKMB();
                            model.minorityInterest = s.minorityInterest.ToKMB();
                            model.receivables = s.receivables.ToKMB();
                            model.totalAssets = s.totalAssets.ToKMB();
                            model.reportDate = s.reportDate;
                        }

                        model.Peers = await getPeers(symbol);
                        return model;
                    }
                    model.Peers = await getPeers(symbol);
                    return model;
                }
            }
            catch (Exception ex)
            {
                EconomicDataModel model = new EconomicDataModel();
                model.currentAssets = "zero";
                model.currentCash = "zero";
                model.inventory = "zero";
                model.longTermDebt = "zero";
                model.longTermInvestments = "zero";
                model.minorityInterest = "zero";
                model.receivables = "zero";
                model.totalAssets = "zero";
                model.reportDate = "zero";
                model.Peers = await getPeers(symbol);
                return model;
            }
        } 

        public async static Task<List<string>> getPeers(string symbol)
        {
            var apiUrl = $"https://cloud.iexapis.com/stable/stock/{symbol}/peers?token={Token()}";
            try
            {
                string s = string.Empty;
                using (var client = new HttpClient())
                {
                    var response = client.GetAsync(apiUrl).Result;
                    string str = await response.Content.ReadAsStringAsync();
                    s = str;
                }
                s = Regex.Replace(s, "[^a-zA-Z0-9 -]", " ");
                string[] zlist = s.Split();
                List<string> Final = new List<string>();
                foreach (string val in zlist.Where(i => !string.IsNullOrEmpty(i)))
                {
                    Final.Add(val);
                }
                return Final;
            }
            catch (Exception ex)
            {
                List<string> Final = new List<string>();
                Final.Add("No data provided");
                return Final;
            }
        }

        public   static List<FundName> getFunds(string symbol)
        {
            var apiFunds = $"https://cloud.iexapis.com/stable/stock/{symbol}/fund-ownership?token={Token()}";           
            try
            {
                InsiderVM[] md = new InsiderVM[10];
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(apiFunds);
                    HttpResponseMessage result = client.GetAsync(apiFunds).GetAwaiter().GetResult();
                    {
                        var response = client.GetAsync(apiFunds).Result;
                        InsiderVM[] data = result.Content.ReadAsAsync<InsiderVM[]>().GetAwaiter().GetResult();
                        md = data;
                    }
                    
                    List<string> fundNames = new List<string>();
                    var list = md.Select(u => u.entityProperName).ToList();                  
                    //fundNames.ForEach(f => f.Remove(f.Length - 1, 1));
                    List<FundName> funds = new List<FundName>(); 
                    for (int i = 0; i < list.Count; i++)
                    {
                        FundName fund = new FundName();
                        fund.Name = list[i];
                        fund.GoogleUrl = GetGoogle(list[i]);
                        funds.Add(fund);
                    }
                    return funds;                  
                }
            }
            catch (Exception ex)
            {
                List<FundName> funds = new List<FundName>();                
                {
                    FundName fund = new FundName();
                    fund.Name = "no data provided";                   
                    funds.Add(fund);
                }
                return funds;
            }
        }

        public static InsiderVM getInsider(string symbol)
        {
            InsiderVM company = new InsiderVM();
            var apiUrl = $"https://cloud.iexapis.com/stable/stock/{symbol}/insider-summary?token={Token()}"; 
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(apiUrl);
                    HttpResponseMessage result = client.GetAsync(apiUrl).GetAwaiter().GetResult();
                    if (result.IsSuccessStatusCode)
                    {
                        InsiderVM data = result.Content.ReadAsAsync<InsiderVM>().GetAwaiter().GetResult();
                        company = data;
                    }
                }
                return company;
            }
            catch (Exception ex)
            {
                return null;
            }
        }          

        public async Task<List<Crypto>> GetCrypto()
        {
            return null;
        }


    public static async Task<List<SymbolVM>> getTickerSymbolListAsync()
        {
            List<SymbolVM> lTickerSymbol = new List<SymbolVM>();
            var apiUrl = "https://api.iextrading.com/1.0/ref-data/symbols";
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.BaseAddress = new Uri(apiUrl);
                HttpResponseMessage result = await client.GetAsync(apiUrl);
                if (result.IsSuccessStatusCode)
                {
                    lTickerSymbol = await result.Content.ReadAsAsync<List<SymbolVM>>();
                    return lTickerSymbol;
                }
                else
                {
                    return null;
                }
            }
        } 
        public static string GetGoogle(string searchterm)
        {
            var l = searchterm.Split();
            string final = $"https://www.google.com/search?q=";
            for (int i = 0; i < l.Length; i++)
            {
                final = final + $"{l[i]}+";
            }
            return final;
        }
    }
    public static class Extentions
    {
        public static string ToKMB(this decimal num)
        {
            if (num > 999999999 || num < -999999999)
            {
                return num.ToString("0,,,.###B", CultureInfo.InvariantCulture);
            }
            else
            if (num > 999999 || num < -999999)
            {
                return num.ToString("0,,.##M", CultureInfo.InvariantCulture);
            }
            else
            if (num > 999 || num < -999)
            {
                return num.ToString("0,.#K", CultureInfo.InvariantCulture);
            }
            else
            {
                return num.ToString(CultureInfo.InvariantCulture);
            }
        }

        public static string RemoveLast(this string text, string character)
        {
            if (text.Length < 1) return text;
            return text.Remove(text.ToString().LastIndexOf(character), character.Length);
        }

        public static string WikiCreator(this string name)
        {
            if(name == null || name=="")
            {
                return "unknown";
            }
           var names = name.Split();             
           return   names[0] + "_" + names[1];             
        }
    }    
}