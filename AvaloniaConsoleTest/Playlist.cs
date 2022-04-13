using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaConsoleTest
{
	class Playlist
	{
		public string Title { get; }
		public byte[] Image { get; }
		public string ReleaseDate { get; }
		public string Genre { get; }
		public string Label { get; }

		public IEnumerable<Song> Tracks { set; get; } = new List<Song>();

		public Playlist(string title, byte[] image = null, string releaseDate = "", string genre = "", string label = "")
		{
			Title = title;
			Image = image;
			ReleaseDate = releaseDate;
			Genre = genre;
			Label = label;
		}

		public void Show()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(new string('-', Console.WindowWidth));
			stringBuilder.AppendLine($"Playlist : {Title}");
			stringBuilder.AppendLine("Image : " + (Image.Length > 0 ? "Image is present" : "No image"));
			stringBuilder.AppendLine($"Release date : {ReleaseDate}");
			stringBuilder.AppendLine($"Genre : {Genre}");
			stringBuilder.AppendLine($"Label : {Label}");

			stringBuilder.AppendLine("\n\n \tTracks \n\n");

			Console.ForegroundColor = ConsoleColor.Yellow;

			if (Tracks.Any())
			{
				stringBuilder.AppendLine($"{"Title",-50}{"Artist",-20}{"Album",-60}{"Duration",-10}{"Genre",-15}\n");
				foreach (var track in Tracks)
					stringBuilder.AppendLine(track.ToString());
			}

			Console.WriteLine(stringBuilder.ToString());
			Console.ResetColor();
		}
	}
}
