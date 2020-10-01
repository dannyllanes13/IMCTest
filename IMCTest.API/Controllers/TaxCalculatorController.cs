using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IMCTest.Service.Interface;
using IMCTest.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Services.Remoting;
using RestSharp.Extensions;
using static IMCTest.API.Startup;

namespace IMCTest.API.Controllers
{
    [ApiController]
    public class TaxCalculatorController : CustomBaseController
    {
        
        private readonly ServiceResolver _serviceRsolver;

        public TaxCalculatorController(ILogger<TaxCalculatorController> logger, ServiceResolver serviceAccesor): base(logger)
        {
            _serviceRsolver = serviceAccesor;
        }

        [HttpGet]
        [Route("api/taxCalculator/rates/{zipCode}/{country}")]
        public async Task<IActionResult> GetRateByLocation(string zipCode, string country)
        {
            try
            {
                //I use header to pass clientId because on client can set an interceptor or something similar
                //to pass its client id once
                var clientId = GetClientIdFromHeader(Request);
                
                var _taxCalculatorService = _serviceRsolver(clientId);
                var address = new AddressVM
                {
                    Zip = zipCode,
                    Country = country
                };

                var tax = await _taxCalculatorService.GetTaxRateForLocation(address);

                return Ok(tax);
            }
            catch(InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

        }

        [HttpPost]
        [Route("api/taxCalculator/order/calculate")]
        public async Task<IActionResult> CalculateTaxByOrder(OrderVM order)
        {
            try
            {
                var clientId = GetClientIdFromHeader(Request);
                var _taxCalculatorService = _serviceRsolver(clientId);

                var tax = await _taxCalculatorService.GetTaxForOrder(order);

                return Ok(tax);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

        }
    }
}
