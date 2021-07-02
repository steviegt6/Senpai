using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Senpai
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static MainWindow Instance { get; protected set; }

        public MediaPlayer MediaPlayer { get; }

        public bool MediaPlayerOpen { get; private set; }

        public BitmapImage ImoutoImage { get; protected set; }

        public MainWindow()
        {
            InitializeComponent();

            ResizeMode = ResizeMode.NoResize;

            Instance = this;
            MediaPlayer = new MediaPlayer();

            InitializeAudio();
            InitializeImages();
            InitializeTextAndButtons();
        }

        public void InitializeAudio()
        {
            MediaPlayer.MediaOpened += (_, _) =>
            {
                MediaPlayerOpen = true;
                MediaPlayer.Play();
            };

            MediaPlayer.MediaEnded += (_, _) =>
            {
                if (MediaPlayerOpen)
                    MediaPlayer.Close();

                OpenMediaPlayer();
            };

            OpenMediaPlayer();
        }

        public void InitializeImages()
        {
            ImoutoImage = new BitmapImage(new Uri(RequestAssetPath("Assets/Imouto.png")));

            ImoutoViewer.RenderTransform = new TranslateTransform(10D, 10D);
            ImoutoViewer.Source = ImoutoImage;
        }

        public void InitializeTextAndButtons()
        {
            const double xOffset = 312D;
            const double adjustedXOffset = xOffset + 30D;
            const double textAccommodation = 130D;
            const double centralXOffset = xOffset / 2D + 10D;

            PromptMessage.RenderTransform = new TranslateTransform(xOffset / 2D + 20D, 10D);

            CardNumberMessage.RenderTransform = new TranslateTransform(adjustedXOffset, 130D);
            CardNumberBox.RenderTransform = new TranslateTransform(xOffset + textAccommodation, 130D);

            ExpirationDateMessage.RenderTransform = new TranslateTransform(adjustedXOffset, 180D);
            ExpirationDateBox.RenderTransform = new TranslateTransform(xOffset + textAccommodation, 180D);

            SecurityCodeMessage.RenderTransform = new TranslateTransform(adjustedXOffset, 230D);
            SecurityCodeBox.RenderTransform = new TranslateTransform(xOffset + textAccommodation, 230D);

            EnterButton.RenderTransform = new TranslateTransform(centralXOffset, -20D);
            ErrorMessage.RenderTransform = new TranslateTransform(centralXOffset, -40D);
        }

        public void OpenMediaPlayer() => MediaPlayer.Open(new Uri(RequestAssetPath("Assets/Dream.mp3")));

        public static string RequestAssetPath(string path) =>
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, path);

        public void ValidateCreditCardInformation(object sender, RoutedEventArgs e)
        {
            string errorMessage = "Oh noes... ";

            // while loop to simulate early return, use method with out param later lol
            while (true)
            {
                if (CardNumberBox.Text.Length == 0 || ExpirationDateBox.Text.Length == 0 ||
                    SecurityCodeBox.Text.Length == 0)
                {
                    errorMessage += "you didn't entew all dah infow!";
                    break;
                }

                if (CardNumberBox.Text.Length != 16)
                {
                    errorMessage += "cwedit cawd length must be 16!";
                    break;
                }

                if (CardNumberBox.Text.Any(c => !char.IsNumber(c)))
                {
                    errorMessage += "cwedit cawd must be alw numbahs!";
                    break;
                }

                if (ExpirationDateBox.Text.Length != 5)
                {
                    errorMessage += "bad expirashun date length!";
                    break;
                }

                bool failed = false;
                for (int i = 0; i < 5; i++)
                {
                    string text = ExpirationDateBox.Text;

                    switch (i)
                    {
                        case 2:
                            if (text[i] != '/')
                                failed = true;
                            break;

                        default:
                            if (!char.IsNumber(text[i]))
                                failed = true;
                            break;
                    }

                    if (!failed) continue;

                    errorMessage += "bad expirashun date fowmaht~";
                    break;
                }

                if (failed)
                    break;

                // american express is special and uses 4 digits
                if (SecurityCodeBox.Text.Length != 3 && SecurityCodeBox.Text.Length != 4)
                {
                    errorMessage += "cvv length must be 3 or 4!";
                    break;
                }

                if (SecurityCodeBox.Text.Any(c => !char.IsNumber(c))) 
                    errorMessage += "cvv must be alw numbahs!";

                break;
            }

            if (errorMessage == "Oh noes... ")
            {
                ErrorMessage.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                ErrorMessage.Text = "Dis is gud, thanks!";

                EnterButton.IsEnabled = false;
                CardNumberBox.IsEnabled = false;
                ExpirationDateBox.IsEnabled = false;
                SecurityCodeBox.IsEnabled = false;
            }
            else
            {
                ErrorMessage.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                ErrorMessage.Text = errorMessage;
            }
        }
    }
}