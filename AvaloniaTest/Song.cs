namespace AvaloniaTest
{
	public class Song
	{
		public string SongName { get; }
		public string ArtistName { get; }
		public string AlbumName { get; }
		public string Duration { get; }
		public string? Genre { get; set; }

		public Song(string songName, string artistName, string albumName, string duration = "", string? genre = null)
		{
			SongName = songName;
			ArtistName = artistName;
			AlbumName = albumName;
			Duration = duration;
			Genre = genre;
		}
	}
}