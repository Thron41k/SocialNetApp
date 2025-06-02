using Microsoft.EntityFrameworkCore.Infrastructure;
using SocialNetApp.Data.Repository;
using SocialNetApp.Data.Repository.Interfaces;
using SocialNetApp.Data.UoW.Interfaces;

namespace SocialNetApp.Data.UoW;

public class UnitOfWork(ApplicationDbContext app) : IUnitOfWork
{
    private readonly Dictionary<Type, object> _repositories = new();

    public void Dispose()
    {

    }

    public IRepository<TEntity> GetRepository<TEntity>(bool hasCustomRepository = true) where TEntity : class
    {
        if (hasCustomRepository)
        {
            var customRepo = app.GetService<IRepository<TEntity>>();
            if (customRepo != null)
            {
                return customRepo;
            }
        }

        var type = typeof(TEntity);
        if (!_repositories.ContainsKey(type))
        {
            _repositories[type] = new Repository<TEntity>(app);
        }

        return (IRepository<TEntity>)_repositories[type];

    }
    public int SaveChanges(bool ensureAutoHistory = false)
    {
        throw new NotImplementedException();
    }
}