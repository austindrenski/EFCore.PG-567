using System;
using Microsoft.EntityFrameworkCore;

namespace testchar
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            using (var ctx = new TestCharContext(args[0]))
            {
                ctx.Database.EnsureDeleted();
                ctx.Database.EnsureCreated();

                ctx.TableChar.Add(new TableChar { CharIdent = "12345678" });
                ctx.TableChar.Add(new TableChar { CharIdent = "123456  " });
                ctx.SaveChanges();

                var m1 = ctx.TableChar.Find("12345678");
                m1.CharValue = "This will update";
                ctx.SaveChanges();
                Console.WriteLine($"Updated: {m1.CharIdent} - {m1.CharValue}");

                var m2 = ctx.TableChar.Find("123456  ");
                m2.CharValue = "This will NOT update";
                ctx.SaveChanges();
                Console.WriteLine($"Updated: {m2.CharIdent} - {m2.CharValue}");

                Console.WriteLine("Success!");
            }
        }
    }

    public class TestCharContext : DbContext
    {
        private readonly string _connection;
        public DbSet<TableChar> TableChar { get; set; }

        public TestCharContext(string connection) => _connection = connection;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(_connection);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<TableChar>(
                entity =>
                {
                    entity.HasKey(e => e.CharIdent);
                    entity.Property(e => e.CharIdent).HasColumnType("character(8)");
                });
    }

    public class TableChar
    {
        public string CharIdent { get; set; }
        public string CharValue { get; set; }
    }
}