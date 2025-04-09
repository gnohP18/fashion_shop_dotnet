using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace fashion_shop.Core.Entities;
public class User : IdentityUser<int>
{
    public virtual HashSet<Order> Orders { get; set; } = new HashSet<Order>();
}