using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crypto_Payment_Gateway_MVC.Models.Enums;

namespace Crypto_Payment_Gateway_MVC.Models.DbModels
{
    public class WalletTransaction
    {
        public int Id { get; set; }
        public string Hash { get; set; }
        public Wallet Wallet { get; set; }
        public string OtherAddress { get; set; }
        public float Ammount { get; set; }
        public Currency Currency { get; set; }
        public WalletTransactionType WalletTransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
        public bool IsConfirmed { get; set; }
        public float ExchangeRateToUsd { get; set; }
        public bool IsAddedToTransactions { get; set; }


    }
}
