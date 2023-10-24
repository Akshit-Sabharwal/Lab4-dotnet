namespace Lab4.Data
{
    public static class DbInitializer
    {
        public static void Initialize(SportsDbContext context)
        {
            context.Database.EnsureCreated();
        }
    }

}
