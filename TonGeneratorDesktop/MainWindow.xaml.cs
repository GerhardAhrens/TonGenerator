namespace TonGeneratorDesktop
{
    using System;
    using System.Globalization;
    using System.Windows;

    using TonGeneratorLibrary.Core;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CultureInfo cultureInfo = CultureInfo.CurrentUICulture;
        private Sound sound = new Sound();

        public MainWindow()
        {
            InitializeComponent();

            sound.soundVolume = 80;
            sound.OpenDevice(44100);

            this.TxtFrequenz.Text = "4500";
            this.TxtDauer.Text = "1000";
        }

        private void btnSinglePlay_Click(object sender, RoutedEventArgs e)
        {
            UInt16 frequenz = Convert.ToUInt16(this.TxtFrequenz.Text, cultureInfo);
            int dauer = Convert.ToInt32(this.TxtDauer.Text, cultureInfo);
            sound.Play(frequenz, dauer, ushort.MaxValue);
        }

        private void btnSinusPlay_Click(object sender, RoutedEventArgs e)
        {
            int frequenz = Convert.ToInt32(this.TxtSinusFrequenz.Text, cultureInfo);
            sound.Play(frequenz, sound.soundVolume);
            sound.Dispose();
        }
    }
}
