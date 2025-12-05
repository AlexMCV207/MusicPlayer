using CommunityToolkit.Maui.Media;
using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace MusicPlayer 
{ 
    public partial class MainPage : ContentPage 
    { 
        public ObservableCollection<Playlist> Playlists { get; set; } 
        public ObservableCollection<Song> Songs { get; set; }

        Song _currentSong;
        bool _isPlaying = false;
        private bool _isDragging = false;
        private int _currentIndex = -1;
        private readonly string _songsFile = Path.Combine(FileSystem.AppDataDirectory, "songs.json");
        private double _lastVolume = 0.5;
        public MainPage() 
        {
            Playlists = new ObservableCollection<Playlist>(); 
            Songs = new ObservableCollection<Song>(); InitializeComponent(); 
            BindingContext = this;
            _ = LoadSongsAsync();
            Device.StartTimer(TimeSpan.FromMilliseconds(300), () =>
            {
                if (Player?.Duration != null && !_isDragging)
                {
                    double position = Player.Position.TotalSeconds;
                    double duration = Player.Duration.TotalSeconds;

                    if (duration > 0)
                    {
                        SongSlider.Maximum = duration;
                        SongSlider.Value = position;

                        CurrentTimeLabel.Text = FormatTime(position);
                        TotalTimeLabel.Text = FormatTime(duration);
                    }
                }

                return true;
            });
        } 
        private async void AddPlaylistClicked(object sender, EventArgs e) 
        { 
            await Navigation.PushAsync(new AddPlaylistPage(Playlists)); 
        }
        private async void AddSongClicked(object sender, EventArgs e) 
        {
            await Navigation.PushAsync(new AddSongPage(Songs)); 
        }
        private void SongSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Song song)
            {
                PlaySong(song);
            }
        }
        private async void PlaySong(Song song)
        {
            _currentSong = song;
            _currentIndex = Songs.IndexOf(song);
            SongCurrentName.Text = song.sname;
            SongCurrentPerformer.Text = song.performer;
            SongCurrentImage.Source = song.Image;

            Player.Source = MediaSource.FromFile(song.filePath);
            Player.Volume = VolumeSlider.Value;
            await Task.Delay(100);
            Player.Play();

            _isPlaying = true;
            PlayPauseButton.Text = "=";
            PlayPauseButton.Rotation = 90;
        }
        private void PlayPauseClicked(object sender, EventArgs e)
        {
            if (_currentSong == null)
            {
                return;
            }

            if (_isPlaying)
            {
                Player.Pause();
                _isPlaying = false;
                PlayPauseButton.Text = "▶";
                PlayPauseButton.Rotation = 0;
                PlayPauseButton.FontSize = 20;
            }
            else
            {
                Player.Play();
                _isPlaying = true;
                PlayPauseButton.Text = "=";
                PlayPauseButton.Rotation = 90;
                PlayPauseButton.FontSize = 35;
            }
        }
        private string FormatTime(double seconds)
        {
            var time = TimeSpan.FromSeconds(seconds);
            return $"{(int)time.TotalMinutes}:{time.Seconds:D2}";
        }
        private void Player_MediaEnded(object sender, EventArgs e)
        {
            PlayRandomSong();
        }
        private void PlayRandomSong()
        {
            if (Songs == null || Songs.Count == 0)
                return;

            Random rand = new Random();
            int index = rand.Next(Songs.Count);

            var nextSong = Songs[index];

            PlaySong(nextSong);
        }
        private void NextSongClicked(object sender, EventArgs e)
        {
            if (Songs.Count == 0) return;

            _currentIndex++;

            if (_currentIndex >= Songs.Count)
                _currentIndex = 0;

            PlaySong(Songs[_currentIndex]);
        }
        private void PrevSongClicked(object sender, EventArgs e)
        {
            if (Songs.Count == 0) return;

            _currentIndex--;

            if (_currentIndex < 0)
                _currentIndex = Songs.Count - 1;

            PlaySong(Songs[_currentIndex]);
        }
        private void SongSlider_DragStarted(object sender, EventArgs e)
        {
            _isDragging = true;
        }
        private void SongSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (_isDragging)
            {
                CurrentTimeLabel.Text = FormatTime(e.NewValue);
            }
        }
        private void SongSlider_DragCompleted(object sender, EventArgs e)
        {
            _isDragging = false;

            if (Player?.Duration != null)
            {
                var seekPos = TimeSpan.FromSeconds(SongSlider.Value);
                Player.SeekTo(seekPos);
            }
        }
        private async Task SaveSongsAsync()
        {
            try
            {
                var json = JsonSerializer.Serialize(Songs);
                await File.WriteAllTextAsync(_songsFile, json);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", $"Nie udało się zapisać piosenek: {ex.Message}", "OK");
            }
        }
        private async Task LoadSongsAsync()
        {
            try
            {
                if (File.Exists(_songsFile))
                {
                    var json = await File.ReadAllTextAsync(_songsFile);
                    var loadedSongs = JsonSerializer.Deserialize<List<Song>>(json);

                    if (loadedSongs != null)
                    {
                        Songs.Clear();
                        foreach (var song in loadedSongs)
                            Songs.Add(song);
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", $"Nie udało się wczytać piosenek: {ex.Message}", "OK");
            }
        }
        private void VolumeSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (Player != null)
                Player.Volume = e.NewValue;
        }
        private void MuteButtonClicked(object sender, EventArgs e)
        {
            if (Player.Volume > 0)
            {
                _lastVolume = Player.Volume;

                Player.Volume = 0;
                VolumeSlider.Value = 0;

                MuteButton.Text = "🔇";
            }
            else
            {
                Player.Volume = _lastVolume;
                VolumeSlider.Value = _lastVolume;

                MuteButton.Text = "🔈";
            }
        }
    }
}