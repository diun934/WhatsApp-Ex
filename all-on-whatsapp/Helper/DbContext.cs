using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static all_on_whatsapp.Model.DbModel;

namespace all_on_whatsapp
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }

        private readonly string _connectionString;

        // 构造函数接收 connectionString 参数
        public AppDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if (!optionsBuilder.IsConfigured)
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    AppNotice.Show("警告", "未配置有效的连接字符串！", AppNotificationLevel.Warning);
                    return;
                }
                Debug.WriteLine(_connectionString);
                optionsBuilder.UseMySql(_connectionString, new MySqlServerVersion(new Version(5, 7, 44)));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 可以在这里配置实体的映射
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Order>().ToTable("Orders");
        }
    }
}
