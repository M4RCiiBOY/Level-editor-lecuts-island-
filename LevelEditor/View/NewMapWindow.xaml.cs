using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LevelEditor.Control;

namespace LevelEditor.View
{
    public partial class NewMapWindow : Window
    {
        private readonly App controller;
        public string SelectedMapTileType { get; set; }
        public NewMapWindow()
        {
            InitializeComponent();

            this.controller = (App)Application.Current;
        }
        public void SetMapTileTypes(IEnumerable<string> mapTileTypes)
        {
            //foreach(var mapTileType in mapTileTypes)
            //{
            //    var radioButton = new RadioButton();
            //    radioButton.Content = mapTileType;
            //    radioButton.GroupName = "mapTileTypes";
            //    radioButton.Checked += OnMapTileSelected;
            //    radioButton.IsChecked = true;
            //    this.DefaultTitlePanel.Children.Add(radioButton);
            //}
            this.SelectedMapTileType = "17";
        }
        private void OnMapTileSelected(object sender, RoutedEventArgs e)
        {
            RadioButton radiobutton = (RadioButton)sender;
            this.SelectedMapTileType = radiobutton.Content.ToString();
        }
        private void NewMapButton_Click(object sender, RoutedEventArgs e)
        {
            this.controller.CreateMap();
        }
    }
}
