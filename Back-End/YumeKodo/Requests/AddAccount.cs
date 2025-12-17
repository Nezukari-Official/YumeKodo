namespace YumeKodo.Requests;
public class AddAccount
{
    public string Nickname { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
}
