using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crypto_Payment_Gateway_MVC.Models.DbModels.Enums;
using Crypto_Payment_Gateway_MVC.Models.Enums;

namespace Crypto_Payment_Gateway_MVC.Models.DbModels
{
    public class SiteUser : IdentityUser
    {
        public string Name { get; set; }
        public string FamilyName { get; set; }
        public string PostCode { get; set; }
        public string Address { get; set; }
        public float Balance { get; set; }
        public UserStatus UserStatus { get; set; }
        public string CryptoAddress { get; set; }
        public Currency AddressCurrency { get; set; }
        public DateTime CryptoChangeDate { get; set; }




    }
}
