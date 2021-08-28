using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Crypto_Payment_Gateway.Data;
using Crypto_Payment_Gateway.Models.DbModels;
using Crypto_Payment_Gateway.Models.DbModels.Enums;
using Crypto_Payment_Gateway.Models.Enums;
using Crypto_Payment_Gateway.Models.InternalModels;

namespace Crypto_Payment_Gateway.Services
{
    public class AccountingServices: IAccountingServices
    {
        private readonly IApplicationDbContext db;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ILogger<AccountingServices> logger;

        public AccountingServices(IApplicationDbContext db, IWebHostEnvironment webHostEnvironment, ILogger<AccountingServices> _logger)
        {
            this.db = db;
            this.webHostEnvironment = webHostEnvironment;
            logger = _logger;
        }

        public async Task<AccountingAddTransaction> AddTransactionIncrease(Transactions transactions)
        {
            //need to check for any error we add transaction before the checkup
            AccountingAddTransaction result = new();
            transactions.Date = DateTime.Now;
            transactions.IsAdded = true;
            await db.Transactions.AddAsync(transactions);
            float added = db.Transactions.Where(x => x.User == transactions.User && x.IsAdded).Sum(x => x.AmountInUsd);
            float subtracted = db.Transactions.Where(x => x.User == transactions.User && !x.IsAdded).Sum(x => x.AmountInUsd);
            float balance = added - subtracted;
            var entity = db.SiteUsers.SingleOrDefault(x => x.Id == transactions.User.Id);
            if (balance < 0)
            {
                logger.LogInformation("there is propblem in user payments userID is ---> " + transactions.User.Id.ToString());
                entity.UserStatus = Models.DbModels.Enums.UserStatus.DeactivatedForTransactionIssue;
                db.SiteUsers.Update(entity);
                result.Msssage = "error in this user transactions";
                result.Status = Models.Enums.Status.Faild;
            }
            else
            {

                entity.Balance = balance;
                db.SiteUsers.Update(entity);
                result.Msssage = "successful";
                result.Status = Models.Enums.Status.SuccessFul;
            }
            await db.SaveChangesAsync(true);
            return result;

        }

        public async Task<AccountingAddTransaction> AddTransactionDecrease(Transactions transactions)
        {
            AccountingAddTransaction result = new();
            if(transactions.User.UserStatus != Models.DbModels.Enums.UserStatus.Active)
            {
                result.Status = Models.Enums.Status.Faild;
                result.Msssage = "this account is not allowd for this action";
                return result;
            }
            //for now just return string but replace by some model
            transactions.Date = DateTime.Now;
            transactions.IsAdded = false;
            float added = db.Transactions.Where(x => x.User == transactions.User && x.IsAdded).Sum(x => x.AmountInUsd);
            float subtracted = db.Transactions.Where(x => x.User.Id == transactions.User.Id && !x.IsAdded).Sum(x => x.AmountInUsd);
            float balance = added - subtracted;
            var entity = db.SiteUsers.SingleOrDefault(x => x.Id == transactions.User.Id);

            if (balance < 0)
            {
                logger.LogInformation("there is propblem in user payments userID is ---> " + transactions.User.Id.ToString());
                entity.UserStatus = Models.DbModels.Enums.UserStatus.DeactivatedForTransactionIssue;
                db.SiteUsers.Update(entity);
                result.Msssage = "there is error in this user transactions";
                result.Status = Models.Enums.Status.Faild;
            }
            else if (balance < transactions.AmountInUsd)
            {
                result.Msssage = "blance of the user is not enough for this transaction";
                result.Status = Models.Enums.Status.Faild;
            }
            else
            {
                entity.Balance = balance - transactions.AmountInUsd;
                db.SiteUsers.Update(entity);
                await db.Transactions.AddAsync(transactions);
                result.Msssage = "successful";
                result.Status = Models.Enums.Status.SuccessFul;

            }

            await db.SaveChangesAsync(true);
            return result;

        }

        public async Task<float> GetUserBalanceAsync(SiteUser siteUser)
        {
            float added = db.Transactions.Where(x => x.User.Id == siteUser.Id && x.IsAdded).Sum(x => x.AmountInUsd);
            float subtracted = db.Transactions.Where(x => x.User.Id == siteUser.Id && !x.IsAdded).Sum(x => x.AmountInUsd);
            float balance = added - subtracted;

            if (balance < 0)
            {
                logger.LogInformation("there is propblem in user payments userID is ---> " + siteUser.Id.ToString());
                siteUser.UserStatus = Models.DbModels.Enums.UserStatus.DeactivatedForTransactionIssue;
                db.SiteUsers.Update(siteUser);
                await db.SaveChangesAsync(true);
                return 0.0f;
            }

            return balance;
        }

        public async Task<bool> UserWithdrawRequest(SiteUser siteUser, float withdrawAmount)
        {
            float UserCurentBalance =await GetUserBalanceAsync(siteUser);
            if (withdrawAmount > UserCurentBalance)
            {
                return false;
            }
            Transactions transactions = new();
            transactions.AmountInUsd = UserCurentBalance - 1;
            transactions.AmountOriginal = UserCurentBalance;
            transactions.Currency = siteUser.AddressCurrency;
            transactions.IsAdded = false;
            transactions.TransactionType = TransactionType.CryptoWithdraw;
            transactions.User = siteUser;

            var result = await AddTransactionDecrease(transactions);

            if(result.Status != Models.Enums.Status.SuccessFul)
            {
                return false;
            }

            Withdraw withdrawRequest = new();
            withdrawRequest.Amount = transactions.AmountInUsd;
            withdrawRequest.ReciveAddress = siteUser.CryptoAddress;
            withdrawRequest.Status = Models.Enums.WithdrawStatus.WaitingForAdminAprowal;
            withdrawRequest.User = siteUser;
            await db.Withdraws.AddAsync(withdrawRequest);
            await db.SaveChangesAsync(true);

            return true;
        }

        public async Task<AccountingIncreaseBalance> UserIncreaseBalanceRequest(Currency currency, float amount, SiteUser siteUser, string transactionHash, bool isItUserWallet, string userWallet)
        {
            var result = new AccountingIncreaseBalance();
            if(siteUser.UserStatus != UserStatus.Active)
            {
                result.Message = "account is not Active";
                result.Error = true;
                return result;
            }
            if(siteUser.CryptoAddress != userWallet && isItUserWallet)
            {
                result.Error = true;
                result.Message = "change the cryptoAddress is not mached with given";
                return result;
            }
            Wallet wallet;
            if (isItUserWallet)
            {
                wallet = db.Wallets.Where(x => x.IsMainWallet && x.Currency == currency).FirstOrDefault();
                if(wallet == null)
                {
                    result.Error = true;
                    result.Message = "the currency is not suported in Main wallets";
                }

                WaitingWalletTransaction waitingWallet = new();
                waitingWallet.Currency = currency;
                waitingWallet.StartedTime = DateTime.Now;
                waitingWallet.User = siteUser;
                waitingWallet.ReciveWallet = wallet;
                waitingWallet.UserWallet = siteUser.CryptoAddress;

                await db.WaitingWalletTransactions.AddAsync(waitingWallet);

                result.Error = false;
                result.Amount = amount;
                result.IsMainAccount = true;
                result.Message = "increase in MainWallets";
                result.ReciveWallet = wallet.WalletAddress;
                result.StartedTime = DateTime.Now;
                result.SenderWallet = userWallet;

                await db.SaveChangesAsync(true);
                return result;
            }
            else
            {
                wallet = db.Wallets.Where(x => x.IsAvailable && x.Currency == currency && x.IsAvailable).FirstOrDefault();
                if (wallet == null)
                {
                    result.Message = "There is no wallet to add try again later";
                    result.Error = true;
                    return result;
                }
                wallet.IsAvailable = false;
                wallet.ReleaseDate = DateTime.Now.AddMinutes(10);
                db.Wallets.Update(wallet);

                WaitingWalletTransaction waitingWallet = new();
                waitingWallet.Currency = currency;
                waitingWallet.StartedTime = wallet.ReleaseDate.Value.AddMinutes(-10);
                waitingWallet.User = siteUser;
                waitingWallet.ReciveWallet = wallet;



                await db.WaitingWalletTransactions.AddAsync(waitingWallet);
                await db.SaveChangesAsync(true);

                result.Amount = amount;
                result.Error = false;
                result.IsMainAccount = false;
                result.Message = "increase in otherAccount";
                result.ReciveWallet = wallet.WalletAddress;
                result.StartedTime = DateTime.Now;
                result.SenderWallet = userWallet;

                return result;
            }
        }

        public async Task<int> CheckingWaitingWalletTransaction(bool IsInMainWallet, DateTime startedTime, DateTime endTime, bool IsDeleted = false, SiteUser siteUser = null)
        {
            int count = 0;
            if (IsInMainWallet)
            {
                // get all transaction that added to db 
                //confirmed mainwallet from started to ended time given 
                ICollection<WalletTransaction> transactions = db.WalletTransactions.Where(
                    x => x.IsConfirmed &&
                    x.Wallet.IsMainWallet &&
                    x.TransactionDate >= startedTime &&
                    x.TransactionDate <= endTime)
                    .ToList();
                //get all the waiting transaction that added when user requested for the deposit in main wallet from started time 
                ICollection<WaitingWalletTransaction> waitingTransactions = db.WaitingWalletTransactions.Where(
                    x => !x.IsDeleted &&
                    x.StartedTime >= startedTime)
                    .ToList();

                //check each of transaction that is happend with all waiting transactions
                foreach(WalletTransaction transaction in transactions)
                {
                    if(waitingTransactions.Any(x => x.UserWallet == transaction.OtherAddress))
                    {
                        Transactions transactionsInternal = new Transactions();
                        transactionsInternal.TransactionType = transaction.WalletTransactionType == WalletTransactionType.AdminDeposit ? TransactionType.CryptoDeposit   : TransactionType.CryptoWithdraw;
                        transactionsInternal.AmountInUsd = transaction.Ammount;
                        transactionsInternal.AmountOriginal = transaction.Ammount;
                        transactionsInternal.Currency = transaction.Wallet.Currency;
                        transactionsInternal.IsAdded = transaction.WalletTransactionType == WalletTransactionType.UserDeposit ? true : false;
                        transactionsInternal.User = waitingTransactions.Where(x => x.UserWallet == transaction.OtherAddress).FirstOrDefault().User;

                        AccountingAddTransaction accountingAddTransactionResponse = 
                            transactionsInternal.IsAdded ? await AddTransactionIncrease(transactionsInternal) : await AddTransactionDecrease(transactionsInternal);
                        count = accountingAddTransactionResponse.Status == Status.SuccessFul ? count + 1 : count;

                        
                    }
                }

            }

            return count;
        }

        public async Task AddWalletTransactions(DateTime startDate,Currency currency)
        {
            DateTime endTime = DateTime.Now;
            ICollection<WalletTransaction> walletTransactions;
            ICollection<Wallet> wallets = db.Wallets.Where(x => x.Currency == currency).ToList();

            if(currency == Currency.USDTerc20)
            {
                walletTransactions =await GetUSDTerc20Transactions(startDate, wallets, endTime);
            }
            else if(currency == Currency.USDTtrc20)
            {
                walletTransactions = await GetUSDTtrc20Transactions(startDate, wallets, endTime);
            }
            else if (currency == Currency.BUSDbep2)
            {
                walletTransactions = await GetBUSDbep2Transactions(startDate, wallets, endTime);
            }
            else
            {
                return;
            }
        }
        
        private async Task<ICollection<WalletTransaction>> GetUSDTerc20Transactions(DateTime startTime,ICollection<Wallet> wallets,DateTime endTime)
        {
            ICollection<WalletTransaction> walletTransactions = new List<WalletTransaction>();

            //data that need to be past to api
            string jsonObject = "";
            string url = "";
            //object should change to model of response
            Object result;


            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
                    var response = await client.GetAsync(url);
                    if (response != null)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        result = JsonConvert.DeserializeObject<object>(jsonString);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
            //need to create walletTransaction for each object that is recived
            return walletTransactions;


        }
        private async Task<ICollection<WalletTransaction>> GetUSDTtrc20Transactions(DateTime startTime, ICollection<Wallet> wallets, DateTime endTime)
        {
            ICollection<WalletTransaction> walletTransactions = new List<WalletTransaction>();

            //data that need to be past to api
            string jsonObject = "";
            string url = "";
            //object should change to model of response
            Object result;


            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
                    var response = await client.GetAsync(url);
                    if (response != null)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        result = JsonConvert.DeserializeObject<object>(jsonString);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
            //need to create walletTransaction for each object that is recived
            return walletTransactions;


        }
        private async Task<ICollection<WalletTransaction>> GetBUSDbep2Transactions(DateTime startTime, ICollection<Wallet> wallets, DateTime endTime)
        {
            ICollection<WalletTransaction> walletTransactions = new List<WalletTransaction>();

            //data that need to be past to api
            string jsonObject = "";
            string url = "";
            //object should change to model of response
            Object result;


            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
                    var response = await client.GetAsync(url);
                    if (response != null)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        result = JsonConvert.DeserializeObject<object>(jsonString);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
            //need to create walletTransaction for each object that is recived
            return walletTransactions;


        }

    }
}
