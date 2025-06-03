using Microsoft.EntityFrameworkCore;
using SocialNetApp.Data.Repository.Interfaces;

namespace SocialNetApp.Data.Repository;

public class Repository<T> : IRepository<T>
    where T : class
{
    private readonly DbContext _db;

    protected DbSet<T> Set
    {
        get;
    }

    public Repository(ApplicationDbContext db)
    {
        _db = db;
        var set = _db.Set<T>();
        set.Load();

        Set = set;
    }


    public async Task Create(T item)
    {
        await Set.AddAsync(item);
        await _db.SaveChangesAsync();
    }

    public async Task Delete(T item)
    {
        Set.Remove(item);
        await _db.SaveChangesAsync();
    }

    public async Task<T?> Get(int id)
    {
        return await Set.FindAsync(id);
    }

    public IEnumerable<T> GetAll()
    {
        return Set;
    }

    public async Task Update(T item)
    {
        Set.Update(item);
        await _db.SaveChangesAsync();
    }
}