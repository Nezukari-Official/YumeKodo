namespace YumeKodo.Models;
public class Comment
{
    public int CommentId { get; set; }
    public int AccountId { get; set; }
    public Account Account { get; set; }
    public int GameId { get; set; }
    public Game Game { get; set; }
    public string CommentText { get; set; }
    public DateTime CreatedAt { get; set; }
}
