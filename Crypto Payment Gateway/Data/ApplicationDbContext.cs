using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Crypto_Payment_Gateway.Models.DbModels;
using System.Threading;
using System.Threading.Tasks;



namespace Crypto_Payment_Gateway.Data
{
    public class ApplicationDbContext : IdentityDbContext<SiteUser>, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<SiteUser> SiteUsers { get; set; }
        public DbSet<Transactions> Transactions { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletTransaction> WalletTransactions { get; set; }
        public DbSet<Withdraw> Withdraws { get; set; }
        
        public DbSet<WaitingWalletTransaction> WaitingWalletTransactions { get; set; }




        public override int SaveChanges()
        {
            return base.SaveChanges();
        }
        public override void Dispose()
        {
            base.Dispose();
        }
        public override ValueTask DisposeAsync()
        {
            return base.DisposeAsync();
        }
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
