namespace CitroenAPI.Models.DbContextModels
{
    public interface IDbContextFactory
    {
        CitroenDbContext CreateDbContext();
    }
}
