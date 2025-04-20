using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.Entities;

namespace fashion_shop.Core.Interfaces.Repositories;

public interface ISettingRepository : IRepository<Setting>
{
    Task AddManyAsync(List<Setting> entities);
}