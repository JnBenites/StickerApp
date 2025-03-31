// MainWindow.xaml.cs
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace StickerApp
{
    public partial class MainWindow : Window
    {
        private Dictionary<string, StickerWindow> ventanasAbiertas = new();
        private const string ConfigFile = "config.json";

        public class ImagenItem
        {
            public string Path { get; set; }
            public int Escala { get; set; } = 100;
        }

        public MainWindow()
        {
            InitializeComponent();
            CargarImagenes();
            Closing += (s, e) =>
            {
                GuardarConfiguracion();
                foreach (var v in ventanasAbiertas.Values) v.Close();
                ventanasAbiertas.Clear();
            };
        }

        private void BtnImagenes_Click(object sender, RoutedEventArgs e)
        {
            PanelImagenes.Visibility = Visibility.Visible;
            PanelAjustes.Visibility = Visibility.Collapsed;
        }

        private void BtnAjustes_Click(object sender, RoutedEventArgs e)
        {
            PanelImagenes.Visibility = Visibility.Collapsed;
            PanelAjustes.Visibility = Visibility.Visible;
        }

        private void AgregarImagen_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Imágenes|*.png;*.jpg;*.jpeg;*.gif",
                Multiselect = false
            };

            if (dlg.ShowDialog() == true)
            {
                var lista = (ListaImagenes.ItemsSource as List<ImagenItem>) ?? new();
                lista.Add(new ImagenItem { Path = dlg.FileName });
                ListaImagenes.ItemsSource = null;
                ListaImagenes.ItemsSource = lista;
            }
        }

        private void EliminarImagen_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string path)
            {
                ventanasAbiertas.Remove(path, out var win);
                win?.Close();

                var lista = (ListaImagenes.ItemsSource as List<ImagenItem>) ?? new();
                lista.RemoveAll(i => i.Path == path);
                ListaImagenes.ItemsSource = null;
                ListaImagenes.ItemsSource = lista;
            }
        }

        private void Sticker_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox chk && chk.Tag is string path)
            {
                var lista = (ListaImagenes.ItemsSource as List<ImagenItem>) ?? new();
                var img = lista.FirstOrDefault(i => i.Path == path);
                if (img == null) return;

                if (chk.IsChecked == true)
                {
                    var ventana = new StickerWindow(img.Path, img.Escala);
                    ventana.Closed += (s, args) => ventanasAbiertas.Remove(path);
                    ventanasAbiertas[path] = ventana;
                    ventana.Show();
                }
                else if (ventanasAbiertas.Remove(path, out var win))
                {
                    win.Close();
                }
            }
        }

        private void EscalaInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox txt && txt.Tag is string path && int.TryParse(txt.Text, out int escala))
            {
                escala = Math.Clamp(escala, 10, 500);
                var lista = (ListaImagenes.ItemsSource as List<ImagenItem>) ?? new();
                var img = lista.FirstOrDefault(i => i.Path == path);
                if (img != null)
                {
                    img.Escala = escala;
                    if (ventanasAbiertas.TryGetValue(path, out var win))
                        win.ActualizarEscala(escala);
                }
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        private void GuardarConfiguracion()
        {
            try
            {
                var lista = ListaImagenes.ItemsSource as List<ImagenItem>;
                File.WriteAllText(ConfigFile, JsonSerializer.Serialize(lista));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al guardar: {ex.Message}");
            }
        }

        private void CargarImagenes()
        {
            try
            {
                if (File.Exists(ConfigFile))
                {
                    var json = File.ReadAllText(ConfigFile);
                    var lista = JsonSerializer.Deserialize<List<ImagenItem>>(json) ?? new();
                    ListaImagenes.ItemsSource = lista;
                }
                else
                {
                    var pathEj = Path.GetFullPath("sticker1.png");
                    ListaImagenes.ItemsSource = File.Exists(pathEj)
                        ? new List<ImagenItem> { new ImagenItem { Path = pathEj } }
                        : new List<ImagenItem>();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al cargar: {ex.Message}");
                ListaImagenes.ItemsSource = new List<ImagenItem>();
            }
        }
    }
}
