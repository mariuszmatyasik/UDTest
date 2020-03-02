using System.Windows;

namespace UDTest
{
    /// <summary>
    /// Logika interakcji dla klasy TestFrame.xaml
    /// </summary>
    public partial class TestFrame : Window
    {
        MainWindow tmp = new MainWindow();
        string url;
        public TestFrame(MainWindow tmp, string url)
        {
            InitializeComponent();
            this.tmp = tmp;
            this.url = url;
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            tmp.Show();
        }
    }
}
