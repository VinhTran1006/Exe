namespace Agriculture_Analyst.Models.DTOs
{
    public class SignUpRequestDto
  {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
   public string? Phone { get; set; }
        public string? Gender { get; set; }
public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
 public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
  }
}
