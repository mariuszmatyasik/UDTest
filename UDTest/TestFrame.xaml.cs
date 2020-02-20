using System.Windows;

namespace UDTest
{
    /// <summary>
    /// Logika interakcji dla klasy TestFrame.xaml
    /// </summary>
    public partial class TestFrame : Window
    {
        MainWindow tmp = new MainWindow();

        public TestFrame(MainWindow tmp)
        {
            InitializeComponent();
            this.tmp = tmp;
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            tmp.Show();
        }
    }
}
