using DualListSerialization.Models;
using DualListSerialization.Views;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;



namespace DualListSerialization
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ListRandom _currentList = new ListRandom();
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            Settings settingsWindow = new Settings();
            settingsWindow.Owner = this;
            settingsWindow.ShowDialog();
        }
        private void Deserialize_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Text files (*.txt)|*.txt";

            if (dialog.ShowDialog() == true)
            {
                
                InputTextBox.Text = File.ReadAllText(dialog.FileName);
                using (Stream stream = File.OpenRead(dialog.FileName))
                    _currentList.Deserialize(stream);
                _currentList.Current = _currentList.Head;
                UpdateCurrentElement();
            }
        }

        private void Serialize_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Text files (*.txt)|*.txt";
            if (dialog.ShowDialog() == true)
            {

                using (Stream stream = File.OpenWrite(dialog.FileName))
                    _currentList.Serialize(stream);
                
            }
            
        }

        private void GoToStart_Click(object sender, RoutedEventArgs e)
        {
            _currentList.MoveToHead();
            UpdateCurrentElement();
        }

        private void GoToEnd_Click(object sender, RoutedEventArgs e)
        {
            _currentList.MoveToTail();
            UpdateCurrentElement();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            _currentList.MoveNext();
            UpdateCurrentElement();
        }
        private void Random_Click(object sender, RoutedEventArgs e)
        {
            _currentList.MoveRandom();
            UpdateCurrentElement();
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            _currentList.MovePrevious();
            UpdateCurrentElement();
        }

        private void UpdateCurrentElement()
        {
            CurrentElementText.Text = _currentList.Current?.Data ?? "(empty)";
        }
    }
}
