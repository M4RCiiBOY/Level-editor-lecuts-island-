using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Model
{
    [Serializable]
    public struct Vector2l
    {
        private readonly int x;
        private readonly int y;

        public Vector2l(int x, int y)
        {
            this.y = y;
            this.x = x;
        }

        public int X { get { return x; } }
        public int Y { get { return y; } }

        public override string ToString()
        {
            return string.Format("X:{0}, Y:{1}", this.X, this.Y);
        }
    }
}
