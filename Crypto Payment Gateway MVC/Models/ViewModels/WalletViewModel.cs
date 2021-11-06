using Crypto_Payment_Gateway_MVC.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crypto_Payment_Gateway_MVC.Models.ViewModels
{
    public class WalletViewModel
    {
        public int Id { get; set; }
        public string WalletAddress { get; set; }
        public Currency Currency { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime? ReleaseDate { get; set; }
    }
}
