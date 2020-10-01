using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMCTest.ViewModels
{
    public class OrderVM
    {
        public decimal SalesTax { get; set; }
        public decimal Shipping { get; set; }
        public decimal Amount { get; set; }
        public string ToStreet { get; set; }
        public string ToCity { get; set; }
        public string ToState { get; set; }
        public string ToZip { get; set; }
        public string ToCountry { get; set; }
        public string FromStreet { get; set; }
        public string FromCity { get; set; }
        public string FromState { get; set; }
        public string FromZip { get; set; }
        public string FromCountry { get; set; }
        public string ExemptionType { get; set; }
        public string Provider { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionId { get; set; }
        public string CustomerId { get; set; }
    }
}
