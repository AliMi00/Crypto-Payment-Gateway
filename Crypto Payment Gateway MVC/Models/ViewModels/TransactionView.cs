using Crypto_Payment_Gateway_MVC.Models.DbModels;
using Crypto_Payment_Gateway_MVC.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Crypto_Payment_Gateway_MVC.Models.ViewModels
{
    public class TransactionView
    {
        [Display(Name = "Transaction Number")]
        public string Id { get; set; }

        [Display(Name ="Date")]
        public DateTime Date { get; set; }

        [Display(Name = "Amount in USD")]
        public float AmountInUsd { get; set; }

        [Display(Name = "Transaction Type")]
        public TransactionType TransactionType { get; set; }

        [Display(Name = "Currency of Transaction")]
        public Currency Currency { get; set; }

        [Display(Name = "Currency Amount")]
        public float AmountOriginal { get; set; }

    }
}
