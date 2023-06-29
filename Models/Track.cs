#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
namespace SongSmith.Models;
public class Track
{
  [Key]
  public int TrackId { get; set; }


  [Required]
  [MinLength(2, ErrorMessage = "Name must be at least 2 characters.")]
  [MaxLength(30, ErrorMessage = "Name must be at most 30 characters.")]
  public string Name { get; set; }


  [Required]
  public string Instrument { get; set; }


  [Required]
  public byte[] Notes { get; set; }


  [Required]
  public int Rhythm { get; set; }


  public string Description { get; set; }


  public DateTime CreatedAt { get; set; } = DateTime.Now;
  public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
