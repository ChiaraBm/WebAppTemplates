using Microsoft.EntityFrameworkCore;

namespace WebAppTemplate.Api.Database;

public class DatabaseRepository<T> where T : class
{
    private readonly DataContext DataContext;
    private readonly DbSet<T> Set;

    public DatabaseRepository(DataContext dataContext)
    {
        DataContext = dataContext;
        Set = DataContext.Set<T>();
    }

    public IQueryable<T> Query() => Set;

    public async Task<T> AddAsync(T entity)
    {
        var final = Set.Add(entity);
        await DataContext.SaveChangesAsync();
        return final.Entity;
    }

    public async Task UpdateAsync(T entity)
    {
        Set.Update(entity);
        await DataContext.SaveChangesAsync();
    }

    public async Task RemoveAsync(T entity)
    {
        Set.Remove(entity);
        await DataContext.SaveChangesAsync();
    }
}