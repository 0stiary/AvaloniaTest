using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
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
			PlayListBioContainer.IsVisible = false;
		}

		private void ParseButton_OnClick(object? sender, RoutedEventArgs e)
		{
			this.Cursor = new Cursor(StandardCursorType.Wait);
			var isError = false;

			try
			{
				var htmlDoc = new HtmlWeb().Load(PlaylistUrl.Text);
				var playlistInfoNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'productInfo')]");
				var trackListingInfoNode = GetPlayListInfo(playlistInfoNode, out string playlistTitle, out string playlistLabel, out Bitmap playlistImage);

				PlaylistImage.Source = playlistImage;
				PlaylistName.Text = playlistTitle;
				PlaylistReleaseDate.Text = null;
				PlaylistGenre.Text = null;
				PlaylistRecordLabel.Text = playlistLabel;

				TrackList.Items = GetTrackListFromNode(trackListingInfoNode, playlistTitle);
			}
			catch (Exception)
			{
				PlaylistUrl.BorderBrush = Brushes.Red;
				PlaylistUrl.BorderThickness = new Thickness(3);
				PlaylistUrl.Text = null;
				ParseButton.IsEnabled = false;
				isError = true; 
				return;
			}
			finally
			{
				PlayListBioContainer.IsVisible = !isError;
				this.Cursor = null;
			}
		}

		private HtmlNode GetPlayListInfo(HtmlNode playlistInfoNode, out string playlistTitle, out string playlistLabel, out Bitmap playlistImage)
		{
			var listingInfoNode = playlistInfoNode.SelectSingleNode("//div[contains(@class, 'listing')]");

			playlistImage =  BitmapImageFromUrl(playlistInfoNode.SelectSingleNode("//div[@class='h-100 image']/img").Attributes["src"].Value);
			playlistTitle = listingInfoNode.SelectSingleNode("//h1[@class='artist']").InnerHtml;
			playlistLabel = playlistInfoNode.SelectSingleNode("//div[@class='details']")
				.SelectSingleNode("//p[contains(text(), 'Label')]/span/a").InnerHtml;
			
			return listingInfoNode;
		}


		private IEnumerable<Song> GetTrackListFromNode(HtmlNode listingNode, string playlistTitle)
		{
			var songsList = listingNode.SelectSingleNode("//h2[@class='tracks']").InnerHtml.Split("<br>")
										.Select(s => s.Replace("\"", "")).ToList();

			const string artistAlbumSplitString = " - ";
			var artistName = playlistTitle[..playlistTitle.IndexOf(artistAlbumSplitString, StringComparison.CurrentCultureIgnoreCase)];
			var albumName = playlistTitle.Remove(0, artistName.Length + artistAlbumSplitString.Length);


			return songsList.Select(songTitle => new Song(songTitle, artistName, albumName)).ToList();
		}

		private Bitmap BitmapImageFromUrl(string url)
		{
			using WebClient client = new();
			return new Bitmap(new MemoryStream(client.DownloadData(url)));
		}

		private void PlaylistUrl_OnGotFocus(object? sender, GotFocusEventArgs e)
		{
			Task.Run(() =>
			{
				while (true)
				{
					if (!PlaylistUrl.Text.IsNullOrWhiteSpace())
					{
						Dispatcher.UIThread.InvokeAsync(() => { 
							ParseButton.IsEnabled = true; 
							PlaylistUrl.BorderBrush = Brushes.Black;
							PlaylistUrl.BorderThickness = new Thickness(1);
						});
						break;
					}
					Thread.Sleep(100);
				}
			});

		}
	}
}
