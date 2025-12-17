using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using YumeKodo.Services.Interfaces;
using System.Security.Claims;
using YumeKodo.Models;
using YumeKodo.CORE;
using System.Text;


namespace YumeKodo.Services.Implementation;
public class JWTService : IJWTService
{
    //public AccountToken GetAccountToken(Account Account)
    //{
    //    if (Account == null)
    //        throw new ArgumentNullException(nameof(Account), "Account cannot be null");

    //    if (string.IsNullOrEmpty(Account.Nickname))
    //        throw new ArgumentException("Account Nickname cannot be null or empty");

    //    if (string.IsNullOrEmpty(Account.Email))
    //        throw new ArgumentException("Account Email cannot be null or empty");

    //    var JwtKey = "jox0QDQlzbj8FH5jDM7h1YykQFgJMW9iqjqrqrGfmWTWnIfUSEY1htLleRMXAgxQ";
    //    var JwtIssuer = "YumeKodo";
    //    var JwtAudience = "Account";
    //    var JwtDuration = 1440;

    //    var SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
    //    var Credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);

    //    var Claims = new[]
    //    {
    //        new Claim(JwtRegisteredClaimNames.NameId, Account.AccountId.ToString()),
    //        new Claim(JwtRegisteredClaimNames.Name, Account.Nickname),
    //        new Claim(JwtRegisteredClaimNames.Email, Account.Email),
    //        new Claim("AccountId", Account.AccountId.ToString()),
    //        new Claim("Role", Account.Role.ToString())
    //    };

    //    var Token = new JwtSecurityToken(
    //        issuer: JwtIssuer,
    //        audience: JwtAudience,
    //        expires: DateTime.UtcNow.AddMinutes(JwtDuration),
    //        claims: Claims,
    //        signingCredentials: Credentials
    //    );

    //    return new AccountToken
    //    {
    //        Token = new JwtSecurityTokenHandler().WriteToken(Token)
    //    };
    //}
    public AccountToken GetAccountToken(Account Account)
    {
        if (Account == null)
            throw new ArgumentNullException(nameof(Account), "Account cannot be null");
        if (string.IsNullOrEmpty(Account.Nickname))
            throw new ArgumentException("Account Nickname cannot be null or empty");
        if (string.IsNullOrEmpty(Account.Email))
            throw new ArgumentException("Account Email cannot be null or empty");

        var JwtKey = "jox0QDQlzbj8FH5jDM7h1YykQFgJMW9iqjqrqrGfmWTWnIfUSEY1htLleRMXAgxQ";
        var JwtIssuer = "YumeKodo";
        var JwtAudience = "Account";
        var JwtDuration = 1440;

        var SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
        var Credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);

        var Claims = new[]
        {
        new Claim(JwtRegisteredClaimNames.NameId, Account.AccountId.ToString()),
        new Claim(JwtRegisteredClaimNames.Name, Account.Nickname),
        new Claim(JwtRegisteredClaimNames.Email, Account.Email),
        new Claim("AccountId", Account.AccountId.ToString()),
        new Claim("Role", Account.Role.ToString()),
        //new Claim("ProfileImageURL", Account.ProfileImageURL ?? "null")
        new Claim("ProfileImageURL",
        string.IsNullOrEmpty(Account.ProfileImageURL)
            ? ""
            : Uri.EscapeDataString(Account.ProfileImageURL))
    };

        var Token = new JwtSecurityToken(
            issuer: JwtIssuer,
            audience: JwtAudience,
            expires: DateTime.UtcNow.AddMinutes(JwtDuration),
            claims: Claims,
            signingCredentials: Credentials
        );

        return new AccountToken
        {
            Token = new JwtSecurityTokenHandler().WriteToken(Token)
        };
    }
}
