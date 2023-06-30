// #pragma warning disable CS8618
// #pragma warning disable CS8600
// #pragma warning disable CS8602
// using System.ComponentModel.DataAnnotations;
// using Microsoft.AspNetCore.Identity;
// namespace SongSmith.Models;
// public class User : IdentityUser
// {
//   [Key]
//   public int UserId { get; set; }

//   [Required]
//   [MinLength(2, ErrorMessage = "UserName must be at least 2 characters.")]
//   [MaxLength(20, ErrorMessage = "UserName must be at most 20 characters.")]
//   public override string? UserName { get; set; }
//   public DateTime CreatedAt { get; set; } = DateTime.Now;
//   public DateTime UpdatedAt { get; set; } = DateTime.Now;
// }