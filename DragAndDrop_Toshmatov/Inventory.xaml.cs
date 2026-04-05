using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace DragAndDrop_Toshmatov
{
    public partial class Inventory : Window
    {
        private Border? draggedItem;
        private Point startPoint;

        public Inventory()
        {
            InitializeComponent();
            this.Loaded += (s, e) => InitializeDragDrop();
        }

        private void InitializeDragDrop()
        {
            var allGrids = FindVisualChildren<Grid>(this);
            foreach (var grid in allGrids)
            {
                if (grid.Parent is UniformGrid || (grid.Parent is Border && ((Border)grid.Parent).Parent is UniformGrid))
                {
                    grid.MouseLeftButtonDown += Item_MouseLeftButtonDown;
                    grid.MouseMove += Item_MouseMove;
                    grid.AllowDrop = true;
                    grid.Drop += Item_Drop;
                    grid.DragEnter += Item_DragEnter;
                    grid.DragLeave += Item_DragLeave;
                }
            }
        }

        private void Item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            draggedItem = sender as Border;
            startPoint = e.GetPosition(null);
        }

        private void Item_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && draggedItem != null)
            {
                Point currentPoint = e.GetPosition(null);
                if (Math.Abs(currentPoint.X - startPoint.X) > 5 || Math.Abs(currentPoint.Y - startPoint.Y) > 5)
                {
                    DragDrop.DoDragDrop(draggedItem, draggedItem, DragDropEffects.Move);
                    draggedItem = null;
                }
            }
        }

        private void Item_DragEnter(object sender, DragEventArgs e)
        {
            if (sender is Grid targetGrid)
            {
                targetGrid.Background = new SolidColorBrush(Color.FromRgb(100, 70, 40));
                e.Effects = DragDropEffects.Move;
            }
        }

        private void Item_DragLeave(object sender, DragEventArgs e)
        {
            if (sender is Grid targetGrid)
            {
                targetGrid.Background = Brushes.Transparent;
            }
        }

        private void Item_Drop(object sender, DragEventArgs e)
        {
            if (sender is Grid targetGrid && e.Data.GetData(typeof(Border)) is Border sourceBorder)
            {
                var sourceGrid = sourceBorder.Parent as Grid;
                if (sourceGrid != null && sourceGrid != targetGrid)
                {
                    var sourceImage = sourceGrid.Children.OfType<Image>().FirstOrDefault();
                    var targetImage = targetGrid.Children.OfType<Image>().FirstOrDefault();

                    if (sourceImage != null && targetImage != null)
                    {
                        var tempSource = sourceImage.Source;
                        sourceImage.Source = targetImage.Source;
                        targetImage.Source = tempSource;

                        var sourceText = sourceGrid.Children.OfType<TextBlock>().FirstOrDefault();
                        var targetText = targetGrid.Children.OfType<TextBlock>().FirstOrDefault();

                        if (sourceText != null && targetText != null)
                        {
                            var tempText = sourceText.Text;
                            sourceText.Text = targetText.Text;
                            targetText.Text = tempText;
                        }
                    }
                }
                targetGrid.Background = Brushes.Transparent;
            }
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BackToMain(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                if (child is T t) yield return t;
                foreach (var childOfChild in FindVisualChildren<T>(child)) yield return childOfChild;
            }
        }
    }
}