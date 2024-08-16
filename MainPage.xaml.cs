using Microsoft.Maui.Controls;
using Examen3.Models;
using Examen3.Services;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Examen3.Views;
using Plugin.Media;
using Plugin.Media.Abstractions;

namespace Examen3
{
    public partial class MainPage : ContentPage
    {
        private readonly FirebaseService _firebaseService = new FirebaseService();
        private string _photoFilePath;
        private MediaFile _file;

        public MainPage()
        {
            InitializeComponent();
            InitializeMediaPlugin();
        }

        private async void InitializeMediaPlugin()
        {
            await CrossMedia.Current.Initialize();
        }

        private async void OnAddNotaClicked(object sender, EventArgs e)
        {
            var nota = new Nota
            {
                IdNota = await GetNextId(),
                Descripcion = DescripcionEditor.Text,
                Fecha = GetTimestamp(FechaPicker.Date),
                PhotoRecord = GetImageBytes() // Actualiza aquí
            };

            await _firebaseService.AddNotaAsync(nota);

            await DisplayAlert("Éxito", "Nota guardada correctamente.", "OK");
        }

        private async void OnTakePhotoClicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", "No camera available.", "OK");
                return;
            }

            _file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "test.jpg"
            });

            if (_file == null)
                return;

            CapturedImage.Source = ImageSource.FromStream(() =>
            {
                var stream = _file.GetStream();
                return stream;
            });
        }

        private async void OnSavePhotoClicked(object sender, EventArgs e)
        {
            if (_file == null)
            {
                await DisplayAlert("Error", "No photo to save.", "OK");
                return;
            }
            var path = Path.Combine(FileSystem.AppDataDirectory, _file.Path.Substring(_file.Path.LastIndexOf(Path.DirectorySeparatorChar) + 1));
            using (var stream = _file.GetStream())
            using (var newStream = File.OpenWrite(path))
            {
                await stream.CopyToAsync(newStream);
            }
            await DisplayAlert("Foto Guardada", $"Foto guardada en: {path}", "OK");
            CapturedImage.Source = null;
            _file.Dispose();
            _file = null;
        }
    


    private async Task<int> GetNextId()
        {
            var notas = await _firebaseService.GetNotasAsync();
            return notas.Count > 0 ? notas.Max(n => n.IdNota) + 1 : 1;
        }

        private double GetTimestamp(DateTime dateTime)
        {
            return new DateTimeOffset(dateTime).ToUnixTimeSeconds();
        }

        private byte[] GetImageBytes()
        {
            if (string.IsNullOrEmpty(_photoFilePath))
            {
                return new byte[0];
            }

            using var fileStream = File.OpenRead(_photoFilePath);
            using var memoryStream = new MemoryStream();
            fileStream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }

        private async void OnViewNotasClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ListaNotasPage());
        }
    }
}
