using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RandomImageViewer.WPF
{
    public partial class ImageViewWindow : Window
    {
        public ReactiveProperty<BitmapImage> RandomImage { get; } = new ReactiveProperty<BitmapImage>();

        public ReactiveProperty<string> RemainingTime { get; } = new ReactiveProperty<string>();

        private ReactiveProperty<int> RemainingTimeInSeconds { get; } = new ReactiveProperty<int>();

        public ReactiveCommand SkipPreviousCommand { get; }
        public ReactiveCommand PlayCommand { get; }
        public ReactiveCommand StopCommand { get; }
        public ReactiveCommand SkipNextCommand { get; }

        public ImageViewWindow(
            ImageSelector imageSelector,
            int imageSwitchInterval)
        {
            InitializeComponent();
            DataContext = this;

            RemainingTimeInSeconds.Subscribe(remainingTimeInSeconds =>
            {
                TimeSpan timeSpan = new TimeSpan(0, 0, remainingTimeInSeconds);
                RemainingTime.Value = timeSpan.ToString(@"hh\:mm\:ss");
            });

            RemainingTimeInSeconds.Value = imageSwitchInterval;
            RandomImage.Value = LoadImage(imageSelector.Next());

            SkipPreviousCommand = new ReactiveCommand();

            SkipNextCommand = new ReactiveCommand();

            var timer = new ReactiveTimer(TimeSpan.FromSeconds(1));
            timer.Subscribe(x =>
            {
                RemainingTimeInSeconds.Value -= 1;
                if (RemainingTimeInSeconds.Value == 0)
                {
                    SkipNextCommand.Execute();
                    RemainingTimeInSeconds.Value = imageSwitchInterval;
                }
            });
            timer.Start();

            PlayCommand = new ReactiveCommand()
                .WithSubscribe(() =>
                {
                    if (!timer.IsEnabled)
                    {
                        timer.Start();
                    }
                });
            StopCommand = new ReactiveCommand()
                .WithSubscribe(() =>
                {
                    timer.Stop();
                });

            SkipPreviousCommand.Subscribe(() =>
            {
                RandomImage.Value = LoadImage(imageSelector.Previous());
                RemainingTimeInSeconds.Value = imageSwitchInterval;
            });

            SkipNextCommand.Subscribe(() =>
            {
                RandomImage.Value = LoadImage(imageSelector.Next());
                RemainingTimeInSeconds.Value = imageSwitchInterval;
            });
        }

        private BitmapImage LoadImage(string imgPath)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();

            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.CreateOptions = BitmapCreateOptions.None;
            bitmapImage.UriSource = new Uri(imgPath);
            bitmapImage.EndInit();

            bitmapImage.Freeze();

            return bitmapImage;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ButtonsBorder.Visibility == Visibility.Visible)
            {
                ButtonsBorder.Visibility = Visibility.Hidden;
            }
            else
            {
                ButtonsBorder.Visibility = Visibility.Visible;
            }
        }
    }
}