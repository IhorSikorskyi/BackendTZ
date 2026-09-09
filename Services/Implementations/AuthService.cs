using BackendTZ.DTOs.Requests;
using BackendTZ.DTOs.Responses;
using BackendTZ.Entities;
using BackendTZ.Repositories.Interfaces;
using BackendTZ.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BackendTZ.Services.Implementations;

public class AuthService(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IJwtTokenGenerator jwtTokenGenerator, 
    IPasswordHasher<User> passwordHasher) : IAuthService
{
    public async Task<TokenResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        if(await userRepository.IsExistByEmailAsync(request.Email, ct))
        {
            throw new InvalidOperationException("User with this email already exists.");
        }

        if(request.Password != request.ConfirmPassword)
        {
            throw new InvalidOperationException("Passwords do not match.");
        }

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            Number = request.Number ?? string.Empty,
            PasswordHash = string.Empty
        };

        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        await userRepository.AddAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);
        
        return BuildTokenResponse(user, jwtTokenGenerator);
    }

    public async Task<TokenResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await userRepository.GetUserByEmailAsync(request.Email, ct) 
                   ?? throw new UnauthorizedAccessException("Invalid email or password.");

        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if(result == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        return BuildTokenResponse(user, jwtTokenGenerator);
    }

    public async Task<TokenResponse> LogoutAsync(CancellationToken ct = default)
    {
        // TODO: Add refresh token and implement logout functionality

        throw new NotImplementedException();
    }

    private static TokenResponse BuildTokenResponse(User user, IJwtTokenGenerator tokenGenerator)
    {
        var token = tokenGenerator.GenerateToken(user);

        return new TokenResponse(
            AccessToken: token.AccessToken,
            ExpiresAt: token.ExpiresAt,
            User: token.User
        );
    }
}