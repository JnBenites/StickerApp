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
        public StickerWindow(string imagePath, int escalaPorcentaje)
        {
            InitializeComponent();

            try
            {
                string rutaAbsoluta = Path.GetFullPath(imagePath);

                if (File.Exists(rutaAbsoluta))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(rutaAbsoluta, UriKind.Absolute);
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();

                    // Muestra imagen animada (GIF incluido)
                    ImageBehavior.SetAnimatedSource(StickerImage, image);

                    // Aplica escala
                    StickerImage.Width = image.PixelWidth * (escalaPorcentaje / 100.0);
                    StickerImage.Height = image.PixelHeight * (escalaPorcentaje / 100.0);
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

            // Hace la ventana siempre visible (topmost)
            Loaded += (s, e) =>
            {
                var hWnd = new WindowInteropHelper(this).Handle;
                SetWindowPos(hWnd, new IntPtr(-1), 0, 0, 0, 0,
                    SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
            };
        }

        // Función nativa de Windows para mantener en topmost
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
            int X, int Y, int cx, int cy, uint uFlags);

        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 SWP_SHOWWINDOW = 0x0040;

        // Permite cambiar la escala visual del sticker en tiempo real
        public void ActualizarEscala(int nuevaEscala)
        {
            StickerImage.Width = StickerImage.Source.Width * (nuevaEscala / 100.0);
            StickerImage.Height = StickerImage.Source.Height * (nuevaEscala / 100.0);
        }
    }
}
