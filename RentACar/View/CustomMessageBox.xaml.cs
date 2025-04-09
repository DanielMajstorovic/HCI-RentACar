using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RentACar.View
{
    public enum MessageType
    {
        Info,
        Success,
        Warning,
        Error,
        Question
    }

    public partial class CustomMessageBox : Window
    {
        public CustomMessageBox(string title, string message, MessageType type, bool showCancelButton = false)
        {
            InitializeComponent();

            TitleTextBlock.Text = title;
            MessageTextBlock.Text = message;

            SetMessageIcon(type);

            if (showCancelButton)
            {
                CancelButton.Visibility = Visibility.Visible;
            }
        }

        private void SetMessageIcon(MessageType type)
        {
            string iconPath;

            switch (type)
            {
                case MessageType.Success:
                    iconPath = "pack://application:,,,/Res/success.png";
                    break;
                case MessageType.Warning:
                    iconPath = "pack://application:,,,/Res/warning.png";
                    break;
                case MessageType.Error:
                    iconPath = "pack://application:,,,/Res/error.png";
                    break;
                case MessageType.Question:
                    iconPath = "pack://application:,,,/Res/question.png";
                    break;
                case MessageType.Info:
                default:
                    iconPath = "pack://application:,,,/Res/info.png";
                    break;
            }

            try
            {
                IconImage.Source = new BitmapImage(new Uri(iconPath));
            }
            catch (Exception)
            {
                IconImage.Visibility = Visibility.Collapsed;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}