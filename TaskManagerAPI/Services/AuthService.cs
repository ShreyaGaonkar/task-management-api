using TaskManagerAPI.Data.Repositories;
using TaskManagerAPI.DTO.Request;
using TaskManagerAPI.DTO.Response;
using TaskManagerAPI.Exceptions;
using TaskManagerAPI.Helpers;
using TaskManagerAPI.Models;
using TaskManagerAPI.Services.Mappers;

namespace TaskManagerAPI.Services
{
    public interface IAuthService
    {
        Task<AuthResultDTO> LoginAsync(LoginDTO loginDTO);
        Task<ServiceResult<UserProfileDTO>> RegisterAsync(UserRequestDTO userDto);
        Task<UserProfileDTO> GetProfile();
    }

    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserContextService _userContextService;
        private readonly IRoleRepository _roleRepository;

        public AuthService(IConfiguration config,
                           IUserRepository userRepository,
                           IPasswordHasher passwordHasher,
                           IUserContextService userContextService,
                           IRoleRepository roleRepository)
        {
            _config = config;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _userContextService = userContextService;
            _roleRepository = roleRepository;
        }

        public async Task<AuthResultDTO> LoginAsync(LoginDTO loginDTO)
        {
            var user = await AuthenticateUserAsync(loginDTO.Email, loginDTO.Password);

            if (user == null)
                return new AuthResultDTO { Success = false, Message = Captions.InvalidUsernamePassword };

            var token = JwtHelper.GenerateJwtToken(user, _config);

            return new AuthResultDTO { Success = true, Token = token, Message = Captions.LoginSuccessful };
        }


        public async Task<ServiceResult<UserProfileDTO>> RegisterAsync(UserRequestDTO userDTO)
        {
            bool roleExists = await _roleRepository.RoleExistsAsync(userDTO.RoleId);
            if (!roleExists)
            {
                throw new NotFoundException(Captions.InvalidRole);
            }

            var emailExist = await _userRepository.UserExistByEmailAsync(userDTO.Email);
            if (emailExist)
            {
                throw new NotFoundException(Captions.EmailAlreadyExist);
            }

            var registrationResult = await RegisterUserAsync(userDTO);

            if (!registrationResult.Success)
                return registrationResult;

            return new ServiceResult<UserProfileDTO> { Success = true, Message = Captions.UserRegisterSuccessful, Data = registrationResult.Data };

        }

        public async Task<UserProfileDTO> GetProfile()
        {
            var user = await _userRepository.GetByIdAsync(_userContextService.UserId);

            if (user == null)
            {
                throw new NotFoundException(Captions.UserNotFound);
            }

            return UserMapper.MapToUserProfileDTO(user);

        }


        private async Task<User> AuthenticateUserAsync(string email, string password)
        {
            var user = await _userRepository.GetUserWithRolesAsync(email);

            if (user == null || !_passwordHasher.VerifyPassword(password, user.PasswordHash))
                return null;

            return user;
        }

        private async Task<ServiceResult<UserProfileDTO>> RegisterUserAsync(UserRequestDTO userDto)
        {
            var newUser = new User
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email.ToLower(),
                PasswordHash = _passwordHasher.HashPassword(userDto.Password),
                RoleId = userDto.RoleId,
                CreatedDate = DateTime.UtcNow
            };

            var user = await _userRepository.InsertAsync(newUser);

            var userData = UserMapper.MapToUserProfileDTO(user);

            return new ServiceResult<UserProfileDTO> { Success = true, Data = userData };
        }
    }
}
