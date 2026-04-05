using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DragAndDrop_Toshmatov
{
    public partial class Inventory : Window
    {
        private Image? draggedImage;
        private Border? sourceBorder;

        public Inventory()
        {
            InitializeComponent();
        }

        private void Item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            sourceBorder = sender as Border;
            if (sourceBorder?.Child is Image img)
            {
                draggedImage = img;
            }
        }

        private void Item_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && draggedImage != null)
            {
                DragDrop.DoDragDrop(draggedImage, draggedImage, DragDropEffects.Move);
            }
        }

        private void Item_DragEnter(object sender, DragEventArgs e)
        {
            if (sender is Border target)
            {
                target.Background = new SolidColorBrush(Color.FromRgb(100, 70, 40));
                e.Effects = DragDropEffects.Move;
            }
        }

        private void Item_DragLeave(object sender, DragEventArgs e)
        {
            if (sender is Border target)
            {
                target.Background = new SolidColorBrush(Color.FromRgb(58, 36, 24));
            }
        }

        private void Item_Drop(object sender, DragEventArgs e)
        {
            if (sender is Border targetBorder && e.Data.GetData(typeof(Image)) is Image draggedImg)
            {
                if (sourceBorder != null && sourceBorder != targetBorder)
                {
                    var temp = sourceBorder.Child;
                    sourceBorder.Child = targetBorder.Child;
                    targetBorder.Child = temp;
                }
                targetBorder.Background = new SolidColorBrush(Color.FromRgb(58, 36, 24));
                draggedImage = null;
                sourceBorder = null;
            }
        }

        private void CloseWindow(object sender, RoutedEventArgs e) => this.Close();
        private void BackToMain(object sender, RoutedEventArgs e) => this.Close();
    }
}