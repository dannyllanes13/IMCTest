using AutoMapper;
using IMCTest.Service.Interface;
using IMCTest.Service.Validators;
using IMCTest.ViewModels;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Taxjar;

namespace IMCTest.Service.Implementation
{
    public class TaxCalculatorClientB : ITaxCalculator
    {
        private readonly string apiKey;
        private readonly IMapper _mapper;
        private const string Url = "https://api.taxjar.com/v2/";

        public TaxCalculatorClientB(IMapper mapper, IConfiguration config)
        {
            apiKey = config["ApiKey"];
            _mapper = mapper;
        }

        public async Task<decimal> GetTaxRateForLocation(AddressVM address)
        {
            if (string.IsNullOrEmpty(address.Zip))
            {
                throw new InvalidOperationException("ZipCode is a required data");
            }

            if (address.Country.ToUpper() == "US" && !RegEx.isValidZipCode(address.Zip))
            {
                throw new InvalidOperationException("ZipCode data is not a valid US ZipCode.");
            }

            if (!string.IsNullOrEmpty(address.Country))
            {
                if (!RegEx.IsValidCountry(address.Country.ToUpper()))
                {
                    throw new InvalidOperationException("Country code is not a valid value.");
                }
            }

            var addressApi = _mapper.Map<Address>(address);

            var client = new RestClient(Url) { Authenticator = new JwtAuthenticator(this.apiKey) };

            var request =
                    new RestRequest($"rates/{address.Zip}")
                    {
                        Method = Method.GET,
                        RequestFormat = DataFormat.Json
                    };

            if (!string.IsNullOrEmpty(address.City))
            {
                request.AddQueryParameter("city", address.City);
            }

            if (!string.IsNullOrEmpty(address.State))
            {
                request.AddQueryParameter("state", address.State);
            }

            if (!string.IsNullOrEmpty(address.Country))
            {
                request.AddQueryParameter("country", address.Country);
            }

            if (!string.IsNullOrEmpty(address.Street))
            {
                request.AddQueryParameter("street", address.Street);
            }

            var response = await client.ExecuteGetAsync(request);

            if (response.ResponseStatus != ResponseStatus.Completed || response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("Something was wrong on the server");
            }                        

            var responseData = JsonConvert.DeserializeObject<RateResponseAttributes>(response.Content);

            return responseData.CombinedRate;
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

            var client = new RestClient(Url) { Authenticator = new JwtAuthenticator(this.apiKey) };
            var request =
                new RestRequest("taxes")
                {
                    Method = Method.POST,
                    RequestFormat = DataFormat.Json
                };
            request.AddJsonBody(JsonConvert.SerializeObject(orderApi));
            var response = await client.ExecutePostAsync(request);

            if (response.ResponseStatus != ResponseStatus.Completed || response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("Something was wrong on the server");
            }

            var responseData = JsonConvert.DeserializeObject<TaxResponseAttributes>(response.Content);

            return responseData.AmountToCollect;
        }
    }
}
