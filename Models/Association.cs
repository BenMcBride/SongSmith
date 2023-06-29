#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
namespace SongSmith.Models;
public class Association
{
  [Key]
  public int AssociationId { get; set; }
  public int SongId { get; set; }
  public int TrackId { get; set; }
  public Song? Song { get; set; }
  public Track? Track { get; set; }
}
