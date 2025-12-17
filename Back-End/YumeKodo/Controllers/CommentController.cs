using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
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
public class CommentController : ControllerBase
{
    private DataContext _Context;

    public CommentController(DataContext Context)
    {
        _Context = Context;
    }


    [HttpPost("Add-Comment")]
    public ActionResult AddComment(int accountId, int gameId, string commentText)
    {
        if (string.IsNullOrWhiteSpace(commentText))
        {
            return BadRequest(new ApiResponse<bool>
            {
                Status = StatusCodes.Status400BadRequest,
                Data = false,
                Message = "Comment Text Cannot Be Empty"
            });
        }

        var comment = new Comment
        {
            AccountId = accountId,
            GameId = gameId,
            CommentText = commentText.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _Context.Comments.Add(comment);
        _Context.SaveChanges();

        return Ok(new ApiResponse<Comment>
        {
            Status = StatusCodes.Status200OK,
            Data = comment,
            Message = "Comment Added Successfully!"
        });
    }

    [HttpGet("Get-Game-Comments/{gameId}")]
    public ActionResult GetGameComments(int gameId)
    {
        var comments = _Context.Comments
            .Include(c => c.Account)
            .Where(c => c.GameId == gameId)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new
            {
                commentId = c.CommentId,
                accountId = c.AccountId,
                nickname = c.Account.Nickname,
                role = c.Account.Role.ToString(),
                commentText = c.CommentText,
                createdAt = c.CreatedAt
            })
            .ToList();

        return Ok(new ApiResponse<object>
        {
            Status = StatusCodes.Status200OK,
            Data = comments,
            Message = "Comments Retrieved Successfully"
        });
    }

    [HttpDelete("Delete-Comment/{commentId}")]
    public ActionResult DeleteComment(int commentId, int requestingUserId)
    {
        var comment = _Context.Comments
            .Include(c => c.Account)
            .FirstOrDefault(c => c.CommentId == commentId);

        if (comment == null)
        {
            return NotFound(new ApiResponse<bool>
            {
                Status = StatusCodes.Status404NotFound,
                Data = false,
                Message = "Comment Not Found"
            });
        }

        var requestingUser = _Context.Accounts
            .FirstOrDefault(a => a.AccountId == requestingUserId);

        if (requestingUser == null)
        {
            return NotFound(new ApiResponse<bool>
            {
                Status = StatusCodes.Status404NotFound,
                Data = false,
                Message = "User Not Found"
            });
        }

        bool canDelete = false;

        switch (requestingUser.Role)
        {
            case Roles.Creator:
                canDelete = true;
                break;
            case Roles.Admin:
                canDelete = comment.AccountId == requestingUserId ||
                           comment.Account.Role == Roles.User;
                break;
            case Roles.User:
                canDelete = comment.AccountId == requestingUserId;
                break;
        }

        if (!canDelete)
        {
            return Forbid(new ApiResponse<bool>
            {
                Status = StatusCodes.Status403Forbidden,
                Data = false,
                Message = "You Don't Have Permission To Delete This Comment"
            }.ToString());
        }

        _Context.Comments.Remove(comment);
        _Context.SaveChanges();

        return Ok(new ApiResponse<bool>
        {
            Status = StatusCodes.Status200OK,
            Data = true,
            Message = "Comment Deleted Successfully"
        });
    }

    [HttpPut("Edit-Comment/{commentId}")]
    public ActionResult EditComment(int commentId, int requestingUserId, string newCommentText)
    {
        if (string.IsNullOrWhiteSpace(newCommentText))
        {
            return BadRequest(new ApiResponse<bool>
            {
                Status = StatusCodes.Status400BadRequest,
                Data = false,
                Message = "Comment Text Cannot Be Empty"
            });
        }

        var comment = _Context.Comments
            .FirstOrDefault(c => c.CommentId == commentId);

        if (comment == null)
        {
            return NotFound(new ApiResponse<bool>
            {
                Status = StatusCodes.Status404NotFound,
                Data = false,
                Message = "Comment Not Found"
            });
        }

        if (comment.AccountId != requestingUserId)
        {
            return Forbid(new ApiResponse<bool>
            {
                Status = StatusCodes.Status403Forbidden,
                Data = false,
                Message = "You Can Only Edit Your Own Comments"
            }.ToString());
        }

        comment.CommentText = newCommentText.Trim();
        _Context.SaveChanges();

        return Ok(new ApiResponse<Comment>
        {
            Status = StatusCodes.Status200OK,
            Data = comment,
            Message = "Comment Updated Successfully"
        });
    }
}
