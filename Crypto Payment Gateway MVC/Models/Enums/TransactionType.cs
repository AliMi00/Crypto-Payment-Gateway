using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crypto_Payment_Gateway_MVC.Models.DbModels
{
    public enum TransactionType
    {
        CryptoDeposit,
        PaypalDeposit,
        VisaDeposit,
        MasterDeposit,
        InternalBuy,
        InternalSell,
        InternalReward,
        CryptoWithdraw,
        Temp
    }
}
