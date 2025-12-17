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
public class GameController : ControllerBase
{
    private DataContext _Context;
    private readonly IJWTService JwtService;

    public GameController(DataContext Context, IJWTService JwtService)
    {
        _Context = Context;
        JwtService = JwtService;
    }

    [HttpPost("Add-New-Game")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public ActionResult AddNewGame([FromBody] AddGameWithGenre request)
    {
        var userIdClaim = User.FindFirst("AccountId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized("Invalid User Token");
        }

        var user = _Context.Accounts.FirstOrDefault(a => a.AccountId == userId);
        if (user?.Role != Roles.Creator && user?.Role != Roles.Admin)
        {
            return Forbid("Only Creator And Admin Can Add Games");
        }

        var GameExists = _Context.Games.FirstOrDefault(u => u.Title == request.Title);

        if (GameExists == null)
        {
            var Game = new Game
            {
                Title = request.Title,
                Description = request.Description,
                Genre = request.Genre,
                ThumbnailURL = request.ThumbnailURL,
                DownloadURL = request.DownloadURL
            };

            _Context.Games.Add(Game);
            _Context.SaveChanges();

            return Ok(new ApiResponse<Game>
            {
                Status = StatusCodes.Status200OK,
                Data = Game,
                Message = "Game Added Successfully!"
            });
        }
        else
        {
            return BadRequest(new ApiResponse<bool>
            {
                Status = StatusCodes.Status409Conflict,
                Data = false,
                Message = "Game Already Exists!"
            });
        }
    }

    [HttpPut("Edit-Posted-Game")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public ActionResult EditPostedGame(string Title, string NewTitle, string NewDescription, Genres NewGenre, string NewThumbnail, string NewDownloadURL)
    {
        var userIdClaim = User.FindFirst("AccountId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized("Invalid User Token");
        }

        var user = _Context.Accounts.FirstOrDefault(a => a.AccountId == userId);
        if (user?.Role != Roles.Creator && user?.Role != Roles.Admin)
        {
            return Forbid("Only Creator And Admin Can Edit Games");
        }

        var GameExists = _Context.Games.FirstOrDefault(u => u.Title == Title);

        if (GameExists == null)
        {
            var Response = new ApiResponse<bool>
            {
                Status = StatusCodes.Status404NotFound,
                Data = false,
                Message = "Game Not Found!"
            };
            return NotFound(Response);
        }

        GameExists.Title = NewTitle;
        GameExists.Description = NewDescription;
        GameExists.Genre = NewGenre;
        GameExists.ThumbnailURL = NewThumbnail;
        GameExists.DownloadURL = NewDownloadURL;
        _Context.SaveChanges();

        var SuccessResponse = new ApiResponse<Game>
        {
            Status = StatusCodes.Status200OK,
            Data = GameExists,
            Message = "Game Updated Successfully!"
        };

        return Ok(SuccessResponse);
    }

    [HttpGet("Get-All-Games")]
    public ActionResult GetAllGames(string? genre = null, double? minRating = null)
    {
        var gamesQuery = _Context.Games.AsQueryable();


        if (!string.IsNullOrEmpty(genre) && Enum.TryParse<Genres>(genre, true, out var genreEnum))
        {
            gamesQuery = gamesQuery.Where(g => g.Genre == genreEnum);
        }

        var games = gamesQuery.Select(g => new
        {
            gameId = g.GameId,
            title = g.Title,
            description = g.Description,
            genre = g.Genre.ToString(),
            thumbnailURL = g.ThumbnailURL,
            downloadURL = g.DownloadURL,
            averageRating = _Context.Ratings.Where(r => r.GameId == g.GameId).Any()
                ? _Context.Ratings.Where(r => r.GameId == g.GameId).Average(r => r.StarRating)
                : 0,
            totalRatings = _Context.Ratings.Count(r => r.GameId == g.GameId)
        }).ToList();


        if (minRating.HasValue)
        {
            games = games.Where(g => g.averageRating >= minRating.Value).ToList();
        }

        return Ok(new ApiResponse<object>
        {
            Status = StatusCodes.Status200OK,
            Data = games,
            Message = "Games Retrieved Successfully"
        });
    }

    [HttpGet("Get-Game-By-Id")]
    public ActionResult GetGameById(int Id)
    {
        var game = _Context.Games.FirstOrDefault(u => u.GameId == Id);

        if (game == null)
        {
            return NotFound(new ApiResponse<bool>
            {
                Status = StatusCodes.Status404NotFound,
                Data = false,
                Message = "Game Not Found!"
            });
        }

        var gameWithRating = new
        {
            gameId = game.GameId,
            title = game.Title,
            description = game.Description,
            genre = game.Genre.ToString(),
            thumbnailURL = game.ThumbnailURL,
            downloadURL = game.DownloadURL,
            averageRating = _Context.Ratings.Where(r => r.GameId == game.GameId).Any()
                ? _Context.Ratings.Where(r => r.GameId == game.GameId).Average(r => r.StarRating)
                : 0,
            totalRatings = _Context.Ratings.Count(r => r.GameId == game.GameId)
        };

        return Ok(new ApiResponse<object>
        {
            Status = StatusCodes.Status200OK,
            Data = gameWithRating,
            Message = "Game Retrieved Successfully!"
        });
    }

    [HttpGet("Get-Genres")]
    public ActionResult GetGenres()
    {
        var genres = Enum.GetNames(typeof(Genres)).ToList();

        return Ok(new ApiResponse<List<string>>
        {
            Status = StatusCodes.Status200OK,
            Data = genres,
            Message = "Genres Retrieved Successfully"
        });
    }

    [HttpGet("Download-Game/{gameId}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public ActionResult DownloadGame(int gameId)
    {
        var userIdClaim = User.FindFirst("AccountId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(new ApiResponse<bool>
            {
                Status = StatusCodes.Status401Unauthorized,
                Data = false,
                Message = "Please Sign In To Download Games"
            });
        }

        var user = _Context.Accounts.FirstOrDefault(a => a.AccountId == userId);
        if (user == null || !user.EmailIsVerified)
        {
            return Unauthorized(new ApiResponse<bool>
            {
                Status = StatusCodes.Status401Unauthorized,
                Data = false,
                Message = "Please Verify Your Email To Download Games"
            });
        }

        var game = _Context.Games.FirstOrDefault(g => g.GameId == gameId);
        if (game == null)
        {
            return NotFound(new ApiResponse<bool>
            {
                Status = StatusCodes.Status404NotFound,
                Data = false,
                Message = "Game Not Found"
            });
        }

        return Ok(new ApiResponse<object>
        {
            Status = StatusCodes.Status200OK,
            Data = new
            {
                downloadURL = game.DownloadURL,
                gameTitle = game.Title
            },
            Message = "Download Authorized"
        });
    }

    [HttpDelete("Delete-Game/{Id}")]
    public ActionResult DeleteGame(int Id)
    {
        var Game = _Context.Games.FirstOrDefault(u => u.GameId == Id);

        if (Game == null)
        {
            var Response = new ApiResponse<bool>
            {
                Status = StatusCodes.Status404NotFound,
                Data = false,
                Message = "Game Not Found!"
            };
            return NotFound(Response);
        }
        else
        {
            _Context.Games.Remove(Game);
            _Context.SaveChanges();

            var Response = new ApiResponse<bool>
            {
                Status = StatusCodes.Status200OK,
                Data = true,
                Message = "Game Deleted Successfully!"
            };
            return Ok(Response);
        }
    }
}
