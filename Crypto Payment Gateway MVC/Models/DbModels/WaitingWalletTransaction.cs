using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Crypto_Payment_Gateway_MVC.Models.Enums;

namespace Crypto_Payment_Gateway_MVC.Models.DbModels
{
    public class WaitingWalletTransaction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public SiteUser User { get; set; }
        public DateTime StartedTime { get; set; }
        public Wallet ReciveWallet { get; set; }
        public string UserWallet { get; set; }
        public Currency Currency { get; set; }
        public WalletTransactionType WalletTransactionType { get; set; }
        public float Amount { get; set; }
        public string Hash { get; set; }
        public bool IsDeleted { get; set; }






    }
}
