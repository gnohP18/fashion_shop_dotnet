using System.Linq.Expressions;
using fashion_shop.Core.Entities;

namespace fashion_shop.Core.Interfaces.Repositories;

public interface IProductItemRepository : IRepository<ProductItem>
{
    void UpdateManySelective(List<ProductItem> entities, params Expression<Func<ProductItem, object>>[] updatedProperties);
}