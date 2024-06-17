using Intelligent_AutoWms.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Intelligent_AutoWms.Model
{
    public class Intelligent_AutoWms_DbContext:DbContext
    {
        public DbSet<WMS_Users> Users { get; set; }

        public DbSet<WMS_Roles> Roles { get; set; }

        public DbSet<WMS_User_Role_RelationShips> _User_Role_RelationShips { get; set; }

        public DbSet<WMS_WareHouse> WareHouses { get; set; }

        public DbSet<WMS_Area> Areas { get; set; }

        public DbSet<WMS_Shelf> Shelves { get; set; }

        public DbSet<WMS_Location> Locations { get; set; }

        public DbSet<WMS_Port> Ports { get; set; }

        public DbSet<WMS_Receipt_Orders> Receipt_Orders { get; set; }

        public DbSet<WMS_Delivery_Orders> Delivery_Orders { get; set; }

        public DbSet<WMS_Task> WMS_Tasks { get; set; }

        public DbSet<WMS_Operate_Log> Operate_Logs { get; set; }

        public DbSet<WMS_Inventory> Inventories { get; set; }

        public DbSet<WMS_Permission> WMS_Permissions { get; set; }


        public Intelligent_AutoWms_DbContext(DbContextOptions<Intelligent_AutoWms_DbContext> options) : base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
