using Agriculture_Analyst.Models;
using Agriculture_Analyst.Models.DTOs;
using Agriculture_Analyst.Repositories.Interfaces;
using Agriculture_Analyst.Services.Interfaces;

namespace Agriculture_Analyst.Services.Implementations
{
    public class AuthService : IAuthService
    {
    private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository userRepository, ILogger<AuthService> logger)
        {
      _userRepository = userRepository;
  _logger = logger;
        }

   public async Task<AuthResponseDto> SignUpAsync(SignUpRequestDto request)
        {
   try
  {
     // Validate input
       if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Email))
   {
      return new AuthResponseDto
               {
         Success = false,
          Message = "Username and Email are required."
     };
              }

    // Check password match
                if (request.Password != request.ConfirmPassword)
    {
         return new AuthResponseDto
          {
        Success = false,
       Message = "Passwords do not match."
        };
   }

     // Check password length
            if (request.Password.Length < 6)
          {
      return new AuthResponseDto
         {
      Success = false,
        Message = "Password must be at least 6 characters long."
         };
           }

           // Check if user exists
 if (await _userRepository.UserExistsAsync(request.Username, request.Email))
           {
  return new AuthResponseDto
    {
          Success = false,
         Message = "Username or Email already exists."
         };
   }

        // Create new user
          var user = new User
            {
          Username = request.Username,
 Email = request.Email,
      Name = request.Name,
    Phone = request.Phone,
 Gender = request.Gender,
              DateOfBirth = request.DateOfBirth,
         Address = request.Address,
        Password = HashPassword(request.Password),
    IsActive = true,
          AddedAt = DateTime.Now
    };

  var createdUser = await _userRepository.CreateUserAsync(user);

        return new AuthResponseDto
                {
           Success = true,
        Message = "User registered successfully!",
    User = new UserDto
         {
               UserId = createdUser.UserId,
     Username = createdUser.Username,
Email = createdUser.Email,
             Name = createdUser.Name,
    Phone = createdUser.Phone,
        Gender = createdUser.Gender,
 DateOfBirth = createdUser.DateOfBirth,
  Address = createdUser.Address,
      IsActive = createdUser.IsActive,
            AddedAt = createdUser.AddedAt
           }
    };
            }
          catch (Exception ex)
      {
         _logger.LogError($"Error during sign up: {ex.Message}");
  return new AuthResponseDto
        {
     Success = false,
               Message = "An error occurred during registration. Please try again."
  };
    }
    }

        public async Task<AuthResponseDto> SignInAsync(SignInRequestDto request)
        {
            try
        {
      // Validate input
      if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
         {
             return new AuthResponseDto
  {
        Success = false,
 Message = "Username and Password are required."
         };
       }

       // Get user by username
         var user = await _userRepository.GetUserByUsernameAsync(request.Username);

 if (user == null)
   {
     return new AuthResponseDto
           {
       Success = false,
            Message = "Invalid username or password."
        };
}

                // Check if user is active
   if (!user.IsActive)
                {
       return new AuthResponseDto
      {
          Success = false,
     Message = "User account is inactive."
      };
          }

 // Verify password
       if (!VerifyPassword(request.Password, user.Password))
    {
       return new AuthResponseDto
           {
          Success = false,
       Message = "Invalid username or password."
              };
          }

 return new AuthResponseDto
            {
        Success = true,
        Message = "Login successful!",
          User = new UserDto
        {
UserId = user.UserId,
      Username = user.Username,
    Email = user.Email,
       Name = user.Name,
 Phone = user.Phone,
   Gender = user.Gender,
               DateOfBirth = user.DateOfBirth,
             Address = user.Address,
   IsActive = user.IsActive,
      AddedAt = user.AddedAt
         }
           };
      }
            catch (Exception ex)
     {
   _logger.LogError($"Error during sign in: {ex.Message}");
       return new AuthResponseDto
    {
                Success = false,
      Message = "An error occurred during login. Please try again."
                };
            }
        }

   public string HashPassword(string password)
        {
return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hash)
    {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
