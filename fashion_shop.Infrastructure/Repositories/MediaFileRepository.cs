using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Interfaces.Repositories;
using fashion_shop.Infrastructure.Database;

namespace fashion_shop.Infrastructure.Repositories;

public class MediaFileRepository : IMediaFileRepository
{
    private readonly ApplicationDbContext _dbContext;

    public MediaFileRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public IQueryable<MediaFile> Queryable => _dbContext.MediaFiles.AsQueryable();

    public IUnitOfWork UnitOfWork => _dbContext;

    public async Task AddAsync(MediaFile entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.MediaFiles.AddAsync(entity, cancellationToken);
    }

    public void Delete(MediaFile entity)
    {
        _dbContext.MediaFiles.Remove(entity);
    }

    public void DeleteByIds(IEnumerable<int> ids)
    {
        var dataDelete = _dbContext.MediaFiles.Where(x => ids.Contains(x.Id)).ToList();

        _dbContext.MediaFiles.RemoveRange(dataDelete);
    }

    public void DeleteMany(List<MediaFile> entities)
    {
        throw new NotImplementedException();
    }

    public void Update(MediaFile entity)
    {
        _dbContext.MediaFiles.Update(entity);
    }
}