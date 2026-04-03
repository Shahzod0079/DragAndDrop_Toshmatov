using System.Windows;
using System.Windows.Threading;


namespace DragAndDrop_Toshmatov
{

    public partial class MainWindow : Window
    {
        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            dispatcherTimer.Tick += DispatcherTimer_Tick; 
            dispatcherTimer.Interval = new System.TimeSpan(0, 0, 0, 0, 1000 / 60); 
        }
        private void DispatcherTimer_Tick(object sender, System.EventArgs e)
        {

        }

        private void image_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
        private void Image_MouseUDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }
}