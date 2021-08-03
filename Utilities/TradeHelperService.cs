using System;
using Microsoft.AspNetCore.Authorization;
using FantasyWealth.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using FantasyWealth.Areas.Identity.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using FantasyWealth.Controllers;
using Microsoft.EntityFrameworkCore;
using System.Linq;


namespace FantasyWealth.Utilities
{
    [Authorize]
    public class TradeHelperService : Controller
    {
        public static IConfiguration Configuration { get; set; }
        private readonly UserManager<User> _userManager;
        private readonly StockMasterDbContext _dbContext;
        // private readonly SignInManager<FantasyWealthUser> _signInManager;
        private readonly IHttpContextAccessor _context;

        public TradeHelperService(
            StockMasterDbContext DbContext,
            UserManager<User> userManager,
            
            IHttpContextAccessor context)
            
        {
            //constructor injection to get a dependency.
            _dbContext = DbContext;
            _userManager = userManager;
            //  _signInManager = signInManager;
            _context = context;
            
        }
        [HttpGet]
        public async Task<decimal> apiAveragePurchasePrice(int symbolId)
        {
            /* raw sql
            select sum(t.price)/count(*)
            from Wealths w
            inner join Trades t on w.SymbolId=t.SymbolId
            where w.UserId=@userId and w.SymbolId=@symbolId and t.Action=1 (buy)
             */


            string userId = _userManager.GetUserAsync(_context.HttpContext.User).Result.Id;
            var averagePrice = await _dbContext.Trades
                    .FromSql("SELECT * from Trades   where UserId={0} and SymbolId={1} and Action=1", userId, symbolId)
                    .Select(t => t.Price)
                    .AverageAsync();

            return averagePrice;
        }

        [HttpGet]
        public async Task<string> apiWealthFind(int symbolId)
        {
            var user = _userManager.GetUserAsync(_context.HttpContext.User);
            string userId = user.Result.Id;
            var wealth = await _dbContext.Wealths
            .Where(w => w.UserId == userId && w.SymbolId == symbolId)
            .Select(s => s.Quantity).ToListAsync();
          
            if (wealth.Count() > 0)
            {
                return wealth[0].ToString();
            }
            else
            {
                return null;
            }
        }
        public async Task createWealth(Transaction transaction)
        {
            int symbolId = transaction.Trade.SymbolId;
            string userId = transaction.UserId;
            string action = transaction.Trade.Action.ToString();
            
            var wealth = await isWealthOwner(symbolId, userId);
            
            if (wealth != null)
            {
              
                if (transaction.Trade.Action.ToString().ToUpper() == "BUY")
                {
                    wealth.Quantity = wealth.Quantity + transaction.Trade.Quantity;
                    wealth.UpdatedDate = DateTime.Now;
                    await _dbContext.SaveChangesAsync();
                }
                else if (transaction.Trade.Action.ToString().ToUpper() == "SELL")
                {
                   
                    if (transaction.Trade.Quantity <= wealth.Quantity)
                    {
                        wealth.Quantity = wealth.Quantity - transaction.Trade.Quantity;
                        wealth.UpdatedDate = DateTime.Now;
                        await _dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        //we had to check it before. 
                        // Not happens 
                        // ajax and jQuery already implemented. It will not happen if user doesn't have share enough.
                    }
                }
            }
            else
            {
                Wealth tempWealth = new Wealth();
                tempWealth.CreationDate = DateTime.Now;
                tempWealth.UpdatedDate = DateTime.Now;
                tempWealth.Quantity = transaction.Trade.Quantity;
                tempWealth.SymbolId = transaction.Trade.SymbolId;
                tempWealth.UserId = transaction.UserId;
                await CreateWealth(tempWealth);
            }
        }
        public async Task<Wealth> isWealthOwner(int symbolId, string userId)
        {

            var wealth = await _dbContext.Wealths
            .Where(w => w.UserId == userId && w.SymbolId == symbolId)
            .FirstOrDefaultAsync();
          
            if (wealth == null)
            {
                return null;
            }
            else
            {
                return wealth;
            }
        }
        public async Task<Transaction> createTradeTransaction(Trade trade)
        {
            Transaction transaction = new Transaction();
            transaction.CreationDate = DateTime.Now;
            transaction.TradeId = trade.Id;
            transaction.UserId = trade.UserId;
            transaction.Reconciled = true;
            if (trade.Action.ToString().ToUpper() == "SELL")
            {
                transaction.TransactionType = TransactionType.Debit;
                transaction.TotalAmount = trade.Quantity * trade.Price;
                transaction.FromAccount = Account.Broker;
                transaction.ToAccount = Account.Broker;

            }
            else if (trade.Action.ToString().ToUpper() == "BUY")
            {
                transaction.TransactionType = TransactionType.Credit;
                transaction.TotalAmount = trade.Quantity * trade.Price;
                transaction.FromAccount = Account.Broker;
                transaction.ToAccount = Account.Broker;
            }

            await createTransaction(transaction);
            return transaction;
        }
        public async Task initiateFund(decimal balance, string userId)
        {
            Transaction transaction = new Transaction();

            transaction.TotalAmount = balance;
            transaction.Comment = "New Account. Current Promotion on new reagistration!";
            transaction.CreationDate = DateTime.Now;
            transaction.FromAccount = Account.Broker;
            transaction.ToAccount = Account.Broker;
            transaction.Reconciled = true;
            transaction.TransactionType = TransactionType.Debit;
            transaction.TradeId = null;
            transaction.UserId = userId;
            await createTransaction(transaction);
        }
        public async Task createTransaction([Bind("Id,UserId,TradeId,TransType,FromAccount,ToAccount,TotalAmount,Comment,TimeStamp,Reconciled")] Transaction transaction)
        {
            _dbContext.Add(transaction);
            await _dbContext.SaveChangesAsync();

        }
        public async Task CreateWealth([Bind("Id,UserId,SymbolId,Quantity,CreattionDate,UpdatedDate")] Wealth wealth)
        {
            _dbContext.Add(wealth);
            await _dbContext.SaveChangesAsync();
        }
        public static string readConfigurationSetting(string configurationName)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
            Configuration = builder.Build();

            return Configuration[configurationName];

        }

        public async Task<decimal> getUserCashBalance(Transaction transaction)
        {
            var user = await _userManager.GetUserAsync(_context.HttpContext.User);
            if (transaction.UserId == user.Id)
            {
                decimal balance = user.CashBalanceAmount;
                return balance;
            }
            else
            {
                return 0;
            }
        }
        public async Task<bool> updateUserCashBalance(Transaction transaction)
        {
            bool result = true;
            var user = await _userManager.GetUserAsync(_context.HttpContext.User);
            if (user.Id == transaction.UserId)
            {
                decimal balance = user.CashBalanceAmount;
                decimal transactionAmount = transaction.TotalAmount;
                int switchType = transaction.TransactionType.GetHashCode();
                switch (switchType)
                {
                    case 0:  //Transfer
                        if (transaction.FromAccount.GetHashCode() == 0 && transaction.ToAccount.GetHashCode() == 1)  //Bank 2 Broker
                        {
                            balance = balance + transactionAmount;
                        }
                        else if (transaction.FromAccount.GetHashCode() == 1 && transaction.ToAccount.GetHashCode() == 0) //Broker 2 Bank
                        {
                            balance = balance - transactionAmount;
                        }
                        else
                        {
                           
                            // Send message
                            // Do nothing
                            transaction.Comment = "No Money Transfer.";
                            result = false;
                        }
                        break;
                    case 1:   //Debit
                        balance = balance + transactionAmount;
                        break;
                    case 2: //CRedit
                        balance = balance - transactionAmount;
                        break;
                    default:
                        transaction.Comment = "No Money Transfer.";
                        // send message
                        break;
                }
                user.CashBalanceAmount = balance;
                await _userManager.UpdateAsync(user);
            }
            else
            {
                result = false;
            }
            return result;
        }
    }
}