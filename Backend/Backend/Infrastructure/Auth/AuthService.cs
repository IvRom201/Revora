using Backend.Domain.Entities;
using Backend.Infrastructure.Auth.DTOs;
using Backend.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Auth;

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly IJwtService _jwtService;
    private readonly PasswordHasher<Employee> _passwordHasher;

    public AuthService(
        AppDbContext dbContext,
        IJwtService jwtService,
        PasswordHasher<Employee> passwordHasher)
    {
        _dbContext = dbContext;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var employee = await _dbContext.Employees
            .FirstOrDefaultAsync(e => e.Login == request.Login && e.IsActive);

        if (employee is null)
        {
            throw new UnauthorizedAccessException("Invalid login or password.");
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(
            employee,
            employee.PasswordHash,
            request.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException("Invalid login or password.");
        }

        var token = _jwtService.GenerateToken(employee);

        return new LoginResponse
        {
            Token = token,
            Login = employee.Login,
            Role = employee.Role.ToString()
        };
    }
}