using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace LevelEditor.Control
{
    using LevelEditor.View;
    using LevelEditor.Model;
    public partial class App : Application
    {
        private const string MapFileExtension = ".map";
        private const string MapFileFilter = "Map files (.map)|*map";
        private const string XMLElementHeight = "Height";
        private const string XMLElementWidth = "Width";
        private const string XMLElementPosX = "X";
        private const string XMLElementPosY = "y";
        private const string XMLElementTiles = "Tiles";
        private const string XMLElementObjects = "Objects";
        private const string XMLElementType = "Type";
        public AboutWindow aboutWindow;
        public MainWindow mainWindow;
        public NewMapWindow newMapWindow;
        public ObjectWindow objectWindow;
        public BrushWindow brushWindow;
        private MapTileType currentDrawTile;
        private MapTileType currentObjectType;
        private Map map;
        private Map objectMap;
        private Dictionary<string, BitmapImage> tileImages;
        private Dictionary<string, MapTileType> tileTypes;
        private Dictionary<string, MapTileType> objectTypes;
        private Dictionary<string, BitmapImage> objectImages;

        internal Dictionary<string, MapTileType> TileTypes { get => tileTypes; set => tileTypes = value; }
        internal Dictionary<string, MapTileType> ObjectTypes { get => objectTypes; set => objectTypes = value; }

        private void OnActivated(object sender,EventArgs e)
        {
            this.mainWindow = (MainWindow)this.MainWindow;
            this.mainWindow.SetMapTileTypes(this.TileTypes.Keys);
        }
        private void OnStartup(object sender,StartupEventArgs e)
        {
            //MAPTILES

            this.TileTypes = new Dictionary<string, MapTileType>();

            var empty = new MapTileType("empty",MapTileType.pack.CSTM);
            this.TileTypes.Add(empty.Name, empty);

            var P1 = new MapTileType("P1", MapTileType.pack.CSTM);
            this.TileTypes.Add(P1.Name, P1);

            var P2 = new MapTileType("P2", MapTileType.pack.CSTM);
            this.TileTypes.Add(P2.Name, P2);

            var Enemy = new MapTileType("Enemy", MapTileType.pack.CSTM);
            this.TileTypes.Add(Enemy.Name, Enemy);
            

            for (int i = 0; i < 255; i++) //adding forest
            {
                var tile = new MapTileType(i.ToString(), MapTileType.pack.FRST);
                this.TileTypes.Add(tile.Name, tile);
            }

            this.tileImages = new Dictionary<string, BitmapImage>();
            foreach (var tileType in this.TileTypes.Values)
            {
                string packfolder = "";
                switch ((MapTileType.pack)tileType.Assetpack)
                {
                    case MapTileType.pack.FRST:
                        packfolder = "forest_64/";
                        break;
                    case MapTileType.pack.CSTM:
                        packfolder = "custom/";
                        break;
                    case MapTileType.pack.VILG:
                        packfolder = "";
                        break;
                    case MapTileType.pack.DNGN:
                        packfolder = "";
                        break;
                }
                var imageURI = "pack://application:,,,/Resources/Tiles/" + packfolder + tileType.Name + ".png";
                BitmapImage tileImage = new BitmapImage();
                tileImage.BeginInit();
                tileImage.UriSource = new Uri(imageURI);
                tileImage.EndInit();
                this.tileImages.Add(tileType.Name, tileImage);
            }

            //OBJECT LAYER TILES (Might be removed later, since mostly same tiles are used anyway

            this.ObjectTypes = new Dictionary<string, MapTileType>();

            this.objectTypes.Add(Enemy.Name, Enemy);
            this.objectTypes.Add(P2.Name, P2);
            this.objectTypes.Add(P1.Name, P1);
            this.objectTypes.Add(empty.Name, empty);

            for (int i = 0; i < 255; i++)
            {
                var tile = new MapTileType(i.ToString(), MapTileType.pack.FRST);
                this.objectTypes.Add(tile.Name, tile);
            }

            this.objectImages = new Dictionary<string, BitmapImage>();
            foreach (var objectType in this.ObjectTypes.Values)
            {
                string packfolder = "";
                switch ((MapTileType.pack)objectType.Assetpack)
                {
                    case MapTileType.pack.FRST:
                        packfolder = "forest_64/";
                        break;
                    case MapTileType.pack.CSTM:
                        packfolder = "custom/";
                        break;
                    case MapTileType.pack.VILG:
                        packfolder = "";
                        break;
                    case MapTileType.pack.DNGN:
                        packfolder = "";
                        break;
                }
                var imageURI = "pack://application:,,,/Resources/Tiles/" + packfolder + objectType.Name + ".png";
                BitmapImage tileImage = new BitmapImage();
                tileImage.BeginInit();
                tileImage.UriSource = new Uri(imageURI);
                tileImage.EndInit();
                this.objectImages.Add(objectType.Name, tileImage);
            }
            
        }
        #region COMMANDS
        public bool CanExecuteClose()
        {
            return true;
        }
        public bool ExecuteHelp()
        {
            return true;
        }
        public bool ExecuteNew()
        {
            return true;
        }
        public bool ExecuteOpen()
        {
            return true;
        }
        public bool CanExecuteSaveAs()
        {
            return this.map != null;
        }
        public void CreateNew()
        {
            if (this.newMapWindow ==null|| !this.newMapWindow.IsLoaded)
            {
                this.newMapWindow = new NewMapWindow();
                this.newMapWindow.SetMapTileTypes(this.TileTypes.Keys);
            }
            this.newMapWindow.Show();
            this.newMapWindow.Focus();
        }

        

        public void SaveAs()
        {
            SaveFileDialog saveDialog = new SaveFileDialog
            {
                AddExtension = true,
                CheckPathExists = true,
                DefaultExt = MapFileExtension,
                FileName = "map",
                Filter = MapFileFilter,
                ValidateNames = true
            };
            var result = saveDialog.ShowDialog();
            if (result == false) return;

            //FileStream fs = new FileStream(saveDialog.FileName, FileMode.Create);  Idea of Serialization had to be abandoneddue to not being able to deserialize in Unity 
            //IFormatter formatter = new BinaryFormatter();
            //formatter.Serialize(fs, map);
            //formatter.Serialize(fs, objectMap);
            //fs.Close();

            
            using (Stream stream = saveDialog.OpenFile())
            {
                XmlWriterSettings settings = new XmlWriterSettings { Indent = true };
                using (var writer = XmlWriter.Create(stream, settings))
                {

                    writer.WriteStartElement("map");
                    writer.WriteAttributeString("xmlns", "map", null, "http://www.sae.edu");
                    writer.WriteElementString(XMLElementWidth, this.map.Width.ToString());
                    writer.WriteElementString(XMLElementHeight, this.map.Height.ToString());
                    writer.WriteStartElement(XMLElementTiles);
                    foreach (var mapTile in this.map.Tiles)
                    {
                        writer.WriteStartElement("MapTile");
                        writer.WriteElementString(XMLElementPosX, mapTile.Position.X.ToString());
                        writer.WriteElementString(XMLElementPosY, mapTile.Position.Y.ToString());
                        writer.WriteElementString(XMLElementType, mapTile.Assetpack + "_" + mapTile.Type);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    writer.WriteStartElement(XMLElementObjects);
                    foreach (var objectTile in this.objectMap.Tiles)
                    {
                        writer.WriteStartElement("ObjectTile");
                        writer.WriteElementString(XMLElementPosX, objectTile.Position.X.ToString());
                        writer.WriteElementString(XMLElementPosY, objectTile.Position.Y.ToString());
                        writer.WriteElementString(XMLElementType, objectTile.Assetpack + "_" + objectTile.Type);

                        writer.WriteElementString("Id",  "" + objectTile.Id);
                        writer.WriteElementString("Connection", objectTile.Connection);


                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
            }
        }
        public void Open()
        {
            OpenFileDialog openDialog = new OpenFileDialog
            {
                AddExtension = true,
                CheckPathExists = true,
                DefaultExt = MapFileExtension,
                FileName = "lil",
                Filter = MapFileFilter,
                ValidateNames = true
            };
            var result = openDialog.ShowDialog();
            if (result == false) return;

            //FileStream fs = new FileStream(openDialog.FileName, FileMode.Open);
            //BinaryFormatter formatter = new BinaryFormatter();
            //this.map = (Map)formatter.Deserialize(fs);
            //this.objectMap = (Map)formatter.Deserialize(fs);
            //this.UpdateMapCanvas();
            //fs.Close();

            using (Stream stream = openDialog.OpenFile())
            {
                using (var reader = XmlReader.Create(stream))
                {
                    try
                    {

                        reader.Read();
                        reader.ReadToFollowing(XMLElementWidth);
                        reader.ReadStartElement();
                        var width = reader.ReadContentAsInt();
                        reader.ReadToFollowing(XMLElementHeight);
                        reader.ReadStartElement();
                        var height = reader.ReadContentAsInt();
                        var loadedMap = new Map(width, height);
                        reader.ReadToFollowing(XMLElementTiles);
                        for (int i = 0; i < width * height; i++)
                        {
                            reader.ReadToFollowing(XMLElementPosX);
                            reader.ReadStartElement();
                            var x = reader.ReadContentAsInt();
                            reader.ReadToFollowing(XMLElementPosY);
                            reader.ReadStartElement();
                            var y = reader.ReadContentAsInt();
                            reader.ReadToFollowing(XMLElementType);
                            reader.ReadStartElement();
                            string tmp = reader.ReadContentAsString();
                            var pack = tmp.Substring(0, 4);
                            var type = tmp.Substring(5, tmp.Length - 5);
                            var mapTile = new MapTile(x, y, type,pack);
                            loadedMap[x, y] = mapTile;
                        }
                        var loadedObjectMap = new Map(width, height);
                        reader.ReadToFollowing(XMLElementObjects);
                        for (int i = 0; i < width * height; i++)
                        {
                            reader.ReadToFollowing(XMLElementPosX);
                            reader.ReadStartElement();
                            var x = reader.ReadContentAsInt();
                            reader.ReadToFollowing(XMLElementPosY);
                            reader.ReadStartElement();
                            var y = reader.ReadContentAsInt();
                            reader.ReadToFollowing(XMLElementType);
                            reader.ReadStartElement();
                            string tmp = reader.ReadContentAsString();
                            var pack = tmp.Substring(0, 4);
                            var type = tmp.Substring(5, tmp.Length - 5);
                            reader.ReadToFollowing("Id");
                            reader.ReadStartElement();
                            var id = reader.ReadContentAsInt();
                            reader.ReadToFollowing("Connection");
                            reader.ReadStartElement();
                            var connection = reader.ReadContentAsString();

                            var objectTile = new MapTile(x, y, type, pack, id, connection);
                            loadedObjectMap[x, y] = objectTile;
                        }
                        this.map = loadedMap;
                        this.objectMap = loadedObjectMap;
                        this.UpdateMapCanvas();
                    }
                    catch (XmlException)
                    {
                        MessageBox.Show("XML_EXCEPTION_ERROR");
                    }
                    catch (InvalidCastException)
                    {
                        MessageBox.Show("INVALID_CAST EXCEPTION");
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        MessageBox.Show("ARG_OUTOFRANGE EXCEPTION");
                    }

                }
            }
        }
        private void UpdateRecentFiles()
        {
            this.mainWindow.UpdateRecentFiles();
        }
        public void ObjectEditing()
        {
            if (map == null)
            {
                MessageBox.Show("No object layer available. Open or create a map first.");
                return;
            }
            if (this.objectWindow == null || !this.objectWindow.IsLoaded)
            {
                this.objectWindow = new ObjectWindow();
                this.objectWindow.SetObjectTypes(ObjectTypes.Keys);
                this.objectWindow.UpdateMapCanvas(map, objectMap);
            }
            this.objectWindow.Show();
            this.objectWindow.Focus();
            if (this.brushWindow != null)
                this.brushWindow.Close();
            CreateBrushWindow();
        }

        public void CreateBrushWindow()
        {
            this.brushWindow = new BrushWindow();
            this.brushWindow.InitializeBrushes(this.objectTypes.Keys);
            this.brushWindow.Show();
        }

        public void Quit()
        {
            Shutdown();
        }
        public void About()
        {
            aboutWindow = new AboutWindow();
            aboutWindow.Show();
        }
        #endregion
        public BitmapImage GetTileImage(string tileType)
        {
            return this.tileImages[tileType];
        }
        public BitmapImage GetObjectImage(string tileType)
        {
            return this.objectImages[tileType];
        }
        public void OnBrushSelected(string brush)
        {
            this.currentDrawTile = this.TileTypes[brush];
        }
        public void OnObjectSelected(string brush)
        {
            this.currentObjectType = this.ObjectTypes[brush];
        }
        public void OnTileClicked(Vector2l position)
        {
            if (this.currentDrawTile == null) return;
            this.map[position.X,position.Y].Type = this.currentDrawTile.Name;
            this.map[position.X, position.Y].Assetpack = (MapTile.pack)this.currentDrawTile.Assetpack;
            this.mainWindow.UpdateMapCanvas(position, tileImages[this.currentDrawTile.Name]);
        }
        public void OnObjectClicked(Vector2l position)
        {
            if (this.currentObjectType == null) return;
            this.objectMap[position.X, position.Y].Type = this.currentObjectType.Name;
            this.objectMap[position.X, position.Y].Assetpack = (MapTile.pack)this.currentDrawTile.Assetpack;
            this.objectWindow.UpdateMapCanvas(position, objectImages[this.currentObjectType.Name]);
        }
        public void CreateMap()
        {
            if (map != null)
            {
                var result = MessageBox.Show("Do you want to save your map? Unsaved progress will be lost.", "Warning", MessageBoxButton.YesNoCancel,MessageBoxImage.Warning);
                if (result == MessageBoxResult.No)
                {

                }
                if (result==MessageBoxResult.Yes)
                {
                    this.SaveAs();
                }
                if (result==MessageBoxResult.Cancel)
                {
                    return;
                }
            }
            
            int width;
            int height;
            try
            {
                width = int.Parse(this.newMapWindow.TextBoxMapWidth.Text);
                height = int.Parse(this.newMapWindow.TextBoxMapHeight.Text);
            }
            catch(FormatException)
            {
                MessageBox.Show("Incorrect Width or Height");
                return;
            }
            try
            {
                this.map = new Map(width, height);
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Incorrect map size");
                return;
            }
            var defaultMapTile = this.newMapWindow.SelectedMapTileType;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    this.map[x, y] = new MapTile(x, y, defaultMapTile,MapTile.pack.FRST);
                }
            }
            this.objectMap = new Map(width, height); //no need for try-catch, values already tested with other map
            string empty = "empty";
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    this.objectMap[x, y] = new MapTile(x, y, empty,MapTile.pack.CSTM);
                }
            }
            this.UpdateMapCanvas();
            this.mainWindow.SetMapTileTypes(TileTypes.Keys);
            this.newMapWindow.Close();
            if (this.brushWindow!=null)
            this.brushWindow.Close();
                this.brushWindow = new BrushWindow();
                this.brushWindow.InitializeBrushes(this.TileTypes.Keys);
                this.brushWindow.Show();

        }
        public void UpdateMapCanvas()
        {
            this.mainWindow.UpdateMapCanvas(this.map);
        }

        public void UpdateObjectCanvas()
        {
            this.objectWindow.UpdateMapCanvas(map, objectMap);
        }

        public int getObjectTileId(int x,int y)
        {
            return this.objectMap[x, y].Id;
        }

        public void SetObjectTileId(int x, int y, int id)
        {
            this.objectMap[x, y].Id = id;
        }

        public string getObjectTileConnection(int x, int y)
        {
            return this.objectMap[x, y].Connection;
        }

        public void SetObjectTileConnection(int x, int y, string text)
        {
            this.objectMap[x, y].Connection = text;
        }
    }
}