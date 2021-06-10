namespace Identity.Api.Application.Configuration
{
    public enum DbType
    {
        SqlServer,
        InMemory
    }
    
    public class DbConfig
    {
        public DbType Database { get; set; }
        public bool Create { get; set; }
        public bool Seed { get; set; }
    }
}