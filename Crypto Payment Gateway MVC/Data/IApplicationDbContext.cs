using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Crypto_Payment_Gateway_MVC.Models.DbModels;

namespace Crypto_Payment_Gateway_MVC.Data
{
    public interface IApplicationDbContext : IDisposable
    {
        DbSet<SiteUser> SiteUsers { get; set; }
        public DbSet<Transactions> Transactions { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletTransaction> WalletTransactions { get; set; }
        public DbSet<Withdraw> Withdraws { get; set; }
        public DbSet<WaitingWalletTransaction> WaitingWalletTransactions { get; set; }







        void Dispose();
        ValueTask DisposeAsync();
        int SaveChanges();
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);
    }
}
