using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Model
{
    [Serializable]
    public class Map
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public MapTile[,] Tiles { get; private set; }
        public Map(int width, int height)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException("width", "Width must be greater than 0");
            if (height <= 0)
                throw new ArgumentOutOfRangeException("height", "height must be greater than 0");

            this.Width = width;
            this.Height = height;

            this.Tiles = new MapTile[width, height];
        }

        public MapTile this[int x,int y]
        {
            get { return this.Tiles[x, y]; }
            set { this.Tiles[x, y] = value; }
        }
    }
}
