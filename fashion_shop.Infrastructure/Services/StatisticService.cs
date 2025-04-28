using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.DTOs.Common;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.DTOs.Responses.Admin;
using fashion_shop.Core.Interfaces.Repositories;
using fashion_shop.Core.Interfaces.Services;
using fashion_shop.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Minio.DataModel;

namespace fashion_shop.Infrastructure.Services;

public class StatisticService : IStatisticService
{
    private readonly IOrderDetailRepository _orderDetailRepository;
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly MinioSettings _minioSettings;

    public StatisticService(
        IOrderDetailRepository orderDetailRepository,
        IProductRepository productRepository,
        IOptions<MinioSettings> options,
        IOrderRepository orderRepository)
    {
        _orderDetailRepository = orderDetailRepository ?? throw new ArgumentNullException(nameof(orderDetailRepository));
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _minioSettings = options.Value ?? throw new ArgumentNullException(nameof(options.Value));
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    }

    public async Task<List<TopSellerResponse>> GetTopSellerAsync(GetTopSellerRequest request)
    {
        var query = _orderDetailRepository
            .Queryable
            .AsNoTracking();

        if (request.FromDate is not null)
        {
            var fromDateOffset = new DateTimeOffset(request.FromDate.Value, TimeSpan.Zero);
            query = query.Where(od => od.CreatedAt >= fromDateOffset);
        }

        if (request.ToDate is not null)
        {
            var toDateOffset = new DateTimeOffset(request.ToDate.Value, TimeSpan.Zero);
            query = query.Where(od => od.CreatedAt <= toDateOffset);
        }

        var dataProduct = await query
            .Include(i => i.ProductItem)
            .ThenInclude(p => p.Product)
            .ThenInclude(p => p.Category)
            .GroupBy(o => new
            {
                o.ProductItem.ProductId,
                o.Product.Name,
                o.Product.ImageUrl
            })
            .Select(g => new TopSellerResponse
            {
                Id = g.Key.ProductId,
                Name = g.Key.Name,
                ImageUrl = !string.IsNullOrWhiteSpace(g.Key.ImageUrl) ?
                    _minioSettings.GetUrlImage(g.Key.ImageUrl, true, false) : ProductConstant.DefaultImage200,
                TotalQuantitySold = g.Sum(x => x.Quantity)
            })
            .OrderByDescending(x => x.TotalQuantitySold)
            .Take(request.numberOfProduct)
            .ToListAsync();

        return dataProduct;
    }


    public async Task<List<SalesRevenueResponse>> GetSalesRevenueAsnc(GetSalesRevenueRequest request)
    {
        // 1. Hàm Group Key
        // 2. Dùng Group Key
        // 3. Tạo danh sách thời gian đầy đủ (kể cả ngày không có đơn)
        // 4. Ghép dữ liệu

        var query = _orderRepository
            .Queryable
            .Include(o => o.OrderDetails)
            .AsNoTracking();

        var now = DateTimeOffset.UtcNow;

        var fromDate = request.Mode switch
        {
            Core.Common.StatisticEnum.Week => now.AddDays(-7),
            Core.Common.StatisticEnum.Month => now.AddMonths(-1),
            Core.Common.StatisticEnum.Quarter => now.AddMonths(-3),
            Core.Common.StatisticEnum.HalfAYear => now.AddMonths(-6),
            Core.Common.StatisticEnum.Year => now.AddYears(-1),
            _ => now.AddDays(-7)
        };

        query = query.Where(c => c.CreatedAt >= fromDate);

        var grouped = query
            .AsEnumerable()
            .GroupBy(od => GetGroupKey(od.CreatedAt, request.Mode))
            .Select(g => new
            {
                Key = g.Key,
                TotalAmount = g.Sum(x => x.TotalAmount),
                TotalItem = g.SelectMany(x => x.OrderDetails).Sum(od => od.Quantity)
            })
            .OrderBy(x => x.Key)
            .ToList();

        foreach (var item in grouped)
        {
            System.Console.WriteLine(item.TotalItem);
        }

        // 3. Tạo danh sách thời gian đầy đủ (kể cả ngày không có đơn)
        List<DateTime> dateRange = new();
        var tempDate = GetGroupKey(fromDate, request.Mode);
        var endDate = GetGroupKey(now, request.Mode);

        while (tempDate <= endDate)
        {
            tempDate = request.Mode switch
            {
                Core.Common.StatisticEnum.Week => tempDate.AddDays(1),
                Core.Common.StatisticEnum.Month => tempDate.AddDays(1),
                Core.Common.StatisticEnum.Quarter => tempDate.AddDays(7),
                Core.Common.StatisticEnum.HalfAYear => tempDate.AddDays(7),
                Core.Common.StatisticEnum.Year => tempDate.AddMonths(1),
                _ => tempDate.AddDays(1)
            };

            dateRange.Add(tempDate);
        }

        // 4. Ghép dữ liệu
        var result = dateRange
            .Select(date =>
            {
                var found = grouped.FirstOrDefault(x => x.Key == date);
                return new SalesRevenueResponse
                {
                    Label = request.Mode switch
                    {
                        Core.Common.StatisticEnum.Week => date.ToString("yyyy-MM-dd"),
                        Core.Common.StatisticEnum.Month => date.ToString("yyyy-MM-dd"),
                        Core.Common.StatisticEnum.Quarter => "Week of " + date.ToString("yyyy-MM-dd"),
                        Core.Common.StatisticEnum.HalfAYear => "Week of " + date.ToString("yyyy-MM-dd"),
                        Core.Common.StatisticEnum.Year => date.ToString("yyyy-MM"),
                        _ => date.ToString("yyyy-MM-dd")
                    },
                    TotalAmount = found?.TotalAmount ?? 0,
                    TotalItem = found?.TotalItem ?? 0
                };
            })
            .ToList();
        await Task.CompletedTask;

        return result;

    }

    private DateTime GetGroupKey(DateTimeOffset createdAt, Core.Common.StatisticEnum mode)
    {
        var date = createdAt.UtcDateTime.Date;

        return mode switch
        {
            Core.Common.StatisticEnum.Week => date,
            Core.Common.StatisticEnum.Month => date,
            Core.Common.StatisticEnum.Quarter => date.AddDays(-(int)date.DayOfWeek + (int)DayOfWeek.Monday),
            Core.Common.StatisticEnum.HalfAYear => date.AddDays(-(int)date.DayOfWeek + (int)DayOfWeek.Monday),
            Core.Common.StatisticEnum.Year => new DateTime(date.Year, date.Month, 1),
            _ => date
        };
    }

}