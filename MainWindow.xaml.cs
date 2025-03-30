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
        public class ImagenItem
        {
            public string Path { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();
            CargarImagenes();
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

        private void MostrarSticker_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string path)
            {
                new StickerWindow(path).Show();
            }
        }

        private void EliminarImagen_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string path)
            {
                var lista = new List<ImagenItem>(ListaImagenes.ItemsSource as IEnumerable<ImagenItem>);
                lista.RemoveAll(i => i.Path == path);
                ListaImagenes.ItemsSource = lista;
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}
