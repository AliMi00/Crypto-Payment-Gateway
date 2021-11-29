using Crypto_Payment_Gateway_MVC.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crypto_Payment_Gateway_MVC.Models.ViewModels
{
    public class AddWalletViewModel
    {
        public int Id { get; set; }
        public string WalletAddress { get; set; }
        public Currency Currency { get; set; }
        public string PrivateKey { get; set; }
        public int UsedCounter { get; set; }
        public bool IsMainWallet { get; set; }
        public bool IsAvailable { get; set; }
    }
}
