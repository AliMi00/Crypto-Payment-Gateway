using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crypto_Payment_Gateway.Models.DbModels;
using Crypto_Payment_Gateway.Models.Enums;
using Crypto_Payment_Gateway.Models.InternalModels;

namespace Crypto_Payment_Gateway.Services
{
    public interface IAccountingServices
    {
        /// <summary>
        /// add valid transaction to db and update balance of user (Increase money transaction)
        /// </summary>
        /// <param name="transactions"></param>
        /// <returns></returns>
        Task<AccountingAddTransaction> AddTransactionIncrease(Transactions transactions);

        /// <summary>
        /// basicly add valid transaction to db and update balance of user (Decrease money transaction)
        /// </summary>
        /// <param name="transactions"></param>
        /// <returns></returns>
        Task<AccountingAddTransaction> AddTransactionDecrease(Transactions transactions);

        /// <summary>
        /// get curent user balance
        /// </summary>
        /// <returns></returns>
        Task<float> GetUserBalanceAsync(SiteUser siteUser);

        /// <summary>
        /// user withdraw need to check and then withdraw the amount to user walet address 
        /// </summary>
        /// <param name="siteUser"></param>
        /// <param name="withdrawAmount"></param>
        /// <returns></returns>
        Task<bool> UserWithdrawRequest(SiteUser siteUser,float withdrawAmount);

        /// <summary>
        /// for adding balance we use this func 
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="amount"></param>
        /// <param name="siteUser"></param>
        /// <param name="transactionHash"> it is not requerd in case user use his own wallet</param>
        /// <param name="isItUserWallet">if it is it use main wallet and if the money added buy user wallet address is maping it to his account</param>
        /// <param name="userWallet"></param>
        /// <returns> model for showing the user the wallet and end time </returns>
        public Task<AccountingIncreaseBalance> UserIncreaseBalanceRequest(Currency currency, float amount, SiteUser siteUser, string transactionHash, bool isItUserWallet, string userWallet);

        public Task<int> CheckingWaitingWalletTransaction(bool IsInMainWallet, DateTime startedTime, DateTime endTime, bool IsDeleted = false, SiteUser siteUser = null);



    }
}
