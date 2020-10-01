using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Taxjar;

namespace IMCTest.UnitTest
{
    public class TaxOrderTest
    {
        private TaxjarApi _client;

        [SetUp]
        public void Setup()
        {
            _client = new TaxjarApi("5da2f821eee4035db4771edab942a4cc");
        }
                
        [Test]
        public async Task TaxForOrder()
        {
            var orderRequest = new Order
            {
                FromCountry = "US",
                FromZip = "92093",
                FromState = "CA",
                ToCountry = "US",
                ToZip = "90002",
                ToState = "CA",
                Amount = 270,
                Shipping = 7.5m,
            };
            var response = await _client.TaxForOrderAsync(orderRequest);
            Assert.AreEqual(27.68, response.AmountToCollect);
        }

        [Test]
        public void InvalidParameters()
        {
            var ex = Assert.ThrowsAsync<TaxjarException>(async () => await _client.TaxForOrderAsync(new Order()));
            Assert.That(ex.HttpStatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }
    }
}
