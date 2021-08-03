using FantasyWealth.Areas.Identity.Data;
using FantasyWealth.Models;
using Microsoft.AspNetCore.Mvc;
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
    public class Repository
    {
        private readonly StockMasterDbContext _db;
        public Repository(StockMasterDbContext db)
        {
            _db = db;
        }

        public void Add<T>(T Entity) where T : class
        {
            _db.Add(Entity);
        }
        public void Delete<T>(T Entity) where T : class
        {
            _db.Remove(Entity);
        }
        public WatchList FindWatchList(User user)
        {
           return _db.WatchLists.Where(u => u.User.Id == user.Id).SingleOrDefault();
        }
        
        public List<WatchList> GetMyWatchLists(User user)
        {
            return _db.WatchLists.Where(u => u.User.Id == user.Id).ToList();
        }

        public int GetMyW(User user)
        {
            return _db.WatchLists.Where(u => u.User == user).Select(u => u.Id).SingleOrDefault();
        }

        public List<CompanyPriceChange> GetMyWatchList(User user, int WId)
        {
            List<Company> myCompanies = new List<Company>();
            List<CompanyPriceChange> myCompaniesPC = new List<CompanyPriceChange>();
           var Wlist = _db.WatchLists.Where(u => u.User.Id == user.Id && u.Id == WId).SingleOrDefault();
            var coms = _db.WatchList_Companies.Where(u => u.WatchList.Id == Wlist.Id).Select(u => u.CompanyId).ToList();
            foreach(var c in coms)
            {
                Company company = _db.Companies.Where(u => u.Id == c).SingleOrDefault();
                myCompanies.Add(company);
            }
           
            foreach(var c in myCompanies)
            {
                CompanyPriceChange cc = new CompanyPriceChange();
                cc.Company = c;
                cc.Price = iexTrading.GetPrice(c.Symbol);
                cc.Change = iexTrading.getChange(c.Symbol);                
                myCompaniesPC.Add(cc);
            }
            return myCompaniesPC;            
        }
 
        public bool isWatchListed(Company company, WatchList watchlist)
        {
            return _db.WatchList_Companies.Any(u => u.CompanyId == company.Id && u.WatchList.Id == watchlist.Id);
        }

        public string Token()
        {
            return "pk_b5d70060b9404cc8878bfbc4696918b9";
        }

        public async Task<bool> Commit()
        {
            return await _db.SaveChangesAsync() > 0;
        }

        public bool FundExists(string name)
        {
            return _db.Funds.Where(u => u.Name == name).Any();
        }

       public bool CompanyExists(string name)
            {
                return _db.Companies.Where(u => u.Symbol == name).Any();
            } 

        public  string GetName(string symbol)
        {
            var apiUrl = $"https://cloud.iexapis.com/stable/stock/{symbol}/company?token={Token()}";
            try
            {
                string name = string.Empty;
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(apiUrl);
                    HttpResponseMessage result = client.GetAsync(apiUrl).GetAwaiter().GetResult();
                    if (result.IsSuccessStatusCode)
                    {
                        CompanyVM data = result.Content.ReadAsAsync<CompanyVM>().GetAwaiter().GetResult();
                        name = data.companyName;
                    }
                }
                return name;
            }
            catch (Exception ex)
            {
                return "unknown";
            }
        }
        public CompanyVM Get_Company (string symbol)
        {
            var apiUrl2 = $"https://cloud.iexapis.com/stable/stock/{symbol}/company?token={iexTrading.Token()}";
            CompanyVM company = new CompanyVM();
            using (HttpClient client3 = new HttpClient())
            {
                HttpResponseMessage result3 = client3.GetAsync(apiUrl2).GetAwaiter().GetResult();
                if (result3.IsSuccessStatusCode)
                {
                    CompanyVM data = result3.Content.ReadAsAsync<CompanyVM>().GetAwaiter().GetResult();
                    company = data; 
                }
                return company;
            }
        }


        public User GetUser()
        {
             return _db.Users.Where(u => u.Id == "05c6775b-53cc-40ca-bc01-76612962fdb5").SingleOrDefault();
        }
        public string getWebsite(string symbol)
        {
            try
            {
                var apiUrl2 = $"https://cloud.iexapis.com/stable/stock/{symbol}/company?token={iexTrading.Token()}";
                EarningsInfo info = new EarningsInfo();

                using (HttpClient client3 = new HttpClient())
                {
                    HttpResponseMessage result3 = client3.GetAsync(apiUrl2).GetAwaiter().GetResult();
                    if (result3.IsSuccessStatusCode)
                    {
                        CompanyVM data = result3.Content.ReadAsAsync<CompanyVM>().GetAwaiter().GetResult();
                        if (data.CEO != null)
                        {
                            info.Website = data.website;
                        }
                    }
                    return info.Website;
                }
            }
            catch (Exception ex)
            {
                return "unknown";
            }
        }

        public string getCEO(string symbol)
        {
            try
            {
                var apiUrl2 = $"https://cloud.iexapis.com/stable/stock/{symbol}/company?token={iexTrading.Token()}";
                EarningsInfo info = new EarningsInfo();

                using (HttpClient client3 = new HttpClient())
                {
                    HttpResponseMessage result3 = client3.GetAsync(apiUrl2).GetAwaiter().GetResult();
                    if (result3.IsSuccessStatusCode)
                    {
                        CompanyVM data = result3.Content.ReadAsAsync<CompanyVM>().GetAwaiter().GetResult();
                        if (data.CEO != null)
                        {
                            info.CEO = data.CEO;
                        }
                    }
                    return info.CEO;
                }
            }
            catch(Exception ex)
            {
                return "unknown";
            } 
        }
            public string getDescription(string symbol)
            {
            try
            {
                var apiUrl2 = $"https://cloud.iexapis.com/stable/stock/{symbol}/company?token={iexTrading.Token()}";
                EarningsInfo info = new EarningsInfo();

                using (HttpClient client3 = new HttpClient())
                {
                    HttpResponseMessage result3 = client3.GetAsync(apiUrl2).GetAwaiter().GetResult();
                    if (result3.IsSuccessStatusCode)
                    {
                        CompanyVM data = result3.Content.ReadAsAsync<CompanyVM>().GetAwaiter().GetResult();
                        if (data.description == "")
                        {
                            info.Description = "no data provided";
                        }
                        else
                        {
                            info.Description = data.description;
                        }
                    }
                    return info.Description;
                }
            }          
            catch(Exception ex)
            {
                return "no description";
            }
        }

        public string getIndustry(string symbol)
        {
            try
            {
                var apiUrl2 = $"https://cloud.iexapis.com/stable/stock/{symbol}/company?token={iexTrading.Token()}";
                EarningsInfo info = new EarningsInfo();

                using (HttpClient client3 = new HttpClient())
                {
                    HttpResponseMessage result3 = client3.GetAsync(apiUrl2).GetAwaiter().GetResult();
                    if (result3.IsSuccessStatusCode)
                    {
                        CompanyVM data = result3.Content.ReadAsAsync<CompanyVM>().GetAwaiter().GetResult();
                        if (data.industry == "")
                        {
                            info.Industry = "no data provided";
                        }
                        else
                        {
                            info.Industry = data.industry;
                        }
                    }
                    return info.Industry;
                }
            }
            catch (Exception ex)
            {
                return "no description";
            }
        }

        public string getEarnings(string symbol)
        {
            try
            {
                var apiUrl2 = $"https://cloud.iexapis.com/stable/stock/{symbol}/stats?token={iexTrading.Token()}";
                EarningsInfo info = new EarningsInfo();

                using (HttpClient client3 = new HttpClient())
                {
                    HttpResponseMessage result3 = client3.GetAsync(apiUrl2).GetAwaiter().GetResult();
                    if (result3.IsSuccessStatusCode)
                    {
                        CompanyVM data = result3.Content.ReadAsAsync<CompanyVM>().GetAwaiter().GetResult();
                        if (data.nextEarningsDate != "")
                        {
                            info.Earnings = data.nextEarningsDate;
                        }
                        else
                        {
                            info.Earnings = "2022-01-01";
                        }
                    }
                    return info.Earnings;
                }
            }catch(Exception ex)
            {
                return "2022-01-01";
            }
         }
                    public  decimal getPeRatio(string symbol)
        {
            try
            {
                var apiUrl2 = $"https://cloud.iexapis.com/stable/stock/{symbol}/stats?token={iexTrading.Token()}"; 
                EarningsInfo info = new EarningsInfo();
                using (HttpClient client3 = new HttpClient())
                {
                    HttpResponseMessage result3 = client3.GetAsync(apiUrl2).GetAwaiter().GetResult();
                    if (result3.IsSuccessStatusCode)
                    {
                        CompanyVM data = result3.Content.ReadAsAsync<CompanyVM>().GetAwaiter().GetResult(); 

                        if (data.peRatio != 0)
                        {
                            info.PeRatio = data.peRatio;
                        }
                    }
                    return info.PeRatio;
                }
            }
            catch(Exception ex)
            {
                return 0;
            }           
        }
        public Company FindCompany(string symbol)
        {
            return _db.Companies.SingleOrDefault(u => u.Symbol == symbol);
        }

        public async Task Create_Company(string symbol)
        {  
            Company company = new Company
            {
                Symbol = symbol,
                CompanyName = GetName(symbol),
                PeRatio = getPeRatio(symbol),
                CEO = getCEO(symbol),
                description = getDescription(symbol),
                website = getWebsite(symbol),
                Industry = getIndustry(symbol)
            };
            _db.Companies.Add(company);
            await Task.CompletedTask;
        } 

        public void ReturnEarnings(string date, string symbol)
        {
            var company = FindCompany(symbol); 
            DateTime dateToSave = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var c = date.Split("-");
            int number = Int16.Parse(c[1]);

            if (number <= 3 && company.FirstQ != null)
            {
                company.FirstQ = dateToSave;
            }
            if (number >= 3 && number <= 6 && company.SecondQ != null)
            {
                company.SecondQ = dateToSave;
            };
            if (number >= 6 && number <= 9 && company.ThirdQ != null)
            {
                company.ThirdQ = dateToSave;
            }
            if (number >= 9 && company.FirstQ != null)
            {
                company.FourthQ = dateToSave;
            }
        }

        public async Task SaveEarnings(string date, string symbol)
        {
            var company = _db.Companies.SingleOrDefault(u => u.Symbol == symbol);
            try
            {               
                if (company == null && date != null)
                {
                    await Create_Company(symbol);
                    await Commit();
                    Company addedCompany = FindCompany(symbol);
                    DateTime dateToSave = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    var c = date.Split("-");
                    int number = Int16.Parse(c[1]);

                    if (number <= 3)
                    {
                        addedCompany.FirstQ = dateToSave;
                        await Commit();
                    }
                    if (number >= 3 && number <= 6)
                    {
                        addedCompany.SecondQ = dateToSave;
                        await Commit();
                    };
                    if (number >= 6 && number <= 9)
                    {
                        addedCompany.ThirdQ = dateToSave;
                        await Commit();
                    }
                    if (number >= 9)
                    {
                        addedCompany.FourthQ = dateToSave;
                        await Commit();
                    }
                }
                else
                {
                    Company addedCompany = FindCompany(symbol);
                    await SavingEarnings(date, addedCompany);
                }
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {                
                await SavingEarnings(date, company);
            }
        }

         public async Task SavingEarnings(string date,Company company)
        {
            try
            {
                DateTime dateToSave = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var c = date.Split("-");
                int number = Int16.Parse(c[1]);

                if (number <= 3 && company.FirstQ != null)
                {
                    company.FirstQ = dateToSave;
                    await Commit();
                }
                if (number >= 3 && number <= 6 && company.SecondQ != null)
                {
                    company.SecondQ = dateToSave;
                    await Commit();
                };
                if (number >= 6 && number <= 9 && company.ThirdQ != null)
                {
                    company.ThirdQ = dateToSave;
                    await Commit();
                }
                if (number >= 9 && company.FirstQ != null)
                {
                    company.FourthQ = dateToSave;
                    await Commit();
                }
                await Task.CompletedTask;
            }catch(Exception ex)
            {

            }
           
        }           
    } 
}
 
