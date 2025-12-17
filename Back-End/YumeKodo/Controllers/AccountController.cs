using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using YumeKodo.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YumeKodo.Requests;
using YumeKodo.Models;
using YumeKodo.Enums;
using YumeKodo.Data;
using YumeKodo.SMTP;
using YumeKodo.CORE;
using System.Data;
using BCrypt.Net;
using BCrypt;



namespace YumeKodo.Controllers;

[Route("api/[controller]")]
[ApiController]

public class AccountController : ControllerBase
{
    private readonly IJWTService JwtService;
    private DataContext _Context;

    public AccountController(IJWTService jwtService, DataContext context)
    {
        JwtService = jwtService;
        _Context = context;
    }

    [HttpPost("Sign-Up")]
    public ActionResult SignUp(AddAccount Request)
    {
        var UserExists = _Context.Accounts.FirstOrDefault(u => u.Email == Request.Email);

        if (UserExists == null)
        {
            var Account = new Account
            {
                Nickname = Request.Nickname,
                Email = Request.Email,
                Role = Roles.User,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                EmailIsVerified = false
            };
            Account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(Request.PasswordHash);

            Random Random = new Random();
            string RandomCode = Random.Next(1000, 9999).ToString();
            Account.VerificationCode = RandomCode;

            SMTPService SMTPService = new SMTPService();
            SMTPService.Verification(Account.Email, Subject: "Please Confirm Your Email", Account.VerificationCode);

            _Context.Accounts.Add(Account);
            _Context.SaveChanges();

            var Response = new ApiResponse<Account>
            {
                Status = StatusCodes.Status200OK,
                Data = Account,
                Message = "Account Created Successfully! Please verify your email."
            };
            return Ok(Response);
        }
        else
        {
            var Response = new ApiResponse<bool>
            {
                Status = StatusCodes.Status409Conflict,
                Data = false,
                Message = "Account Already Exists!"
            };
            return BadRequest(Response);
        }
    }

    [HttpPost("Sign-In")]
    public ActionResult SignIn([FromBody] SignInRequest Request)
    {
        var Account = _Context.Accounts.FirstOrDefault(u => u.Email == Request.Email);

        if (Account == null)
        {
            return NotFound(new ApiResponse<bool>
            {
                Status = StatusCodes.Status404NotFound,
                Data = false,
                Message = "Account Not Found!"
            });
        }

        if (Account.PasswordHash == null)
        {
            return StatusCode(500, new ApiResponse<bool>
            {
                Status = StatusCodes.Status500InternalServerError,
                Data = false,
                Message = "Stored password hash is null!"
            });
        }

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(Request.PasswordHash, Account.PasswordHash);

        if (isPasswordValid && Account.EmailIsVerified)
        {
            try
            {
                Console.WriteLine("=== JWT Debug Info ===");
                Console.WriteLine($"Account Is Null: {Account == null}");
                Console.WriteLine($"AccountId: {Account.AccountId}");
                Console.WriteLine($"Nickname: '{Account.Nickname}'");
                Console.WriteLine($"Email: '{Account.Email}'");
                Console.WriteLine($"Role: {Account.Role}");

                Console.WriteLine("About To Call JwtService.AccountToken...");
                var tokenResult = JwtService.GetAccountToken(Account);
                Console.WriteLine($"Token Generated Successfully, Length: {tokenResult?.Token?.Length}");

                return Ok(new ApiResponse<AccountToken>
                {
                    Status = StatusCodes.Status200OK,
                    Data = tokenResult,
                    Message = "Token Generated Successfully!"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== FULL JWT ERROR ===");
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                Console.WriteLine($"InnerException: {ex.InnerException?.Message}");
                Console.WriteLine($"InnerException StackTrace: {ex.InnerException?.StackTrace}");

                return StatusCode(500, new ApiResponse<string>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Data = null,
                    Message = $"JWT generation failed: {ex.Message}"
                });
            }
        }

        if (isPasswordValid && !Account.EmailIsVerified)
        {
            return Unauthorized(new ApiResponse<bool>
            {
                Status = StatusCodes.Status403Forbidden,
                Data = false,
                Message = "Please Verify Your Email!"
            });
        }

        return Unauthorized(new ApiResponse<bool>
        {
            Status = StatusCodes.Status401Unauthorized,
            Data = false,
            Message = "Wrong Password!"
        });
    }

    [HttpPost("Verify-Email/{Email}/{VerificationCode}")]
    public ActionResult Verification(string Email, string VerificationCode)
    {
        var Account = _Context.Accounts.FirstOrDefault(u => u.Email == Email);

        if (Account == null)
        {
            return NotFound(new ApiResponse<bool>
            {
                Status = StatusCodes.Status404NotFound,
                Data = false,
                Message = "Account Not Found!"
            });
        }

        if (Account.VerificationCode == VerificationCode)
        {
            Account.EmailIsVerified = true;
            Account.VerificationCode = null;
            Account.UpdatedAt = DateTime.UtcNow;
            _Context.SaveChanges();

            return Ok(new ApiResponse<bool>
            {
                Status = StatusCodes.Status200OK,
                Data = true,
                Message = "Email Verified Successfully!"
            });
        }
        else
        {
            return BadRequest(new ApiResponse<bool>
            {
                Status = StatusCodes.Status400BadRequest,
                Data = false,
                Message = "Wrong Verification Code!"
            });
        }
    }

    [HttpPost("Get-Password-Reset-Code")]
    public ActionResult GetPasswordResetCode(string AccountEmail)
    {
        var Account = _Context.Accounts.FirstOrDefault(u => u.Email == AccountEmail);

        if (AccountEmail == null)
        {
            var Response = new ApiResponse<bool>
            {
                Status = StatusCodes.Status404NotFound,
                Data = false,
                Message = "Account Not Found!"
            };
            return NotFound(Response);
        }
        else
        {
            if (Account.EmailIsVerified == true)
            {
                Random Random = new Random();
                string RandomCode = Random.Next(10000, 99999).ToString();

                Account.PasswordResetCode = RandomCode;
                SMTPService SMTPService = new SMTPService();

                SMTPService.PasswordResetCode(Account.Email, "Password Reset Code", Account.PasswordResetCode);
                _Context.SaveChanges();


                var Response = new ApiResponse<string>
                {
                    Status = StatusCodes.Status200OK,
                    Data = "Password Reset Code Sent Successfully To Your Email!",
                    Message = null
                };
                return Ok(Response);
            }
            else
            {
                var Response = new ApiResponse<bool>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Data = false,
                    Message = "Email Is Not Verified"
                };
                return BadRequest(Response);
            }
        }
    }

    [HttpPut("Promote-User-To-Admin")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public ActionResult PromoteUserToAdmin(int userId, int requestingUserId)
    {
        var requestingUser = _Context.Accounts.FirstOrDefault(a => a.AccountId == requestingUserId);

        if (requestingUser?.Role != Roles.Creator)
        {
            return Forbid("Only Creator can promote users to Admin");
        }

        var userToPromote = _Context.Accounts.FirstOrDefault(a => a.AccountId == userId);

        if (userToPromote == null)
        {
            return NotFound(new ApiResponse<bool>
            {
                Status = StatusCodes.Status404NotFound,
                Data = false,
                Message = "User not found"
            });
        }

        if (userToPromote.Role == Roles.User)
        {
            userToPromote.Role = Roles.Admin;
            userToPromote.UpdatedAt = DateTime.UtcNow;
            _Context.SaveChanges();

            return Ok(new ApiResponse<bool>
            {
                Status = StatusCodes.Status200OK,
                Data = true,
                Message = "User promoted to Admin successfully"
            });
        }

        return BadRequest(new ApiResponse<bool>
        {
            Status = StatusCodes.Status400BadRequest,
            Data = false,
            Message = "User is already Admin or Creator"
        });
    }

    [HttpPut("Reset-Password")]
    public ActionResult ResetPassword(string Email, string Code, string NewPassword)
    {
        var Account = _Context.Accounts.FirstOrDefault(u => u.Email == Email);

        if (Account == null)
        {
            var Response = new ApiResponse<bool>
            {
                Status = StatusCodes.Status404NotFound,
                Data = false,
                Message = "Account Not Found!"
            };
            return NotFound(Response);
        }
        else
        {
            if (Account.PasswordResetCode == Code)
            {
                var NewPasswordHash = BCrypt.Net.BCrypt.HashPassword(NewPassword);

                Account.PasswordHash = NewPasswordHash;
                Account.PasswordResetCode = null;
                _Context.SaveChanges();

                var Response = new ApiResponse<string>
                {
                    Status = StatusCodes.Status200OK,
                    Data = "Password Was Reseted Successfully!",
                    Message = null
                };
                return Ok(Response);
            }
            else
            {
                var Response = new ApiResponse<string>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Data = "Invalid Code Password Reset Code!",
                    Message = null
                };
                return BadRequest(Response);
            }
        }
    }

    [HttpPut("Update-Profile")]
    public ActionResult UpdateProfile(string Email, string NewNickname, string NewEmail, string NewProfileImage)
    {
        var Account = _Context.Accounts.FirstOrDefault(u => u.Email == Email);

        if(Account == null)
        {
            var Response = new ApiResponse<bool>
            {
                Status = StatusCodes.Status404NotFound,
                Data = false,
                Message = "Account Not Found!"
            };
            return NotFound(Response);
        }
        if (Account.EmailIsVerified == false)
        {
            var Response = new ApiResponse<bool>
            {
                Status = StatusCodes.Status400BadRequest,
                Data = false,
                Message = "Please Verify Your Email First!"
            };
            return BadRequest(Response);
        }

        Account.Nickname = NewNickname;
        Account.Email = NewEmail;
        Account.ProfileImageURL = NewProfileImage;
        _Context.SaveChanges();

        var SuccessResponse = new ApiResponse<bool>
        {
            Status = StatusCodes.Status200OK,
            Data = true,
            Message = "Profile Updated Successfully!"
        };

        return Ok(SuccessResponse);
    }

    [HttpGet("Get-All-Accounts")]
    public ActionResult GetAllAccounts()
    {
        var Accounts = _Context.Accounts.ToList();

        return Ok(new ApiResponse<List<Account>>
        {
            Status = StatusCodes.Status200OK,
            Data = Accounts,
            Message = "Accounts Retrieved Successfully"
        });
    }

    [HttpGet("Get-Account-By-Id")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public ActionResult GetAccountById(int Id)
    {
        var Account = _Context.Accounts.FirstOrDefault(u => u.AccountId == Id);

        if (Account == null)
        {
            return NotFound(new ApiResponse<bool>
            {
                Status = StatusCodes.Status404NotFound,
                Data = false,
                Message = "Account Not Found!"
            });
        }

        if (Account.EmailIsVerified)
        {
            var safeAccount = new
            {
                accountId = Account.AccountId,
                nickname = Account.Nickname,
                email = Account.Email,
                role = Account.Role.ToString(),
                profileImageURL = Account.ProfileImageURL,
                createdAt = Account.CreatedAt
            };

            return Ok(new ApiResponse<object>
            {
                Status = StatusCodes.Status200OK,
                Data = safeAccount,
                Message = "Account Retrieved Successfully!"
            });
        }
        else
        {
            return BadRequest(new ApiResponse<bool>
            {
                Status = StatusCodes.Status400BadRequest,
                Data = false,
                Message = "Please Verify Your Email First!"
            });
        }
    }

    [HttpDelete("Delete-Account/{Id}")]
    public ActionResult DeleteAccount(int Id)
    {
        var Account = _Context.Accounts.FirstOrDefault(u => u.AccountId == Id);

        if (Account == null)
        {
            var Response = new ApiResponse<bool>
            {
                Status = StatusCodes.Status404NotFound,
                Data = false,
                Message = "Account Not Found!"
            };
            return NotFound(Response);
        }
        else
        {
            _Context.Accounts.Remove(Account);
            _Context.SaveChanges();

            var Response = new ApiResponse<bool>
            {
                Status = StatusCodes.Status200OK,
                Data = true,
                Message = "Account Deleted Successfully!"
            };
            return Ok(Response);
        }
    }
}
