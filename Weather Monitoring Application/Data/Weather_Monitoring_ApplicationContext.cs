using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Weather_Monitoring_Application.Models;

namespace Weather_Monitoring_Application.Data
{
    public class Weather_Monitoring_ApplicationContext : DbContext
    {
        public Weather_Monitoring_ApplicationContext (DbContextOptions<Weather_Monitoring_ApplicationContext> options)
            : base(options)
        {
        }

        public DbSet<Weather_Monitoring_Application.Models.Location> Location { get; set; } = default!;
    }
}
