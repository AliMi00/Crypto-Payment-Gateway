using Crypto_Payment_Gateway.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crypto_Payment_Gateway.Models.InternalModels
{
    public class GeneralResponse
    {
        public Status Status { get; set; }
        public string Message { get; set; }

    }
}
