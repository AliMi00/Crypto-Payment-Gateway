using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crypto_Payment_Gateway_MVC.Models.Enums;

namespace Crypto_Payment_Gateway_MVC.Models.DbModels
{
    public class Withdraw
    {
        public int Id { get; set; }
        public SiteUser User { get; set; }
        public float Amount { get; set; }
        public WithdrawStatus Status { get; set; }
        public string SendAddress { get; set; }
        public string ReciveAddress { get; set; }
        public string TransactionHash { get; set; }
        public Transactions Transaction { get; set; }



    }
}
