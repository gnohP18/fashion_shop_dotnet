using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.DTOs.Responses.User
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int TotalAmount { get; set; }
        public string? Note { get; set; }
        public string? CreatedAt { get; set; }
    }
}