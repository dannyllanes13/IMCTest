using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IMCTest.API.Controllers
{
    public class CustomBaseController : ControllerBase
    {
        private readonly ILogger<TaxCalculatorController> _logger;

        public CustomBaseController(ILogger<TaxCalculatorController> logger)
        {
            _logger = logger;
        }

        protected string GetClientIdFromHeader(HttpRequest request)
        {
            if (request.Headers.ContainsKey("ClientId"))
            {
                return request.Headers["ClientId"];
            }

            return null;
        }
    }
}
