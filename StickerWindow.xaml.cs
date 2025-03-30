using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;

namespace StickerApp
{
    public partial class StickerWindow : Window
    {
        public StickerWindow(string imagePath)
        {
            InitializeComponent();

            try
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(imagePath, UriKind.Absolute);
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();

                ImageBehavior.SetAnimatedSource(StickerImage, image);
            }
            catch
            {
                // Imagen inválida
            }

            MouseLeftButtonDown += (s, e) =>
            {
                if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                    DragMove();
            };

            Loaded += (s, e) =>
            {
                var hWnd = new WindowInteropHelper(this).Handle;
                IntPtr HWND_TOPMOST = new IntPtr(-1);
                const UInt32 SWP_NOSIZE = 0x0001;
                const UInt32 SWP_NOMOVE = 0x0002;
                const UInt32 SWP_SHOWWINDOW = 0x0040;

                SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0,
                    SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
            };
        }

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
            int X, int Y, int cx, int cy, uint uFlags);
    }
}
