using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using HtmlAgilityPack;

namespace AvaloniaTest
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private async void ParseButton_OnClick(object? sender, RoutedEventArgs e)
		{
			//parse url?

			var html = PlaylistUrl.Text;

			HtmlWeb web = new ();

			var htmlDoc = web.Load(html);
			
			// img from page
			var imgSrc = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='h-100 image']/img").Attributes["src"].Value;
			PlaylistImage.Source = await BitmapImageFromUrl(imgSrc);

			var productInfoNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'productInfo')]");
			
			var listingInfoNode = productInfoNode.SelectSingleNode("//div[contains(@class, 'listing')]");
			var title = listingInfoNode.SelectSingleNode("//h1[@class='artist']").InnerHtml;
			var songsRaw = listingInfoNode.SelectSingleNode("//h2[@class='tracks']").InnerHtml;
			var songsList = songsRaw.Split("<br>").Select(s => s.Replace("\"", "")).ToList();

			var playlistLabel = productInfoNode.SelectSingleNode("//div[@class='details']").SelectSingleNode("//p[contains(text(), 'Label')]/span/a").InnerHtml;

			PlaylistName.Text = "Title : " + title;
			PlaylistReleaseDate.Text = "Release Date : ";
			PlaylistGenre.Text = "Genre : ";
			PlaylistRecordLabel.Text = "Label : " + playlistLabel;

			string artistAlbumSplitString = " - ";
			var artistName = title[..title.IndexOf(artistAlbumSplitString, StringComparison.CurrentCultureIgnoreCase)];
			var albumName = title.Remove(0, artistName.Length + artistAlbumSplitString.Length);

			List<Song> trackList = songsList.Select(songTitle => new Song(songTitle, artistName, albumName)).ToList();

			SongsList.Items = trackList.Select(t => new 
			{
				SongName = "Title : " + t.SongName,
				ArtistName = "Artist : " + t.ArtistName,
				AlbumName = "Album : " + t.AlbumName,
				Duration = !t.Duration.IsNullOrWhiteSpace() ? "Duration : " + t.Duration : null,
				Genre = !t.Genre.IsNullOrWhiteSpace() ? "Genre : " + t.Genre : null
			});

			//HtmlPageContent.Text = htmlDoc.ParsedText;

			//var node = htmlDoc.DocumentNode.SelectSingleNode("//head/title");
		}

		private async Task<Bitmap> BitmapImageFromUrl(string url)
		{
			using (WebClient client = new WebClient())
			{
				byte[] imageBytes = await client.DownloadDataTaskAsync(url);

				return new Bitmap(new MemoryStream(imageBytes));
			}
		}

		private void PlaylistUrl_OnGotFocus(object? sender, GotFocusEventArgs e)
		{
			Task.Run(() =>
			{
				while (true)
				{
					if (PlaylistUrl.Text is string playlistUri && !playlistUri.IsNullOrWhiteSpace())
					{
						Dispatcher.UIThread.InvokeAsync(() => { ParseButton.IsEnabled = true; });
						break;
					}
					Thread.Sleep(100);
				}
			});

		}
	}
}
