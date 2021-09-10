using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Crypto_Payment_Gateway_MVC.Models.Enums;

namespace Crypto_Payment_Gateway_MVC.Models.DbModels
{
    public class Transactions
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public float AmountInUsd { get; set; }
        public SiteUser User { get; set; }
        public bool IsAdded { get; set; }
        public TransactionType TransactionType { get; set; }
        public Currency Currency { get; set; }
        public float AmountOriginal { get; set; }


    }
}
