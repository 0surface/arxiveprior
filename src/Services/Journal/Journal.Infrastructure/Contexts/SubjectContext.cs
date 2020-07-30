using Journal.Domain.AggregatesModel.SubjectAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journal.Infrastructure.Contexts
{
    public  interface ISubjectContext
    {
        DbSet<Subject> Subjects { get; set; }
    }
    public class SubjectContext : DbContext, ISubjectContext
    {
        private IConfiguration Configuration { get; set; }
        public DbSet<Subject> Subjects { get; set; }

        public SubjectContext(IConfiguration configuration)
        {

            Configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = Configuration["ConnectionString"];
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
