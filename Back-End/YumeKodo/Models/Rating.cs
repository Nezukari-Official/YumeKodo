namespace YumeKodo.Models;
public class Rating
{
    public int RatingId { get; set; }
    public int AccountId { get; set; }
    public Account Account { get; set; }
    public int GameId { get; set; }
    public Game Game { get; set; }
    public int StarRating { get; set; }
    public DateTime CreatedAt { get; set; }
}
