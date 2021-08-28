using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crypto_Payment_Gateway.Models.Enums;

namespace Crypto_Payment_Gateway.Models.InternalModels
{
    public class AccountingAddTransaction
    {
        public Status Status { get; set; }
        public string Msssage { get; set; }

    }
}
