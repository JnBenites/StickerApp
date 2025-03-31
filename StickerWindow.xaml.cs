using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using WpfAnimatedGif;

namespace StickerApp
{
    public partial class StickerWindow : Window
    {
        private BitmapImage bitmapOriginal;
        private double baseWidth;
        private double baseHeight;

        public StickerWindow(string imagePath, int escalaPorcentaje)
        {
            InitializeComponent();

            try
            {
                string rutaAbsoluta = Path.GetFullPath(imagePath);

                if (File.Exists(rutaAbsoluta))
                {
                    bitmapOriginal = new BitmapImage();
                    bitmapOriginal.BeginInit();
                    bitmapOriginal.UriSource = new Uri(rutaAbsoluta, UriKind.Absolute);
                    bitmapOriginal.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapOriginal.EndInit();

                    ImageBehavior.SetAnimatedSource(StickerImage, bitmapOriginal);

                    // Guardamos el tamaño base original de la imagen
                    baseWidth = bitmapOriginal.PixelWidth;
                    baseHeight = bitmapOriginal.PixelHeight;

                    SetWindowSize(escalaPorcentaje);
                }
                else
                {
                    MessageBox.Show("No se encontró la imagen.");
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar imagen: {ex.Message}");
                Close();
            }

            MouseLeftButtonDown += (s, e) =>
            {
                if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                    DragMove();
            };
        }

        public void ActualizarEscala(int nuevaEscala)
        {
            SetWindowSize(nuevaEscala);
        }

        private void SetWindowSize(int porcentaje)
        {
            if (bitmapOriginal != null)
            {
                double factor = porcentaje / 100.0;
                this.Width = baseWidth * factor;
                this.Height = baseHeight * factor;
            }
        }
    }
}
