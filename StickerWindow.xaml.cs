using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;

namespace StickerApp
{
    public partial class StickerWindow : Window
    {
        private BitmapImage bitmapOriginal; // ← 🔹 Imagen original cargada

        public StickerWindow(string imagePath, int escalaPorcentaje)
        {
            InitializeComponent();

            try
            {
                string rutaAbsoluta = Path.GetFullPath(imagePath);

                if (File.Exists(rutaAbsoluta))
                {
                    bitmapOriginal = new BitmapImage(); // ← 🔹 Guardamos la imagen
                    bitmapOriginal.BeginInit();
                    bitmapOriginal.UriSource = new Uri(rutaAbsoluta, UriKind.Absolute);
                    bitmapOriginal.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapOriginal.EndInit();

                    ImageBehavior.SetAnimatedSource(StickerImage, bitmapOriginal);

                    AplicarEscala(escalaPorcentaje); // ← 🔹 Escala inicial
                }
                else
                {
                    MessageBox.Show($"No se encontró la imagen: {rutaAbsoluta}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar la imagen:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }

            // Permite arrastrar el sticker
            MouseLeftButtonDown += (s, e) =>
            {
                if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                    DragMove();
            };

            // Siempre encima
            Loaded += (s, e) =>
            {
                var hWnd = new WindowInteropHelper(this).Handle;
                SetWindowPos(hWnd, new IntPtr(-1), 0, 0, 0, 0,
                    SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
            };
        }

        private void AplicarEscala(int porcentaje)
        {
            if (bitmapOriginal == null) return;

            double factor = porcentaje / 100.0;
            StickerImage.Width = bitmapOriginal.PixelWidth * factor;
            StickerImage.Height = bitmapOriginal.PixelHeight * factor;
        }

        public void ActualizarEscala(int nuevaEscala)
        {
            AplicarEscala(nuevaEscala);
        }

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
            int X, int Y, int cx, int cy, uint uFlags);

        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 SWP_SHOWWINDOW = 0x0040;
    }
}
