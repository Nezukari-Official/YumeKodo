using YumeKodo.Enums;



namespace YumeKodo.Models;
public class Game
{
    public int GameId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Genres Genre { get; set; }

    public string ThumbnailURL { get; set; }
    public string DownloadURL { get; set; }
}
