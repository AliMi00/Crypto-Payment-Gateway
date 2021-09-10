using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crypto_Payment_Gateway_MVC.Models.DbModels.Enums
{
    public enum UserStatus
    {
        Active,
        DeactivatedByUser,
        DeactivatedByAdmin,
        DeactivatedBySystem,
        DeactivatedForTransactionIssue,
    }
}
