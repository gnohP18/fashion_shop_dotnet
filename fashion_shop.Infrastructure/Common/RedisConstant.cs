using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace fashion_shop.Infrastructure.Common
{
    public class RedisConstant
    {
        /// <summary>
        /// Number of user allow Redis
        /// </summary>
        public const uint MaxAllowUser = 1_000_000;

        public static RedisKey CATEGORY_LIST = "CATEGORY:slug";
    }
}