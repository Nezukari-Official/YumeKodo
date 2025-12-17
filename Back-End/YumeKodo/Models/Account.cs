using YumeKodo.Enums;



namespace YumeKodo.Models;
public class Account
{
    public int AccountId { get; set; }
    public string Nickname { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public Roles Role { get; set; } = Roles.User;

    public string? ProfileImageURL { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }


    public string? VerificationCode { get; set; }
    public string? PasswordResetCode { get; set; }


    public bool EmailIsVerified { get; set; }
}
