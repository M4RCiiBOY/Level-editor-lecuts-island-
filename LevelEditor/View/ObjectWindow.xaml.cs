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

namespace LevelEditor.View
{
    using System.Windows;
    using LevelEditor.Control;
    using LevelEditor.Model;
    public partial class ObjectWindow : Window
    {
        private readonly App controller;
        MainWindow mainWindow;
        BrushWindow brushWindow;
        private Image[,] canvasImages;
        private Image[,] canvasObjectImages;
        private bool idEditing = false;
        private Vector2l selectedTile;
        public ObjectWindow()
        {
            this.controller = (App)Application.Current;
            this.mainWindow = (MainWindow)this.controller.MainWindow;
            this.brushWindow = this.controller.brushWindow;
            InitializeComponent();
        }
        public void SetObjectTypes(IEnumerable<string> objectTypes)
        {
            this.ObjectSelection.Children.Clear();
            foreach (var objectType in objectTypes)
            {
                var radioButton = new RadioButton();
                radioButton.Content = objectType;
                radioButton.GroupName = "irrelevant";
                radioButton.Checked += OnObjectSelected;
                this.ObjectSelection.Children.Add(radioButton);
            }

            this.ConnectionSelection.Children.Clear();
            foreach (var objectType in objectTypes)
            {
                var radioButton = new RadioButton();
                radioButton.Content = objectType;
                radioButton.GroupName = "irrelevant";
                radioButton.Checked += OnConnectionSelected;
                this.ConnectionSelection.Children.Add(radioButton);
            }
        }

        private void OnConnectionSelected(object sender, RoutedEventArgs e)
        {
            RadioButton radiobutton = (RadioButton)sender;
            if (radiobutton.Content.ToString()=="empty")
            {
                this.controller.SetObjectTileConnection(selectedTile.X, selectedTile.Y, "");
                return;
            }
            this.controller.SetObjectTileConnection(selectedTile.X, selectedTile.Y, radiobutton.Content.ToString());
        }

        private void OnObjectSelected(object sender, RoutedEventArgs e)
        {
            RadioButton radiobutton = (RadioButton)sender;
            var imageURI = "pack://application:,,,/Resources/Tiles/forest_64/" + radiobutton.Content.ToString() + ".png";
            Image image = new Image();
            BitmapImage imageBmp = new BitmapImage();
            imageBmp.BeginInit();
            imageBmp.UriSource = new Uri(imageURI);
            imageBmp.EndInit();
            image.Source = imageBmp;
            this.previewImage.Children.Clear();
            this.previewImage.Children.Add(image);
            this.controller.OnObjectSelected(radiobutton.Content.ToString());
        }

        

        public void UpdateMapCanvas(Map map, Map objectMap)
        {
            ObjectCanvas.Children.Clear();
            ObjectCanvas.Width = map.Width *  mainWindow.TileSizeInPixels;
            ObjectCanvas.Height = map.Height * mainWindow.TileSizeInPixels;
            canvasImages = new Image[map.Width, map.Height];
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    var mapTile = map[x, y];
                    var image = new Image
                    {
                        Width = mainWindow.TileSizeInPixels,
                        Height = mainWindow.TileSizeInPixels,
                        Source = this.controller.GetTileImage(mapTile.Type),
                        Tag = new Vector2l(x, y)
                    };
                    ObjectCanvas.Children.Add(image);
                    Canvas.SetZIndex((UIElement)image, 1);
                    canvasImages[x, y] = image;
                    Canvas.SetLeft(image, x * mainWindow.TileSizeInPixels);
                    Canvas.SetTop(image, y * mainWindow.TileSizeInPixels);
                }
            }
            canvasObjectImages = new Image[map.Width, map.Height];
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    var objectTile = objectMap[x, y];
                    var image = new Image
                    {
                        Width = mainWindow.TileSizeInPixels,
                        Height = mainWindow.TileSizeInPixels,
                        Source = this.controller.GetObjectImage(objectTile.Type),
                        Tag = new Vector2l(x, y)
                    };
                    ObjectCanvas.Children.Add(image);
                    Canvas.SetZIndex((UIElement)image, 2);
                    canvasObjectImages[x, y] = image;
                    Canvas.SetLeft(image, x * mainWindow.TileSizeInPixels);
                    Canvas.SetTop(image, y * mainWindow.TileSizeInPixels);
                    image.MouseLeftButtonDown += OnTileClicked;
                }
            }

        }

        private void OnTileClicked(object sender, MouseButtonEventArgs e)
        {
            
            Image tile = (Image)sender;
            Vector2l position = (Vector2l)tile.Tag;
            if (idEditing)
            {
                idTextBox.Visibility = Visibility.Visible;
                ConnectionSelection.Visibility = Visibility.Visible;
                selectedTile = position;
                idTextBox.Text = Convert.ToString(this.controller.getObjectTileId(position.X, position.Y));
                foreach (var radio in ConnectionSelection.Children)
                {
                    RadioButton radiobutton = (RadioButton)radio;
                    if (radiobutton.Content.ToString()== this.controller.getObjectTileConnection(position.X, position.Y))
                    {
                        radiobutton.IsChecked = true;
                    }
                    else
                    {
                        radiobutton.IsChecked = false;
                    }
                }
                return;
            }
            this.controller.OnObjectClicked(position);
        }

        public void UpdateMapCanvas(Vector2l position, BitmapImage bitmapImage)
        {
            
            this.canvasObjectImages[position.X, position.Y].Source = bitmapImage;
            
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
        }

        private void Window_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                mainWindow.TileSizeInPixels += 8;
            }
            else
            {
                if (mainWindow.TileSizeInPixels == 8)
                {
                    return;
                }
                mainWindow.TileSizeInPixels -= 8;
            }
            controller.UpdateObjectCanvas();
        }

        private void ObjectLayerButton_Click(object sender, RoutedEventArgs e)
        {
            idEditing = !idEditing;
            if (!idEditing)
            {
                idTextBox.Visibility = Visibility.Hidden;
                ConnectionSelection.Visibility = Visibility.Hidden;
            }
        }

        private void IdChange_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void idTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                controller.SetObjectTileId(selectedTile.X, selectedTile.Y, Convert.ToInt16(idTextBox.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Only numbers are valid /n" + ex.ToString());
                
            }
            
        }

        private void objectTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Only text is valid /n" + ex.ToString());

            }
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
