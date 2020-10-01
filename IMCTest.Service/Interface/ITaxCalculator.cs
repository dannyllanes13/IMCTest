using IMCTest.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMCTest.Service.Interface
{
    public interface ITaxCalculator
    {
        Task<decimal> GetTaxRateForLocation(AddressVM address);

        Task<decimal> GetTaxForOrder(OrderVM order);
    }
}
