#pragma warning disable CS8618
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
namespace SongSmith.Models;
public class MyContext : DbContext
{
  public MyContext(DbContextOptions options) : base(options) { }
  public DbSet<User> Users { get; set; }
  public DbSet<Association> Associations { get; set; }
  public DbSet<Note> Notes { get; set; }
  public DbSet<Song> Songs { get; set; }
  public DbSet<Track> Tracks { get; set; }
  public DbSet<Algorithm> Algorithms { get; set; }
}
