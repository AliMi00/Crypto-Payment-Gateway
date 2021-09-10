using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crypto_Payment_Gateway_MVC.Models.DbModels;
using Crypto_Payment_Gateway_MVC.Models.Enums;
using Crypto_Payment_Gateway_MVC.Models.InternalModels;

namespace Crypto_Payment_Gateway_MVC.Services
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
        /// for depositing Basically this method add waiting wallet tran and return address and end time amount is important for deposit 
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="amount"></param>
        /// <param name="siteUser"></param>
        /// <param name="transactionHash"> it is not requerd in case user use his own wallet</param>
        /// <param name="isItUserWallet">if it is it use main wallet and if the money added buy user wallet address is maping it to his account</param>
        /// <param name="userWallet"></param>
        /// <returns> model for showing the user the wallet and end time </returns>
        public Task<AccountingIncreaseBalance> UserIncreaseBalanceRequest(Currency currency, float amount, SiteUser siteUser, string userWallet = "");

        /// <summary>
        /// checking waiting wallet tran with wallet confirmed tran and if finds mach add to transactions using AddtransactionIncrease
        /// </summary>
        /// <returns> return list of resposes of increase transaction </returns>
        public Task<ICollection<AccountingAddTransaction>> CheckingWaitingWalletTransaction();


        /// <summary>
        /// search ad get data from api and add wallet transactions to db
        /// </summary>
        /// <param name="currency"></param>
        /// <returns>List of wallet transactions that been added to db </returns>
        public Task<ICollection<WalletTransaction>> AddWalletTransactions(Currency currency);

        /// <summary>
        /// get transaction of user in time or all the data 
        /// </summary>
        /// <param name="siteUser"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="transactionCount"> number of record that been returned </param>
        /// <returns></returns>
        public ICollection<Transactions> GetUserTransactions(SiteUser siteUser, DateTime? startDate, DateTime? endDate, int transactionCount);



    }
}
