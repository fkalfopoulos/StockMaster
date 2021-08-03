using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FantasyWealth.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FantasyWealth.Models;

namespace FantasyWealth.Areas.Identity.Data
{
    public class StockMasterDbContext : IdentityDbContext<User>
    {
        public StockMasterDbContext(DbContextOptions<StockMasterDbContext> options)
            : base(options)
        {
        }
        public DbSet<Wealth> Wealths { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Trade> Trades { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<HedgeFund> Funds { get; set; }
        public DbSet<HedgeFund_Companies> Hedge_Companies { get; set; }
        public DbSet<WatchList_Companies> WatchList_Companies { get; set; }
        public DbSet<WatchList> WatchLists { get; set; }         
        public DbSet<Offer> Offers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<User>().Property(FantasyWealthUser => FantasyWealthUser.CashBalanceAmount)
            .HasColumnType("decimal(18,2)");
            builder.Entity<Trade>().Property(Trade => Trade.Price)
           .HasColumnType("decimal(18,2)");
            builder.Entity<Transaction>().Property(Transaction => Transaction.TotalAmount)
           .HasColumnType("decimal(18,2)");
            builder.Entity<Trade>().Property(Trade => Trade.UserId)
            .IsRequired(true);
            builder.Entity<Transaction>().Property(Transaction => Transaction.UserId)
            .IsRequired(true);
            builder.Entity<Wealth>().Property(Wealth => Wealth.UserId)
            .IsRequired(true);
            builder.Entity<Offer>().Property(Offer => Offer.CurrentPrice)
            .HasColumnType("decimal(18,2)");
            builder.Entity<Offer>().Property(Offer => Offer.OfferPrice)
            .HasColumnType("decimal(18,2)");


            

            builder.Entity<HedgeFund>()
           .HasMany(m => m.Companies);


        }
    }
}
