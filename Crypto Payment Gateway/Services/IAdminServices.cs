using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crypto_Payment_Gateway.Models.DbModels;
using Crypto_Payment_Gateway.Models.Enums;
using Crypto_Payment_Gateway.Models.InternalModels;

namespace Crypto_Payment_Gateway.Services
{
    /// <summary>
    /// this service should only use in controllers with admin privilages
    /// </summary>
    public interface IAdminServices
    {
        /// <summary>
        /// upgrade user or add user to Manager role only use in 
        /// admin privilages controller
        /// </summary>
        /// <param name="siteUser"></param>
        /// <returns></returns>
        Task<bool> AddUserToManager(SiteUser siteUser);

        /// <summary>
        /// this func return all the ticket and codes that been sold 
        /// requierd game and date probebly for parameter 
        /// return list of all codes
        /// </summary>
        /// <returns></returns>
        Task GetAllTicket();

        /// <summary>
        /// add winner or other data to front page it should probebly add some more func to do this
        /// </summary>
        /// <returns></returns>
        Task AddDataToFrontPage();

        /// <summary>
        /// managing the games enable or disable games change fee of games as well
        /// </summary>
        /// <returns></returns>
        Task ManagingGames();

        /// <summary>
        /// managing referes and pirsentege that each user get after referar mostly 
        /// </summary>
        /// <returns></returns>
        Task ManagingreReferee();

        /// <summary>
        /// create coupon
        /// </summary>
        /// <returns></returns>
        Task CreateCoupon();

        /// <summary>
        /// disable coupon
        /// </summary>
        /// <returns></returns>
        Task DisableCoupon();

        ICollection<Transactions> GetAllTransactions(DateTime startDate, DateTime endDate);

        ICollection<Wallet> GetWallets(Currency currency = Currency.Other);

        ICollection<WalletTransaction> GetWalletTransactions(DateTime startDate, DateTime endDate);

        ICollection<Withdraw> GetWithdrawsRequest(WithdrawStatus status);

        Task<GeneralResponse> AddWallet(Wallet wallet);

        GeneralResponse ModifyWithdrawsResquest(Withdraw withdraw);

        GeneralResponse ModifyWallet(Wallet wallet);

    }
}
