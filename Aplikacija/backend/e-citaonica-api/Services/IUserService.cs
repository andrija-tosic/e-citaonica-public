using e_citaonica_api.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
namespace e_citaonica_api.Services;

public interface IUserService
{
    public string CreatePasswordHash(string password);
    public bool VerifyPassword(string password, string passwordHash);
    public string CreateToken(Korisnik k);
    public Task SendEmailAsync(string toAdress, string subject, string message);
}
