using Microsoft.EntityFrameworkCore;
using YumeKodo.Models;


namespace YumeKodo.Data;
public class DataContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<Comment> Comments { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Data Source=(localdb)\ProjectModels;Initial Catalog=YumeKodo;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
    }
}
