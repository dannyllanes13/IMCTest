using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Taxjar;

namespace IMCTest.UnitTest
{
    public class TaxRateTest
    {
        private TaxjarApi _client;

        [SetUp]
        public void Setup()
        {
            _client = new TaxjarApi("5da2f821eee4035db4771edab942a4cc");
        }    

        [Test]
        public async Task GetRate()
        {
            var response = await _client.RatesForLocationAsync("33012");

            Assert.AreEqual("HIALEAH", response.City);
            Assert.AreEqual("MIAMI-DADE", response.County);
            Assert.AreEqual("FL", response.State);
            Assert.AreEqual(0.06, response.StateRate);
            Assert.AreEqual(0.01, response.CountyRate);
            Assert.AreEqual(0.07, response.CombinedRate);
        }

        [Test]
        public void GettingInvalidParameters()
        {
            var ex = Assert.ThrowsAsync<TaxjarException>(async () => await _client.RatesForLocationAsync(""));
            Assert.That(ex.HttpStatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }       
    }
}
