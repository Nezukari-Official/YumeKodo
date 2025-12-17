using YumeKodo.Models;
using YumeKodo.CORE;



namespace YumeKodo.Services.Interfaces;
public interface IJWTService
{
    AccountToken GetAccountToken(Account account);
}
