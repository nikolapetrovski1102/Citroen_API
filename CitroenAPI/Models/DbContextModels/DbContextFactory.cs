namespace CitroenAPI.Models.DbContextModels
{
    public class DbContextFactory : IDbContextFactory
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DbContextFactory(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public CitroenDbContext CreateDbContext()
        {
            var scope = _scopeFactory.CreateScope();
            return scope.ServiceProvider.GetRequiredService<CitroenDbContext>();
        }
    }
}
