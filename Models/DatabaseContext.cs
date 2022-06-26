using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace books_api.Models
{
    public class DatabaseContext : DbContext
    {
        public DbSet<BookModel> Books {get; set;}
        public DbSet<AuthorModel> Authors {get; set;}
        public DatabaseContext(DbContextOptions<DatabaseContext> options):base(options){}
    }
}