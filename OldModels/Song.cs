// #pragma warning disable CS8618
// using System.ComponentModel.DataAnnotations;
// namespace SongSmith.Models;
// public class Song
// {
//   [Key]
//   public int SongId { get; set; }


//   [Required]
//   [MinLength(2, ErrorMessage = "Title must be at least 2 characters.")]
//   [MaxLength(30, ErrorMessage = "Title must be at most 30 characters.")]
//   public string Title { get; set; }


//   [Required]
//   public string Genre { get; set; }


//   [Required]
//   public int Tempo { get; set; } // in BPM


//   [Required]
//   public int Duration { get; set; } // in seconds


//   [Required]
//   public string Key { get; set; }


//   [Required]
//   public string Description { get; set; }


//   public int UserId { get; set; }
//   public User? Creator { get; set; }


//   public DateTime CreatedAt { get; set; } = DateTime.Now;
//   public DateTime UpdatedAt { get; set; } = DateTime.Now;
// }
