using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Examen3.Models; 
using Examen3.Services; 
using Examen3;

namespace Examen3.Views
{
    public partial class ListaNotasPage : ContentPage
    {
        private readonly FirebaseService _firebaseService;
        private ObservableCollection<Nota> _notas;

        public ListaNotasPage()
        {
            InitializeComponent();
            _firebaseService = new FirebaseService();
            CargarNotas();
        }

        private async void CargarNotas()
        {
            try
            {
                var notas = await _firebaseService.LeerNotasAsync();
                _notas = new ObservableCollection<Nota>(notas);
                notasListView.ItemsSource = _notas;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudieron cargar las notas: {ex.Message}", "OK");
            }
        }

        private async void OnActualizarClicked(object sender, EventArgs e)
        {
            try
            {
                var swipeItem = sender as SwipeItem;
                var nota = swipeItem?.BindingContext as Nota;

                if (nota != null)
                {
                    
                    await Navigation.PushAsync(new ActualizarNotaPage(nota));
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo obtener la nota para actualizar.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
            }
        }

        private async void OnEliminarClicked(object sender, EventArgs e)
        {
            try
            {
                var swipeItem = sender as SwipeItem;
                var nota = swipeItem?.BindingContext as Nota;

                if (nota != null)
                {
                    var confirm = await DisplayAlert("Confirmar Eliminación", "¿Estás seguro de que quieres eliminar esta nota?", "Sí", "No");
                    if (confirm)
                    {
                        await _firebaseService.EliminarNotaAsync(nota.IdNota);
                        await DisplayAlert("Éxito", "Nota eliminada exitosamente", "OK");
                        _notas.Remove(nota); 
                    }
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo obtener la nota para eliminar.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
            }
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            
            ((ListView)sender).SelectedItem = null;
        }
    }
}
