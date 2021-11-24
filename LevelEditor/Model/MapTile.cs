using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Model
{
    [Serializable]
    public class MapTile
    {
        public Vector2l Position { get; set; }
        public string Type { get; set; }

        private int id;
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private string connection;
        public string Connection
        {
            get { return connection; }
            set { connection = value; }
        }

        public enum pack
        {
            CSTM,
            FRST,
            VILG,
            DNGN
        }

        private pack assetPack;
        private int x;
        private int y;

        public pack Assetpack
        {
            get { return assetPack; }
            set { assetPack = value; }
        }

        public MapTile()
        {
        }

        public MapTile(int x, int y,string type,pack assetPack):this (new Vector2l(x,y),type,assetPack)
        {

        }

        public MapTile(Vector2l pos,string type, pack assetPack)
        {
            this.Position = pos;
            this.Type = type;
            this.Assetpack = assetPack;
        }

        public MapTile(int x, int y, string type, pack assetPack, int id, string connection):this (new Vector2l(x, y),type, assetPack, id, connection)
        {
            
        }

        public MapTile(Vector2l pos, string type, pack assetPack, int id, string connection)
        {
            this.Position = pos;
            this.Type = type;
            this.Id = id;
            this.Connection = connection;
            this.Assetpack = assetPack;
        }

        public MapTile(int x, int y, string type, string assetpack) : this(new Vector2l(x, y), type, assetpack)
        {

        }

        public MapTile(Vector2l pos, string type, string assetpack)
        {
            this.Position = pos;
            this.Type = type;
            this.Assetpack = (pack)Enum.Parse(typeof(pack), assetpack);
        }

        public MapTile(int x, int y, string type, string pack, int id, string connection) : this(x, y, type, pack)
        {
            this.id = id;
            this.connection = connection;
        }
    }
}
