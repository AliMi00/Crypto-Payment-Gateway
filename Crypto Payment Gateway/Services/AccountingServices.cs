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
using System.Globalization;

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

        //ok
        public async Task<AccountingAddTransaction> AddTransactionIncrease(Transactions transactions)
        {
            //need to check for any error we add transaction before the checkup
            AccountingAddTransaction result = new();
            transactions.Date = DateTime.Now;
            if (!transactions.IsAdded)
            {
                result.Status = Status.Faild;
                result.Msssage = "wrong methods";
                return result;
            }
            transactions.IsAdded = true;
            await db.Transactions.AddAsync(transactions);
            float added = db.Transactions.Where(x => x.User == transactions.User && x.IsAdded).Sum(x => x.AmountInUsd);
            float subtracted = db.Transactions.Where(x => x.User == transactions.User && !x.IsAdded).Sum(x => x.AmountInUsd);
            float balance = added - subtracted;
            var user = db.SiteUsers.SingleOrDefault(x => x.Id == transactions.User.Id);
            if (balance < 0)
            {
                logger.LogInformation("there is propblem in user payments userID is ---> " + transactions.User.Id.ToString());
                user.UserStatus = Models.DbModels.Enums.UserStatus.DeactivatedForTransactionIssue;
                db.SiteUsers.Update(user);
                result.Msssage = "error in this user transactions";
                result.Status = Models.Enums.Status.Faild;
            }
            else
            {

                user.Balance = balance;
                db.SiteUsers.Update(user);
                result.Msssage = "successful";
                result.Status = Models.Enums.Status.SuccessFul;
            }
            await db.SaveChangesAsync(true);
            return result;

        }
        //ok
        public async Task<AccountingAddTransaction> AddTransactionDecrease(Transactions transactions)
        {
            AccountingAddTransaction result = new();
            if(transactions.User.UserStatus != Models.DbModels.Enums.UserStatus.Active)
            {
                result.Status = Models.Enums.Status.Faild;
                result.Msssage = "this account is not allowd for this action";
                return result;
            }
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
        //ok
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
        // not ok
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
        //ok
        public async Task<AccountingIncreaseBalance> UserIncreaseBalanceRequest(Currency currency, float amount, SiteUser siteUser, string userWallet = "")
        {
            var result = new AccountingIncreaseBalance();
            if(siteUser.UserStatus != UserStatus.Active)
            {
                result.Message = "account is not Active";
                result.Error = true;
                return result;
            }
            Wallet wallet = new();
        
            ICollection<Wallet> wallets = db.Wallets.Where(x => x.IsAvailable && x.Currency == currency).ToList();
            //wallet = db.Wallets.Where(x => x.IsAvailable && x.Currency == currency).FirstOrDefault();
            if (wallets.Count > 0 )
            {
                result.Message = "There is no wallet right now to add try again later";
                result.Error = true;
                return result;
            }
            foreach (Wallet _wallet in wallets)
            {
                //TODO: get this from db 0.5f
                if (!db.WaitingWalletTransactions.Any(x => x.ReciveWallet == _wallet && !x.IsDeleted && amount - 0.5f < x.Amount && x.Amount < amount + 0.5f))
                {
                    wallet = _wallet;
                    break;
                }

            }
            if (wallet.Id <= 0)
            {
                result.Message = "There is no wallet right now to add try again later";
                result.Error = true;
                return result;
            }

            WaitingWalletTransaction waitingWallet = new()
            {
                Currency = currency,
                StartedTime = DateTime.Now,
                User = siteUser,
                ReciveWallet = wallet,
                WalletTransactionType = WalletTransactionType.UserDeposit,
                Amount = amount,
                IsDeleted = false
            };

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

        //just for depositing for users 
        //ok
        public async Task<ICollection<AccountingAddTransaction>> CheckingWaitingWalletTransaction()
        {
            ICollection<AccountingAddTransaction> resposes = new List<AccountingAddTransaction>();
            ICollection<WalletTransaction> walletTransactions = db.WalletTransactions.Where(x => !x.IsAddedToTransactions && x.IsConfirmed).ToList();
            ICollection<WaitingWalletTransaction> waitingWalletTransactions = db.WaitingWalletTransactions.Where(x => !x.IsDeleted).ToList();

            foreach(WalletTransaction walletTransaction in walletTransactions)
            {
                foreach(WaitingWalletTransaction waitingWalletTransaction in waitingWalletTransactions)
                {
                    if(walletTransaction.Wallet == waitingWalletTransaction.ReciveWallet &&
                        walletTransaction.WalletTransactionType == WalletTransactionType.UserDeposit &&
                        walletTransaction.TransactionDate >= waitingWalletTransaction.StartedTime.AddMinutes(10) &&
                        walletTransaction.TransactionDate <= waitingWalletTransaction.StartedTime &&
                        waitingWalletTransaction.Amount - 0.5f < walletTransaction.Ammount &&
                        walletTransaction.Ammount < waitingWalletTransaction.Amount + 0.5f)
                    {
                        bool isAdded = false;
                        TransactionType transactionType = TransactionType.Temp;
                        if (db.Wallets.Any(x => x.Id == walletTransaction.Wallet.Id) && walletTransaction.WalletTransactionType == WalletTransactionType.UserDeposit)
                        {
                            isAdded = true;
                            transactionType = TransactionType.CryptoDeposit;
                            Transactions transaction = new()
                            {
                                AmountInUsd = walletTransaction.ExchangeRateToUsd * walletTransaction.Ammount,
                                AmountOriginal = walletTransaction.Ammount,
                                Currency = walletTransaction.Currency,
                                Date = walletTransaction.TransactionDate,
                                IsAdded = isAdded,
                                TransactionType = transactionType,
                                User = waitingWalletTransaction.User
                            };
                            AccountingAddTransaction response = await AddTransactionIncrease(transaction);
                            if(response.Status == Status.SuccessFul)
                            {
                                waitingWalletTransaction.IsDeleted = true;
                                walletTransaction.IsAddedToTransactions = true;

                            }
                        }
                        //TODO: need to add some functions for whithdrow transactions
                    }
                }
            }

            return resposes;
        }
        //ok but not finished 
        public async Task<ICollection<WalletTransaction>> AddWalletTransactions(Currency currency )
        {
            ICollection<WalletTransaction> walletTransactions;
            //adding currency is nessecery because of the apis 
            ICollection<Wallet> wallets = db.Wallets.Where(x => x.Currency == currency).ToList();

            //check for each currency and add transaction
            if(currency == Currency.USDTerc20)
            {
                walletTransactions =await GetUSDTerc20Transactions(wallets);
            }
            else if(currency == Currency.USDTtrc20)
            {
                walletTransactions = await GetUSDTtrc20Transactions(wallets);
            }
            else if (currency == Currency.BUSDbep2)
            {
                walletTransactions = await GetBUSDbep2Transactions(wallets);
            }
            else
            {
                return null;
            }
            return walletTransactions;
        }
        
        //for USDT ERC20 get transactions and add to db  ok
        private async Task<ICollection<WalletTransaction>> GetUSDTerc20Transactions(ICollection<Wallet> wallets)
        {

            //TODO : need to set setting throw the db
            ICollection<WalletTransaction> walletTransactions = new List<WalletTransaction>();

            foreach(Wallet wallet in wallets)
            {

                //data that need to be past to api ofset need to be set by admin or something like that 
                string contractaddress = "0xdac17f958d2ee523a2206206994597c13d831ec7";
                string address = wallet.WalletAddress;
                string offset = "100";
                string apikey = "PBG73NEUGH6EKGCUZBTURPSY4TR69CVSW4";

                string url = $"https://api.etherscan.io/api?module=account&action=tokentx&contractaddress={contractaddress}&address={address}&page=1&offset={offset}&sort=asc&apikey={apikey}";

                //object should change to model of response


                EthJsonRespons results = new EthJsonRespons();


                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        char[] ar = { ' ', '\n' };
                        url = url.Trim(ar);
                        //var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
                        var response = await client.GetAsync(url.Trim());
                        if (response != null)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            results = JsonConvert.DeserializeObject<EthJsonRespons>(jsonString);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                }
                foreach (EthJsonRespons.TransactionsEth transactionsEth in results.result)
                {
                    WalletTransaction walletTransaction = new();
                    WalletTransactionType walletTransactionType = WalletTransactionType.UserWithdraw;
                    if (db.Wallets.Any(x => x.WalletAddress == transactionsEth.to))
                    {
                        walletTransaction.Wallet = db.Wallets.Where(x => x.WalletAddress == transactionsEth.to && x.Currency == Currency.USDTerc20).FirstOrDefault();
                        walletTransactionType = WalletTransactionType.UserDeposit;
                    }
                    else if (db.Wallets.Any(x => x.WalletAddress == transactionsEth.from))
                    {
                        walletTransaction.Wallet = db.Wallets.Where(x => x.WalletAddress == transactionsEth.from && x.Currency == Currency.USDTerc20).FirstOrDefault();
                        walletTransactionType = WalletTransactionType.UserWithdraw;
                    }
                    //check for duplicated transactions and invalid wallets
                    if (walletTransaction.Wallet != null && !db.WalletTransactions.Any(x => x.Hash == transactionsEth.hash) && walletTransactions.Any(x => x.Hash == transactionsEth.hash))
                    {
                        walletTransaction.Ammount = float.Parse(transactionsEth.value, CultureInfo.InvariantCulture.NumberFormat) / 1000000;
                        walletTransaction.Currency = Currency.USDTerc20;
                        //for other tokens and coins is has to be change and get from some kind of api
                        walletTransaction.ExchangeRateToUsd = 1;
                        walletTransaction.Hash = transactionsEth.hash;
                        //confirmation is 10 and need to be set by admin
                        walletTransaction.IsConfirmed = int.Parse(transactionsEth.confirmations) > 10 ? true : false;
                        walletTransaction.OtherAddress = transactionsEth.from;
                        walletTransaction.WalletTransactionType = walletTransactionType;
                        walletTransaction.TransactionDate = UnixTimeStampToDateTime(double.Parse(transactionsEth.timeStamp));
                        walletTransaction.IsAddedToTransactions = false;

                        walletTransactions.Add(walletTransaction);
                        db.WalletTransactions.Add(walletTransaction);
                    }

                }

            }


            //add all the valid transaction that been confirmed by 10 
            await db.SaveChangesAsync(true);
            //need to create walletTransaction for each object that is recived
            return walletTransactions;


        }

        private async Task<ICollection<WalletTransaction>> GetUSDTtrc20Transactions( ICollection<Wallet> wallets)
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

        private async Task<ICollection<WalletTransaction>> GetBUSDbep2Transactions( ICollection<Wallet> wallets)
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
        
        public async Task AddWallet(Wallet wallet)
        {
            await db.Wallets.AddAsync(wallet);
            await db.SaveChangesAsync(true);
        }

        private static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }

    }
}
