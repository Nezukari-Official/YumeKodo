using System.Text.Json.Serialization;
using YumeKodo.Enums;



namespace YumeKodo.Requests;
public class AddGameWithGenre
{
    public string Title { get; set; }
    public string Description { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Genres Genre { get; set; }

    public string ThumbnailURL { get; set; }
    public string DownloadURL { get; set; }
}
