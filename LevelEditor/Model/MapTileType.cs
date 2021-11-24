using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Model
{
    [Serializable]
    class MapTileType
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public enum pack
        {
            CSTM,
            FRST,
            VILG,
            DNGN
        }

        private pack assetPack;
        public pack Assetpack
        {
            get { return assetPack; }
            set { assetPack = value; }
        }

        public MapTileType(string name,pack selectedPack)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name", "No name defined");
            this.Name = name;
            this.Assetpack = selectedPack;
        }
    }
}
