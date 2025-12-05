using System.Collections.ObjectModel; 

namespace MusicPlayer
{
    public partial class AddSongPage : ContentPage
    {
        private ObservableCollection<Song> _songs;

        private string _selectedFilePath;
        private string _selectedImagePath;
        public AddSongPage(ObservableCollection<Song> songs)
        {
            InitializeComponent();
            _songs = songs;
        }

        private async void AddSongClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_selectedFilePath))
            {
                await DisplayAlert("B³¹d", "Musisz wybraæ plik audio.", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(_selectedImagePath))
            {
                DisplayAlert("B³¹d", "Musisz wybraæ obraz.", "OK");
                return;
            }
            if(string.IsNullOrWhiteSpace(SongNameEntry.Text) || string.IsNullOrWhiteSpace(PerformerEntry.Text))
            {
                DisplayAlert("B³¹d", "Musisz uzupe³niæ wszystkie pola", "OK");
                return;
            }
            _songs.Add(new Song
            {
                sname = SongNameEntry.Text,
                performer = PerformerEntry.Text,
                Image = _selectedImagePath,
                filePath = _selectedFilePath
            });

            Navigation.PopAsync();

            await Navigation.PopAsync();
        }
        private async void PickFileClicked(object sender, EventArgs e)
        {
            FilePickerFileType AudioFileTypes = new FilePickerFileType(
            new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.Android, new[] { "audio/*" } },
                { DevicePlatform.iOS, new[] { "public.audio" } },       // UTType dla audio
                { DevicePlatform.WinUI, new[] { ".mp3", ".wav", ".m4a", ".flac" } },
                { DevicePlatform.MacCatalyst, new[] { "public.audio" } },
            });
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Wybierz plik audio",
                FileTypes = AudioFileTypes

            });

            if (result != null)
            {
                _selectedFilePath = result.FullPath;
                SelectedFileLabel.Text = $"Wybrano: {result.FileName}";
            }
        }
        private async void PickImageClicked(object sender, EventArgs e)
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Wybierz obraz",
                FileTypes = FilePickerFileType.Images
            });

            if (result != null)
            {
                _selectedImagePath = result.FullPath;

                SelectedImagePreview.Source = _selectedImagePath;
                SelectedImagePreview.IsVisible = true;

                SelectedImageLabel.Text = $"Wybrano: {result.FileName}";
            }
        }
    }
}