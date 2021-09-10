using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crypto_Payment_Gateway_MVC.Models.Enums;

namespace Crypto_Payment_Gateway_MVC.Models.DbModels
{
    public class Wallet
    {
        public int Id { get; set; }
        public string WalletAddress { get; set; }
        public Currency Currency { get; set; }
        public string PrivateKey { get; set; }
        public int UsedCounter { get; set; }
        public bool IsMainWallet { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime? ReleaseDate { get; set; }




    }
}
