using Journal.Domain.AggregatesModel.JobAggregate;
using Microsoft.EntityFrameworkCore;

namespace Journal.Infrastructure
{    
    public class FulfillmentContext : DbContext
    {
        public DbSet<Fulfillment> Fulfillments { get; set; }

        public FulfillmentContext(DbContextOptions<FulfillmentContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
