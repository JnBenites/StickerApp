using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Win32;
using System.Windows.Media;
using System.Windows.Controls.Primitives;


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
            var imagenes = new List<ImagenItem>();

            string pathEjemplo = System.IO.Path.GetFullPath("sticker1.png");

            if (System.IO.File.Exists(pathEjemplo))
            {
                imagenes.Add(new ImagenItem { Path = pathEjemplo });
            }

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
            if (sender is CheckBox chk && chk.Tag is string path)
            {
                if (chk.IsChecked == true)
                {
                    // Buscar el TextBox hermano (EscalaInput) dentro del mismo DataTemplate
                    var parent = VisualTreeHelper.GetParent(chk);
                    while (parent is not StackPanel)
                        parent = VisualTreeHelper.GetParent(parent);

                    int escala = 100;
                    var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
                    for (int i = 0; i < childrenCount; i++)
                    {
                        var child = VisualTreeHelper.GetChild(parent, i);
                        if (child is StackPanel inner)
                        {
                            for (int j = 0; j < VisualTreeHelper.GetChildrenCount(inner); j++)
                            {
                                var txt = VisualTreeHelper.GetChild(inner, j) as TextBox;
                                if (txt != null && txt.Tag as string == path)
                                {
                                    int.TryParse(txt.Text, out escala);
                                    escala = Math.Clamp(escala, 10, 500);
                                }
                            }
                        }
                    }

                    var ventana = new StickerWindow(path, escala);
                    ventana.Closed += (s, args) => ventanasAbiertas.Remove(path);
                    ventanasAbiertas[path] = ventana;
                    ventana.Show();
                }
                else
                {
                    if (ventanasAbiertas.ContainsKey(path))
                    {
                        ventanasAbiertas[path].Close();
                        ventanasAbiertas.Remove(path);
                    }
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
