using Agriculture_Analyst.Models.DTOs;

namespace Agriculture_Analyst.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> SignUpAsync(SignUpRequestDto request);
        Task<AuthResponseDto> SignInAsync(SignInRequestDto request);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
}
