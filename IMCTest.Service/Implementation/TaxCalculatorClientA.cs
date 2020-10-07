using AutoMapper;
using IMCTest.Service.Interface;
using IMCTest.Service.Validators;
using IMCTest.ViewModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Taxjar;

namespace IMCTest.Service.Implementation
{
    public class TaxCalculatorClientA : ITaxCalculator
    {
        private readonly TaxjarApi _taxJarClient;
        private readonly IMapper _mapper;

        public TaxCalculatorClientA(IMapper mapper, IConfiguration config)
        {
            _taxJarClient = new TaxjarApi(config["ApiKey"]);
            _mapper = mapper;
        }

        public async Task<decimal> GetTaxRateForLocation(AddressVM address)
        {
            if (string.IsNullOrEmpty(address.Zip))
            {
                throw new InvalidOperationException("ZipCode is a required data");
            }

            if (!string.IsNullOrEmpty(address.Country))
            {
                if (!RegEx.IsValidCountry(address.Country.ToUpper()))
                {
                    throw new InvalidOperationException("Country code is not a valid value.");
                }
            }

            if (!RegEx.isValidZipCode(address.Zip))
            {
                throw new InvalidOperationException("ZipCode data is not a valid US ZipCode.");
            }

            var addressApi = _mapper.Map<Address>(address);
            var response = await this._taxJarClient.RatesForLocationAsync(addressApi.Zip, addressApi);
            return response.CombinedRate;
        }

        public async Task<decimal> GetTaxForOrder(OrderVM order)
        {
            if (string.IsNullOrEmpty(order.ToCountry) || order.Shipping < 0)
            {
                throw new InvalidOperationException("ZipCode is a required data");
            }

            if (!string.IsNullOrEmpty(order.ToCountry))
            {

                if (!RegEx.IsValidCountry(order.ToCountry.ToUpper()))
                {
                    throw new InvalidOperationException("Destination country is not a valid Code");
                }

                if (order.ToCountry.ToLower() == "us" && string.IsNullOrEmpty(order.ToZip))
                {
                    throw new InvalidOperationException("ZipCode data is required when destination country is US");
                }

                if (order.ToCountry.ToLower() == "us" && !RegEx.isValidZipCode(order.ToZip))
                {
                    throw new InvalidOperationException("ZipCode data is not a valid US ZipCode");
                }

                if ((order.ToCountry.ToLower() == "us" || order.ToCountry.ToLower() == "ca") && string.IsNullOrEmpty(order.ToState))
                {
                    throw new InvalidOperationException("Destination state is required whe Destination country is US or CA");
                }
            }

            var orderApi = _mapper.Map<Order>(order);
            var response = await _taxJarClient.TaxForOrderAsync(orderApi);

            return response.AmountToCollect;
        }
    }
}
