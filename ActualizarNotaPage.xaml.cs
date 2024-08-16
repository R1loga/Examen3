using Microsoft.Maui.Controls;
using Examen3.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using Examen3.Services;

namespace Examen3
{
    public partial class ActualizarNotaPage : ContentPage
    {
        private Nota _nota;

        public ActualizarNotaPage(Nota nota)
        {
            InitializeComponent();
            _nota = nota;
            PopulateFields();
        }

        private void PopulateFields()
        {
            DescripcionEditor.Text = _nota.Descripcion;
            FechaPicker.Date = DateTimeOffset.FromUnixTimeSeconds((long)_nota.Fecha).DateTime;

            if (_nota.PhotoRecord != null)
            {
                //PhotoImage = ImageSource.FromStream(() => new MemoryStream(_nota.PhotoRecord));//
            }
        }

        private async void OnTakePhotoClicked(object sender, EventArgs e)
        {
            try
            {
                var photo = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Select a photo"
                });

                if (photo != null)
                {
                    var stream = await photo.OpenReadAsync();
                    PhotoImage.Source = ImageSource.FromStream(() => stream);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Could not take photo: {ex.Message}", "OK");
            }
        }

        private async void OnUpdateNotaClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(DescripcionEditor.Text))
            {
                await DisplayAlert("Error", "La descripción no puede estar vacía.", "OK");
                return;
            }

            _nota.Descripcion = DescripcionEditor.Text;
            _nota.Fecha = GetTimestamp(FechaPicker.Date);
            _nota.PhotoRecord = await GetImageBytes(PhotoImage);

            var firebaseService = new FirebaseService();
            await firebaseService.UpdateNotaAsync(_nota);

            await DisplayAlert("Éxito", "Nota actualizada correctamente.", "OK");
            await Navigation.PopAsync();
        }

        private double GetTimestamp(DateTime dateTime)
        {
            return new DateTimeOffset(dateTime).ToUnixTimeSeconds();
        }

        private async Task<byte[]> GetImageBytes(Image image)
        {
            if (image.Source is StreamImageSource streamImageSource)
            {
                using var stream = await streamImageSource.Stream(CancellationToken.None);
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
            return Array.Empty<byte>();
        }

    }
}
