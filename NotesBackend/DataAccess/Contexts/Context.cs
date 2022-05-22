using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Contexts
{
    public class Context : DbContext, IContext
    {
        public Context(DbContextOptions<Context> options) 
            : base(options) { }

        public DbSet<UserRto> Users { get; set; }
        public DbSet<NoteRto> Notes { get; set; }
    }
}
