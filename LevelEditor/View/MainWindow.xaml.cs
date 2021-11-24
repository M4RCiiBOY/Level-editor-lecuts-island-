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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LevelEditor.View
{
    using System.Windows;
    using LevelEditor.Control;
    using LevelEditor.Model;
    public partial class MainWindow : Window
    {
        private readonly App controller;
        public int TileSizeInPixels = 32;
        private bool drawEnabled;
        private Image[,] canvasImages;
        BrushWindow brushWindow;

        public MainWindow()
        {
            this.controller = (App)Application.Current;
            this.brushWindow = this.controller.brushWindow;

            InitializeComponent();
        }
        public void SetMapTileTypes(IEnumerable<string> mapTileTypes)
        {

            this.BrushSelectionPanel.Children.Clear();
            
            foreach (var mapTileType in mapTileTypes)
            {
                var radioButton = new RadioButton();
                radioButton.Content = mapTileType;
                radioButton.GroupName = "irrelevant";
                radioButton.Checked += OnBrushSelected;
                this.BrushSelectionPanel.Children.Add(radioButton);
            }
        }

        public void UpdateMapCanvas(Vector2l position,BitmapImage tileImage)
        {
            this.canvasImages[position.X, position.Y].Source = tileImage;
        }
        public void UpdateMapCanvas(Map map)
        {
            MapCanvas.Children.Clear();
            MapCanvas.Width = map.Width * TileSizeInPixels;
            MapCanvas.Height = map.Height * TileSizeInPixels;
            canvasImages = new Image[map.Width, map.Height];
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    var mapTile = map[x, y];
                    var image = new Image
                    {
                        Width = TileSizeInPixels,
                        Height = TileSizeInPixels,
                        Source = this.controller.GetTileImage(mapTile.Type),
                        Tag = new Vector2l(x, y)
                    };
                    MapCanvas.Children.Add(image);
                    canvasImages[x, y] = image;
                    Canvas.SetLeft(image, x * TileSizeInPixels);
                    Canvas.SetTop(image, y * TileSizeInPixels);
                    image.MouseLeftButtonDown += OnBrushDown;
                    image.MouseLeftButtonUp += OnBrushUp;
                    image.MouseMove += OnTileHold;
                    image.MouseLeftButtonDown += OnTileClicked;
                }
            }
        }

        private void OnTileClicked(object sender, MouseButtonEventArgs e)
        {
            var tile = (Image)sender;
            var position = (Vector2l)tile.Tag;
            this.controller.OnTileClicked(position);
        }

        private void OnTileHold(object sender, RoutedEventArgs e)
        {
            if (!this.drawEnabled) return;
            var tile = (Image)sender;
            var position = (Vector2l)tile.Tag;
            this.controller.OnTileClicked(position);
        }
        private void OnBrushDown(object sender, RoutedEventArgs e)
        {
            this.drawEnabled = true;
        }
        private void OnBrushUp(object sender, RoutedEventArgs e)
        {
            this.drawEnabled = false;
        }
        private void OnBrushSelected(object sender, RoutedEventArgs e)
        {

            RadioButton radiobutton = (RadioButton)sender;
            var imageURI = "pack://application:,,,/Resources/Tiles/forest_64/" + radiobutton.Content.ToString()+".png";
            Image image = new Image();
            BitmapImage imageBmp = new BitmapImage();
            imageBmp.BeginInit();
            imageBmp.UriSource = new Uri(imageURI);
            imageBmp.EndInit();
            image.Source = imageBmp;
            this.previewImage.Children.Clear();
            this.previewImage.Children.Add(image);
            this.controller.OnBrushSelected(radiobutton.Content.ToString());
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            controller.Quit();
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            controller.About();
        }
        #region COMMANDS
        private void CommandExecuteNew(object sender, RoutedEventArgs e)
        {
            this.controller.CreateNew();
        }
        private void CommandCanExecuteNew(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void CommandExecuteSaveAs(object sender, RoutedEventArgs e)
        {
            this.controller.SaveAs();
        }
        private void CommandCanExecuteSaveAs(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void CommandExecuteOpen(object sender, RoutedEventArgs e)
        {
            this.controller.Open();
            
        }
        private void CommandCanExecuteOpen(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void CommandExecuteHelp(object sender, RoutedEventArgs e)
        {
            this.controller.About();
        }
        private void CommandCanExecuteHelp(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void CommandExecuteClose(object sender, RoutedEventArgs e)
        {
            this.controller.Quit();
        }
        private void CommandCanExecuteClose(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        #endregion

        private void ObjectLayerButton_Click(object sender, RoutedEventArgs e)
        {
            controller.ObjectEditing();
        }

        internal void UpdateRecentFiles()
        {
            MenuItem item = new MenuItem();
            this.menuItemHeader.Items.Add(item);
        }

        private void MapCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            this.drawEnabled = false;
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            TileSizeInPixels +=8;
            controller.UpdateMapCanvas();
            
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            TileSizeInPixels -= 8;
            controller.UpdateMapCanvas();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
        }

        private void Window_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                TileSizeInPixels += 8;
            }
            else
            {
                if (TileSizeInPixels == 8)
                {
                    return;
                }
                TileSizeInPixels -= 8;
            }
            controller.UpdateMapCanvas();
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {

        }

        private void Brushes_Click(object sender, RoutedEventArgs e)
        {
            if (this.brushWindow == null || !this.brushWindow.IsLoaded)
            {
                this.controller.CreateBrushWindow();
            }
        }
    }
}
