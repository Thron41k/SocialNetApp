using SocialNetApp.Data.Repository.Interfaces;

namespace SocialNetApp.Data.UoW.Interfaces;

public interface IUnitOfWork : IDisposable
{
    int SaveChanges(bool ensureAutoHistory = false);

    IRepository<TEntity> GetRepository<TEntity>(bool hasCustomRepository = true) where TEntity : class;
}