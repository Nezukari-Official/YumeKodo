using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
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
public class YumeController : ControllerBase
{
    private DataContext _Context;
    private readonly IJWTService JwtService;

    public YumeController(DataContext Context, IJWTService JwtService)
    {
        _Context = Context;
        this.JwtService = JwtService;
    }

    [HttpPost("Chat")]
    public ActionResult<ApiResponse<object>> Chat([FromBody] UserMessage Request)
    {
        if (string.IsNullOrEmpty(Request.Message))
        {
            return BadRequest(new ApiResponse<object>
            {
                Status = StatusCodes.Status400BadRequest,
                Data = null,
                Message = "Message Cannot Be Empty"
            });
        }

        var Game = _Context.Games
            .AsEnumerable()
            .FirstOrDefault(g => Request.Message.Contains(g.Title, StringComparison.OrdinalIgnoreCase));


        if (Game != null)
        {
            return Ok(new ApiResponse<object>
            {
                Status = StatusCodes.Status200OK,
                Data = new
                {
                    reply = $"Sure! To download '{Game.Title}', click the link below. After downloading, just unzip the file and run the .exe. Have fun!",
                    downloadLink = Game.DownloadURL
                },
                Message = "Game Instructions Provided!"
            });
        }

        var SmallTalkReply = GetSmallTalkReply(Request.Message);
        return Ok(new ApiResponse<object>
        {
            Status = StatusCodes.Status200OK,
            Data = new { Reply = SmallTalkReply },
            Message = "Casual Reply Initialized!"
        });
    }

    private string GetSmallTalkReply(string Input)
    {
        Input = Input.ToLower();
        var Random = new Random();

        if (Input.Contains("hello") || Input.Contains("hi"))
        {
            var Greetings = new[]
            {
                "Hello there! I'm Yume, nice to see you!",
                "Hiya! Yume here, ready to chat!",
                "Hey! Glad you dropped by!",
                "Yo! What’s up?",
                "Hey hey! How’s your day going?",
                "Oh, hi! I was just thinking of you!",
                "Hii! Want to talk about games?",
                "Hello! Always happy when someone says hi!",
                "Oh, hey there! You caught me daydreaming.",
                "Hello, traveler of dreams!",
                "Hey you! Yes, you!",
                "Hi, hi! So good to see you!"
            };
            return Greetings[Random.Next(Greetings.Length)];
        }
        if (Input.Contains("how are you"))
        {
            var SmallTalkResponses = new[]
            {
                "I'm doing well, thank you for asking! Ready to help you with any games!",
                "Pretty good! I've been waiting to chat with you!",
                "Feeling great — especially now that you're here!",
                "Doing fine! How about you?",
                "Not bad at all! Thinking about new adventures",
                "I’m feeling cheerful today!",
                "I’ve been better, but chatting with you makes me happy!",
                "I’m energized and ready for anything!",
                "I feel like it’s a good day for gaming!",
                "I’m okay — just curious what’s on your mind.",
                "I’m doing well! What about yourself?",
                "Perfectly fine. Wanna make it more interesting with a game?"
            };
            return SmallTalkResponses[Random.Next(SmallTalkResponses.Length)];
        }
        if (Input.Contains("thank you") || Input.Contains("thanks"))
        {
            var GratitudeResponses = new[]
            {
                "You're very welcome!",
                "No problem at all!",
                "Anytime — happy to help!",
                "It’s my pleasure!",
                "Of course, you can always count on me!",
                "No worries — that’s what I’m here for!",
                "Always glad to help out a friend.",
                "You’re welcome, big time!",
                "No need to thank me, hehe.",
                "Don’t mention it!",
                "Anything for you!",
                "You’re welcome, and thank you for being so nice!"
            };
            return GratitudeResponses[Random.Next(GratitudeResponses.Length)];
        }

        var FallbackResponses = new[]
        {
            "Hehe, I'm here! Feel free to ask me about games or just chat!",
            "I’m always around if you want to talk or need game instructions!",
            "Yup, listening! What’s on your mind?",
            "I didn’t quite catch that, but I’m curious!",
            "Tell me more, please",
            "I’m not sure about that, but I can try to help!",
            "Oh! Interesting… wanna explain a bit more?",
            "I may not understand fully, but I’m happy to chat!",
            "Hmm, that’s a tough one… but I’m here for you!",
            "I’ll figure it out if you give me a hint",
            "That’s cute. Keep going!",
            "Sorry if I sound confused, but I’m still learning"
        };
        return FallbackResponses[Random.Next(FallbackResponses.Length)];
    }
}
