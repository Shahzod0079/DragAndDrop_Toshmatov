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
        private int currentFilledSlots = 56;
        private int currentMoney = 14633098;

        public Inventory()
        {
            InitializeComponent();
            UpdateSlotsDisplay();
            UpdateMoneyDisplay();
        }

        private void UpdateSlotsDisplay()
        {
            SlotsText.Text = $"{currentFilledSlots}/80";
        }

        private void UpdateMoneyDisplay()
        {
            MoneyText.Text = currentMoney.ToString("N0");
        }

        //  уменьшаем деньги при перетаскивании
        private void UpdateMoney(int amount)
        {
            currentMoney = amount;
            UpdateMoneyDisplay();
        }

        // Добавляем или убавляем деньги
        private void AddMoney(int amount)
        {
            currentMoney += amount;
            UpdateMoneyDisplay();
        }

        private void SubtractMoney(int amount)
        {
            currentMoney -= amount;
            UpdateMoneyDisplay();
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
                    // Обновляем слоты
                    if (targetBorder.Child == null && sourceBorder.Child != null)
                    {
                        currentFilledSlots++;
                    }
                    else if (targetBorder.Child != null && sourceBorder.Child == null)
                    {
                        currentFilledSlots--;
                    }

                    var temp = sourceBorder.Child;
                    sourceBorder.Child = targetBorder.Child;
                    targetBorder.Child = temp;

                    UpdateSlotsDisplay();

                    // Обновляем деньги при пеертаскивании
                    SubtractMoney(1000);
                }
                targetBorder.Background = new SolidColorBrush(Color.FromRgb(58, 36, 24));
                draggedImage = null;
                sourceBorder = null;
            }
        }

        // Клик по золоту - добавляем деньги
        private void OnGoldClicked(object sender, MouseButtonEventArgs e)
        {
            AddMoney(5000);
        }

        private void CloseWindow(object sender, RoutedEventArgs e) => this.Close();
        private void BackToMain(object sender, RoutedEventArgs e) => this.Close();
    }
}