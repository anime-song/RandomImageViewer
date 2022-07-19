using Microsoft.WindowsAPICodePack.Dialogs;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RandomImageViewer.WPF
{
    /// <summary>
    /// OptionSelectWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class OptionSelectWindow : Window
    {
        public ReactiveProperty<string> SelectedFolderPath { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> FoundImageFilesCount { get; } = new ReactiveProperty<string>();

        public ReactiveProperty<string> IntervalInSeconds { get; } = new ReactiveProperty<string>();

        public ReactiveCommand SelectImageFolderCommand { get; }
        public ReactiveCommand StartCommand { get; }

        private ImageSelector imageSelector;

        public OptionSelectWindow()
        {
            InitializeComponent();
            DataContext = this;

            SelectedFolderPath.SetValidateNotifyError(x =>
            {
                if (string.IsNullOrEmpty(x))
                {
                    return "フォルダを選択してください";
                }

                return null;
            });

            IntervalInSeconds.SetValidateNotifyError(x =>
            {
                if (string.IsNullOrEmpty(x))
                {
                    return "間隔を入力してください";
                }

                bool failed = !int.TryParse(x, out var interval);
                if (failed)
                {
                    return "間隔は数字で入力してください";
                }

                if (interval < 1)
                {
                    return "間隔は1秒以上で入力してください";
                }

                return null;
            });

            SelectImageFolderCommand = new ReactiveCommand()
                .WithSubscribe(() =>
                {
                    using (var dialog = new CommonOpenFileDialog()
                    {
                        Title = "フォルダを選択してください",
                        RestoreDirectory = true,
                        IsFolderPicker = true,
                    })
                    {
                        if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
                        {
                            return;
                        }

                        SelectedFolderPath.Value = dialog.FileName;
                        var imageFolder = new DirectoryInfo(dialog.FileName);
                        imageSelector = new ImageSelector(imageFolder);

                        FoundImageFilesCount.Value = imageSelector.ImageFilesCount.ToString();
                    }
                });

            var canExecute = SelectedFolderPath.ObserveHasErrors
                .CombineLatest(IntervalInSeconds.ObserveHasErrors, (x, y) => !x && !y);

            StartCommand = new ReactiveCommand(canExecute)
                .WithSubscribe(() =>
                {
                    var imageView = new ImageViewWindow(imageSelector, int.Parse(IntervalInSeconds.Value));
                    Hide();
                    imageView.ShowDialog();
                    Show();
                });
        }
    }
}