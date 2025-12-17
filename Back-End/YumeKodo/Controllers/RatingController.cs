using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using YumeKodo.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YumeKodo.Requests;
using YumeKodo.Models;
using YumeKodo.Data;
using YumeKodo.SMTP;
using YumeKodo.CORE;
using System.Data;
using BCrypt.Net;
using BCrypt;



namespace YumeKodo.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RatingController : ControllerBase
{
    private DataContext _Context;

    public RatingController(DataContext Context)
    {
        _Context = Context;
    }


    [HttpPost("Add-Rating")]
    public ActionResult AddRating(int accountId, int gameId, int starRating)
    {
        if (starRating < 1 || starRating > 5)
        {
            return BadRequest(new ApiResponse<bool>
            {
                Status = StatusCodes.Status400BadRequest,
                Data = false,
                Message = "Star Rating Must Be Between 1 And 5"
            });
        }

        var existingRating = _Context.Ratings
            .FirstOrDefault(r => r.AccountId == accountId && r.GameId == gameId);

        if (existingRating != null)
        {
            existingRating.StarRating = starRating;
            existingRating.CreatedAt = DateTime.UtcNow;
            _Context.SaveChanges();

            return Ok(new ApiResponse<Rating>
            {
                Status = StatusCodes.Status200OK,
                Data = existingRating,
                Message = "Rating Updated Successfully!"
            });
        }
        else
        {
            var rating = new Rating
            {
                AccountId = accountId,
                GameId = gameId,
                StarRating = starRating,
                CreatedAt = DateTime.UtcNow
            };

            _Context.Ratings.Add(rating);
            _Context.SaveChanges();

            return Ok(new ApiResponse<Rating>
            {
                Status = StatusCodes.Status200OK,
                Data = rating,
                Message = "Rating Added Successfully!"
            });
        }
    }

    [HttpGet("Get-Game-Ratings/{gameId}")]
    public ActionResult GetGameRatings(int gameId)
    {
        var ratings = _Context.Ratings
            .Where(r => r.GameId == gameId)
            .ToList();

        if (!ratings.Any())
        {
            return Ok(new ApiResponse<object>
            {
                Status = StatusCodes.Status200OK,
                Data = new { averageRating = 0, totalRatings = 0 },
                Message = "No Ratings Found For This Game"
            });
        }

        var averageRating = ratings.Average(r => r.StarRating);
        var totalRatings = ratings.Count;

        return Ok(new ApiResponse<object>
        {
            Status = StatusCodes.Status200OK,
            Data = new { averageRating = Math.Round(averageRating, 1), totalRatings },
            Message = "Ratings Retrieved Successfully"
        });
    }

    [HttpGet("Get-User-Rating")]
    public ActionResult GetUserRating(int accountId, int gameId)
    {
        var rating = _Context.Ratings
            .FirstOrDefault(r => r.AccountId == accountId && r.GameId == gameId);

        if (rating == null)
        {
            return Ok(new ApiResponse<object>
            {
                Status = StatusCodes.Status200OK,
                Data = null,
                Message = "User Hasn't Rated This Game Yet"
            });
        }

        return Ok(new ApiResponse<Rating>
        {
            Status = StatusCodes.Status200OK,
            Data = rating,
            Message = "User Rating Retrieved Successfully"
        });
    }

    [HttpDelete("Delete-Rating")]
    public ActionResult DeleteRating(int accountId, int gameId)
    {
        var rating = _Context.Ratings
            .FirstOrDefault(r => r.AccountId == accountId && r.GameId == gameId);

        if (rating == null)
        {
            return NotFound(new ApiResponse<bool>
            {
                Status = StatusCodes.Status404NotFound,
                Data = false,
                Message = "Rating Not Found"
            });
        }

        _Context.Ratings.Remove(rating);
        _Context.SaveChanges();

        return Ok(new ApiResponse<bool>
        {
            Status = StatusCodes.Status200OK,
            Data = true,
            Message = "Rating Deleted Successfully"
        });
    }
}
