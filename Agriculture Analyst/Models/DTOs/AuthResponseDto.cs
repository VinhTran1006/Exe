namespace Agriculture_Analyst.Models.DTOs
{
    public class AuthResponseDto
    {
     public bool Success { get; set; }
        public string Message { get; set; } = null!;
   public UserDto? User { get; set; }
    }

 public class UserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
      public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
     public string? Phone { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
     public string? Address { get; set; }
        public bool IsActive { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
