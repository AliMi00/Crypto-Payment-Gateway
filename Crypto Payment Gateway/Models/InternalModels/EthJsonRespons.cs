using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crypto_Payment_Gateway.Models.InternalModels
{
    public class EthJsonRespons
    {
        public string status { get; set; }
        public string message { get; set; }
        public List<TransactionsEth> result { get; set; }

        public class TransactionsEth
        {
            public string blockNumber { get; set; }
            //unix timestamp 
            public string timeStamp { get; set; }
            public string hash { get; set; }
            public string nonce { get; set; }
            public string blockHash { get; set; }
            public string from { get; set; }
            public string contractAddress { get; set; }
            public string to { get; set; }
            // *1000000 time of usdt
            public string value { get; set; }
            public string tokenName { get; set; }
            public string tokenSymbol { get; set; }
            public string tokenDecimal { get; set; }
            public string transactionIndex { get; set; }
            public string gas { get; set; }
            public string gasPrice { get; set; }
            public string gasUsed { get; set; }
            public string cumulativeGasUsed { get; set; }
            public string input { get; set; }
            // nomber of blocked since then 
            public string confirmations { get; set; }


        }
    }
}
