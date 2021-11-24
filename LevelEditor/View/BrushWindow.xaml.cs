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
    /// <summary>
    /// Interaction logic for BrushWindow.xaml
    /// </summary>
    public partial class BrushWindow : Window
    {
        private readonly App controller;
        Image[] images;
        Image selection;

        public BrushWindow()
        {
            this.controller = (App)Application.Current;
            InitializeComponent();
        }

        public void InitializeBrushes(IEnumerable<string> mapTileTypes)
        {
            
            int count = 0;
            brushCanvas.Children.Clear();
            List<Image> tmpImageList = new List<Image>();
            foreach (var mapTileType in mapTileTypes)
            {
                var image = new Image
                {
                    Width = 64,
                    Height = 64,
                    Source = this.controller.GetTileImage(mapTileType),
                    Tag = mapTileType
                };
                tmpImageList.Add(image);
                //brushCanvas.Children.Add(image);
                //brushCanvas.Height = count * 70;
                //brushCanvas.Width = this.Width;
                //Canvas.SetTop(image, (count * 70)+3);
                //Canvas.SetLeft(image, 3);
                Canvas.SetZIndex(image, 1);
                image.MouseLeftButtonDown += OnTileClicked;
                count++;
            }
            images = tmpImageList.ToArray();

            BitmapImage imageBmp = new BitmapImage();
            imageBmp.BeginInit();
            imageBmp.UriSource = new Uri("pack://application:,,,/Resources/Icons/Selection.png");
            imageBmp.EndInit();
            selection = new Image()
            {
                Width = 64,
                Height = 64,
                Source = imageBmp,
                Opacity = 50
            };
            brushCanvas.Children.Add(selection);
            Canvas.SetTop(selection, -64);
            Canvas.SetZIndex(selection, 2);

            for (int i = 0; i < images.Length; i++)
            {
                wrap.Children.Add(images[i]);
            }
        }

        private void OnTileClicked(object sender, MouseButtonEventArgs e)
        {
            var tile = (Image)sender;
            string type = (string)tile.Tag;
            
            Canvas.SetTop(selection, Canvas.GetTop((Image)sender));
            Canvas.SetLeft(selection, Canvas.GetLeft((Image)sender));

            controller.OnBrushSelected(type);
            controller.OnObjectSelected(type);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //brushCanvas.Width = this.Width;
            //Canvas.SetTop(selection, -70);
            //int columns = (int)brushCanvas.Width / 70;
            //ChangeBrushColumns(columns, columns);
        }

        private void ChangeBrushColumns(int callback, int columns)
        {
            int number = ((columns - callback) * (images.Length / columns));

            for (int i = 0 + number; i < ((images.Length / columns) + number)+ images.Length % columns; i++)
            {
                int test = 70 * (i - ((columns - callback) * (images.Length / columns)));
                Canvas.SetTop(images[i], 70 * (i - number));
                Canvas.SetLeft(images[i], 3 + (70*(columns-callback)));
                brushCanvas.Height = 70 * (i - number + 1);
            }
            callback--;
            if (callback!=0)
            {
                ChangeBrushColumns(callback, columns);
            }
        }
    }
}
