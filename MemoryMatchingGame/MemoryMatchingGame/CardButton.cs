using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Gaming.Input;
using Windows.UI.Xaml.Media.Imaging;

namespace MemoryMatchingGame
{
    class CardButton: Button
    {

        private static Image DefaultCardImageView
        {
            get
            {
                Image defaultCardImage = new Image();
                BitmapImage bitmapImage = new BitmapImage();
                Uri uri = new Uri("ms-appx:///Assets/0.png");
                bitmapImage.UriSource = uri;
                defaultCardImage.Source = bitmapImage;
                defaultCardImage.Stretch = Stretch.UniformToFill;

                return defaultCardImage;
            }
        }

        public int Value;
        public Image image;
        public bool IsFaceUp { get; private set; }
        public bool IsAlreadyMatched = false;

        // Value: [0-52]
        public CardButton(int value): base()
        {
            this.Value = value;
            this.Content = DefaultCardImageView;
            this.image = CardImageView(value);
            IsFaceUp = false;
        }

        public void FlipOver()
        {
            IsFaceUp = !IsFaceUp;
            if (IsFaceUp) {
                this.Content = image;
            } else
            {
                this.Content = DefaultCardImageView;
            }
        }

        /// forValue must be between 1 and 52
        private Image CardImageView(int forValue)
        {
            Image image = new Image();
            BitmapImage bitmapImage = new BitmapImage();
            Uri uri = new Uri($"ms-appx:///Assets/{forValue}.png");
            bitmapImage.UriSource = uri;
            image.Source = bitmapImage;
            image.Stretch = Stretch.UniformToFill;

            return image;
        }

    }
}
