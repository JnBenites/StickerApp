using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Win32;

namespace StickerApp
{
    public partial class MainWindow : Window
    {

        private Dictionary<string, StickerWindow> ventanasAbiertas = new();

        public class ImagenItem
        {
            public string Path { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();
            CargarImagenes();
        }


        private void EliminarImagen_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string path)
            {
                // 1. Cerrar el sticker si está abierto
                if (ventanasAbiertas.ContainsKey(path))
                {
                    ventanasAbiertas[path].Close();
                    ventanasAbiertas.Remove(path);
                }

                // 2. Eliminar la imagen de la lista
                var lista = new List<ImagenItem>(ListaImagenes.ItemsSource as IEnumerable<ImagenItem>);
                lista.RemoveAll(i => i.Path == path);
                ListaImagenes.ItemsSource = lista;
            }
        }
        private void CargarImagenes()
        {
            List<ImagenItem> imagenes = new List<ImagenItem>
            {
                new ImagenItem { Path = "sticker1.png" },
                new ImagenItem { Path = "sticker2.gif" }
            };

            ListaImagenes.ItemsSource = imagenes;
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
            OpenFileDialog dialogo = new OpenFileDialog
            {
                Filter = "Archivos de imagen|*.png;*.jpg;*.jpeg;*.gif",
                Multiselect = false
            };

            if (dialogo.ShowDialog() == true)
            {
                var lista = new List<ImagenItem>(ListaImagenes.ItemsSource as IEnumerable<ImagenItem>)
                {
                    new ImagenItem { Path = dialogo.FileName }
                };

                ListaImagenes.ItemsSource = lista;
            }
        }

        private void Sticker_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string path)
            {
                if (ventanasAbiertas.ContainsKey(path))
                {
                    ventanasAbiertas[path].Close();
                    ventanasAbiertas.Remove(path);
                }
                else
                {
                    var ventana = new StickerWindow(path);
                    ventana.Closed += (s, args) => ventanasAbiertas.Remove(path);
                    ventanasAbiertas[path] = ventana;
                    ventana.Show();
                }
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}
