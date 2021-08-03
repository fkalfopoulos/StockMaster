using System;
using System.Collections.Generic;
using FantasyWealth.Models;
using Microsoft.AspNetCore.Mvc;
using FantasyWealth.Utilities;
using FantasyWealth.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Web;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace FantasyWealth.Controllers
{
    public class OpenAPIController : Controller
    {
        private readonly StockMasterDbContext _context;
        private readonly Repository _repo;
        private readonly UserManager<User> _userManager;
        public OpenAPIController(StockMasterDbContext context, Repository repo, UserManager<User> m)
        {
            _context = context;
            _repo = repo;
            _userManager = m;
        }
         
        [HttpGet]
        public IActionResult getPrice(PriceModel model)
        {            
            ValuePairs TosendModel = iexTrading.getSymbolPrice(model.Symbol, model.MonthId);
            List<Company> earningDayCompanies = CheckEarnings();
            if(earningDayCompanies != null)
            {
                TosendModel.EarningCompanies = earningDayCompanies;
            }           
            return PartialView(TosendModel);
        }

        [HttpGet]
        public IActionResult marketCap(string SearchSymbol)
        {
            //ViewData["marketCap"] = iexTrading.marketCap(SearchSymbol);
            List<Company> Companies = FindStrike();
            WallViewModel model = new WallViewModel();
                model.Negative_Strike = Companies;
            return PartialView("getCap", model);
        }
        public static bool Exists(int[] ints, int k)
        {
            int index = Array.BinarySearch(ints, k);
            if (index > -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

            [HttpPost]
        public async Task<IActionResult> WatchListed(string symbol)
        {
            User user = _repo.GetUser();
            try
            {
                Company company = _repo.FindCompany(symbol);                
                WatchList watchlist = _repo.FindWatchList(user);      //change this
                 
                if (company != null && !_repo.isWatchListed(company, watchlist))
                {
                    WatchList_Companies item = new WatchList_Companies()
                    {
                        Company = company,
                        CompanyName = company.CompanyName,
                        WatchList = watchlist,
                        WatchListName = watchlist.Name,
                        User = user
                    };
                    _repo.Add(item);
                    await _repo.Commit();
                }
                else
                {
                   await _repo.Create_Company(symbol);
                    _repo.Add(company);
                    await _repo.Commit();
                }
                return Ok();
          }
            catch(Exception ex)
            {
                return Ok($"problem with{ex}");
            }           
        } 
        public IActionResult getChart(string SearchSymbol)
        {
            return PartialView("getChart");
        }

        public async Task<IActionResult> getInfo(string SearchSymbol)
        {
            CompanyVM company = new CompanyVM();

            var apiUrl = $"https://cloud.iexapis.com/stable/stock/{SearchSymbol}/company?token={iexTrading.Token()}"; 
            var apiUrl2 = $"https://cloud.iexapis.com/stable/stock/{SearchSymbol}/stats?token={iexTrading.Token()}";
            var logoUrl = $"https://cloud.iexapis.com/stable/stock/{SearchSymbol}/logo?token={iexTrading.Token()}";
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
                        company.marketcap = data.marketcap;
                    }
                }
                List<Company> CompaniesToAdd = new List<Company>();
                //List<string> Peerslist = await getPeers(SearchSymbol);
                //if (Peerslist != null)
                //{
                //    foreach (var c in Peerslist)
                //    {
                //        if (!_repo.CompanyExists(c))
                //        {
                //           string earnings = _repo.getEarnings(c.ToLower());
                //           await _repo.Create_Company(c.ToLower());
                //           await _repo.Commit();
                //           await _repo.SaveEarnings(earnings, c.ToLower());                        
                //        }
                //    }
                //}
                Company CompanyForDb = _context.Companies.Where(u => u.Symbol == SearchSymbol).SingleOrDefault();
                if (CompanyForDb == null)
                {
                    await _repo.Create_Company(SearchSymbol);
                    await _repo.Commit();
                    string earnings = _repo.getEarnings(SearchSymbol);
                    await  _repo.SaveEarnings(earnings, SearchSymbol);                   
                }
                WallViewModel model = new WallViewModel();
                model.avg10Volume = company.avg10Volume.ToKMB();
                model.avg30Volume = company.avg30Volume.ToKMB();
                model.logo = company.logo;
                model.companyName = company.companyName;
                model.CEO = company.CEO.WikiCreator();
                model.website = company.website;
                model.marketCap = company.marketcap.ToKMB();
                model.city = company.city;
                model.country = company.country;
                model.nextEarningsDate = _repo.getEarnings(SearchSymbol);
                model.peRatio = company.peRatio;
                model.symbol = company.symbol;
                if(company.tags.Count() == 0)
                {
                    model.Tag = "No data provided";
                } else { model.Tag = company.tags.Last(); } 
                //foreach (var c in iexTrading.getFunds(SearchSymbol))
                //{
                //    if (!_repo.FundExists(c.Name))
                //    {
                //        HedgeFund fund = new HedgeFund()
                //        {
                //            Name = c.Name
                //        };
                //        _context.Funds.Add(fund);
                //        await _context.SaveChangesAsync();

                //        HedgeFund_Companies item = new HedgeFund_Companies()
                //        {
                //            Company = com,
                //            CompanyId = com.Id,
                //            CompanyName = com.Symbol,
                //            Fund = fund,
                //            FundId = fund.Id,
                //            FundName = fund.Name
                //        };
                //        _context.Hedge_Companies.Add(item);
                //        await _context.SaveChangesAsync();
                //    }
                //}
                return PartialView("getInfo", model);
            }
            catch (Exception ex)
            {
                WallViewModel model = new WallViewModel();
                return PartialView("getInfo", model);
            }
        }      
         

        public void GetFunds(string SearchSymbol)
        {
            var im = iexTrading.getFunds(SearchSymbol);
        }

        public JsonResult getChartDataAsJson(string SearchSymbol)
        {
            List<ChartVM> lChart = new List<ChartVM>();
            lChart = iexTrading.getSymbolChart(SearchSymbol);
            return Json(lChart);
        }
        public IActionResult getAdvanced(string SearchSymbol)
        {
            AdvancedStatsModel coVM = new AdvancedStatsModel();
            coVM = iexTrading.getAdvanced(SearchSymbol);
            return PartialView("getAdvanced", coVM);
        }

        public async Task<IActionResult> getFin(string SearchSymbol)
        {
            EconomicDataModel coVM = new EconomicDataModel();
            coVM = await iexTrading.getFinancialAsync(SearchSymbol);
            return PartialView("getFin", coVM);
        }       
        public async  Task<List<string>> getPeers(string symbol)
        {
            var apiUrl = $"https://cloud.iexapis.com/stable/stock/{symbol}/peers?token={iexTrading.Token()}";
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
                Final.Add("no data provided");
                return Final;
            }
        }
        public  List<Company> FindStrike()
        {
            List<Company> tickers = _context.Companies.ToList();
            List<Company> positiveCompanies = new List<Company>();
            List<Company> negativeCompanies = new List<Company>();
           
            try
            {
                foreach (var company in tickers)
                {
                    var apiUrl = $"https://cloud.iexapis.com/stable/stock/{company.Symbol}/chart/1m?token={iexTrading.Token()}";
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
                            List<string> changes = lChart.Select(u => u.change).ToList();

                            string previous = string.Empty;
                            int counterNegative = 0;
                            int counterPositive = 0;

                            foreach (var c in changes)
                            {
                                if (c.Contains("-"))
                                {
                                    counterNegative++;
                                }
                                else
                                {
                                    counterNegative = 0;
                                }
                            }
                            if (counterNegative >= 3)
                            {
                                negativeCompanies.Add(company);
                            }
                        }
                    }
                }
                return negativeCompanies.ToList();
            }
            catch(Exception ex)
            {
                return null;
            } 
        }        

        public List<Company> CheckEarnings()
        {
            try
            {
                List<Company> ToSend = new List<Company>();
                string daytme = DateTime.Now.ToString("yyyy-MM-dd");
                DateTime time = DateTime.ParseExact(daytme, "yyyy-MM-dd", CultureInfo.InvariantCulture); ;
                int month = DateTime.Now.Month;
                Company company = new Company();
                if (month <= 3)
                {
                    List<Company> companies = _context.Companies.Where(u => u.FirstQ == time).ToList();
                    ToSend = companies;
                }
                if (month >= 3 && month <= 6)
                {
                    List<Company> companies = _context.Companies.Where(u => u.SecondQ == time).ToList();
                    ToSend = companies;
                };
                if (month >= 6 && month <= 9)
                {
                    List<Company> companies = _context.Companies.Where(u => u.ThirdQ == time).ToList();
                    ToSend = companies;
                }
                if (month >= 9)
                {
                    List<Company> companies = _context.Companies.Where(u => u.FourthQ == time).ToList();
                    ToSend = companies;
                }
                return ToSend;
            }
            catch(Exception ex)
            {
               
                return null;
            }
        }
    }
}