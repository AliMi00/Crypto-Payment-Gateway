using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crypto_Payment_Gateway_MVC.Models.Enums
{
    public enum WithdrawStatus
    {
        Successful,
        WaitingForAdminAprowal,
        WaitingForAccountingDep,
        WaitingForTransaction,
        Faild,
        UnAthorized,
        SystemError,
        CanceledByUser,
        CanceledByAdmin,

        
    }
}
