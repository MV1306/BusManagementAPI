using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BusManagementAPI.Data
{
    public class BusDbContext : DbContext
    {
        public BusDbContext(DbContextOptions<BusDbContext> options) : base(options) { }
        public DbSet<Busroute> BusRoutes { get; set; }
        public DbSet<BusRouteStage> BusRouteStages { get; set; }
        public DbSet<FareMaster> FareMasters { get; set; }
        public DbSet<FareMaster_New> FareMasters_New { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<StageTranslation> StageTranslations { get; set; }

    }
}