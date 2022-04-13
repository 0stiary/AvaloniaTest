using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace AvaloniaConsoleTest
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WindowWidth = 160;
			bool isError;

			do
			{
				isError = false;
				Console.Write("Insert playlist url : ");
				var playlistUrl = Console.ReadLine();
				try
				{
					var htmlDoc = new HtmlWeb().Load(playlistUrl);
					var playlistInfoNode =
						htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'productInfo')]");
					var trackListingInfoNode = GetPlayListInfo(playlistInfoNode, out string playlistTitle,
						out string playlistLabel, out byte[] playlistImage);


					Playlist playlist = new Playlist(playlistTitle, playlistImage, null, null, playlistLabel);
					playlist.Tracks = GetTrackListFromNode(trackListingInfoNode, playlistTitle);

					playlist.Show();
				}
				catch (Exception e)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine(e.Message + "\n\n");
					Console.ResetColor();

					Console.WriteLine("Press any key to continue");
					Console.ReadKey();
					isError = true;
					Console.Clear();
				}
			} while (isError);

		}

		private static HtmlNode GetPlayListInfo(HtmlNode playlistInfoNode, out string playlistTitle, out string playlistLabel, out byte[] playlistImage)
		{
			var listingInfoNode = playlistInfoNode.SelectSingleNode("//div[contains(@class, 'listing')]");

			playlistImage = Task.Run(async() => await BitmapImageFromUrl(playlistInfoNode.SelectSingleNode("//div[@class='h-100 image']/img").Attributes["src"].Value)).Result;
			playlistTitle = listingInfoNode.SelectSingleNode("//h1[@class='artist']").InnerHtml;
			playlistLabel = playlistInfoNode.SelectSingleNode("//div[@class='details']")
				.SelectSingleNode("//p[contains(text(), 'Label')]/span/a").InnerHtml;

			return listingInfoNode;
		}


		private static IEnumerable<Song> GetTrackListFromNode(HtmlNode listingNode, string playlistTitle)
		{
			var songsList = listingNode.SelectSingleNode("//h2[@class='tracks']").InnerHtml.Split("<br>")
				.Select(s => s.Replace("\"", "")).ToList();

			const string artistAlbumSplitString = " - ";
			var artistName = playlistTitle[..playlistTitle.IndexOf(artistAlbumSplitString, StringComparison.CurrentCultureIgnoreCase)];
			var albumName = playlistTitle.Remove(0, artistName.Length + artistAlbumSplitString.Length);


			return songsList.Select(songTitle => new Song(songTitle, artistName, albumName)).ToList();
		}

		private static async Task<byte[]> BitmapImageFromUrl(string url)
		{
			using WebClient client = new();
			return await client.DownloadDataTaskAsync(url);
		}
	}
}
