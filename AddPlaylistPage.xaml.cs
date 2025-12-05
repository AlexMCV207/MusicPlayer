using System.Collections.ObjectModel;

namespace MusicPlayer
{
    public partial class AddPlaylistPage : ContentPage
    {
        private ObservableCollection<Playlist> _playlists;
        public AddPlaylistPage(ObservableCollection<Playlist> playlists)
        {
            InitializeComponent();
            _playlists = playlists;
        }
        private void AddClicked(object sender, EventArgs e)
        {
            _playlists.Add(new Playlist
            {
                Name = NameEntry.Text,
                Author = AuthorEntry.Text,
                SongAmount = int.Parse(AmountEntry.Text),
                Length = "d³ugoœæ: " + LengthEntry.Text,
                Image = ImageEntry.Text
            });

            Navigation.PopAsync();
        }
    }
}