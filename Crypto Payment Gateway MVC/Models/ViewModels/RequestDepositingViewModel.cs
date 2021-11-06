using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crypto_Payment_Gateway_MVC.Models.ViewModels
{
    public class RequestDepositingViewModel
    {
        public string ReciveWallet { get; set; }
        public DateTime StartedTime { get; set; }
        public float Amount { get; set; }
        public bool Error { get; set; }
        public string Message { get; set; }
    }
}
