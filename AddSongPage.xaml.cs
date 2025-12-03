using System.Collections.ObjectModel;

namespace MusicPlayer;

public partial class AddSongPage : ContentPage
{
    private ObservableCollection<Song> _songs;

    public AddSongPage(ObservableCollection<Song> songs)
    {
        InitializeComponent();
        _songs = songs;
    }

    private void AddSongClicked(object sender, EventArgs e)
    {
        _songs.Add(new Song
        {
            sname = SongNameEntry.Text,
            performer = PerformerEntry.Text,
            Image = ImageEntry.Text
        });

        Navigation.PopAsync();
    }
}