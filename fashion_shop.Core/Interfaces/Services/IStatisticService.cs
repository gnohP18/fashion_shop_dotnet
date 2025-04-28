using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.DTOs.Responses.Admin;

namespace fashion_shop.Core.Interfaces.Services;

public interface IStatisticService
{
    Task<List<TopSellerResponse>> GetTopSellerAsync(GetTopSellerRequest request);
    Task<List<SalesRevenueResponse>> GetSalesRevenueAsnc(GetSalesRevenueRequest request);
}