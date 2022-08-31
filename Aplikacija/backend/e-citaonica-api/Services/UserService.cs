using e_citaonica_api.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
namespace e_citaonica_api.Services;

public class UserService : IUserService
{
    private readonly IConfiguration _configuration;
    public UserService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string CreatePasswordHash(string password)
    {
        string salt = BCrypt.Net.BCrypt.GenerateSalt(12);
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);
        return hashedPassword;
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }

    public string CreateToken(Korisnik k)
    {
        List<Claim> claims = new()
        {
            new Claim("preferred_username", k.Email),
            new Claim("roles", k.Tip),
            new Claim("name", k.Ime)
        };

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
            _configuration.GetSection("AppSettings:Token").Value));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds);
        
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        
        return jwt;
    }

    public async Task SendEmailAsync(string toAdress, string subject, string message)
    {
        var mailMessage = new MailMessage(_configuration["ReturnPaths:SenderEmail"], toAdress, subject, message);

        using var client = new SmtpClient(_configuration["SMTP:Host"], int.Parse(_configuration["SMTP:Port"]))
        {
            Credentials = new NetworkCredential(_configuration["SMTP:ApiKey"], _configuration["SMTP:SecretKey"])
        };
        await client.SendMailAsync(mailMessage);
    }
}
