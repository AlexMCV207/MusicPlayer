using System.Collections.ObjectModel;

namespace MusicPlayer
{
    
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Playlist> Playlists { get; set; }
        public ObservableCollection<Song> Songs { get; set; }
        
        public MainPage()
        {
            Playlists = new ObservableCollection<Playlist>();
            Songs = new ObservableCollection<Song>();
            InitializeComponent();
            BindingContext = this;
        }
        private async void AddPlaylistClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddPlaylistPage(Playlists));
        }
        private async void AddSongClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddSongPage(Songs));
        }
    }
    
}