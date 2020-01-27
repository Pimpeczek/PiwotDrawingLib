using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PiwotToolsLib.PMath;

namespace PiwotDrawingLib.UI.Controls
{
    abstract class Scroll: ContainerControl
    {
        protected static readonly string verticalStreachScrollCharacters = "▄█▀";
        protected static readonly string horizontalStreachScrollCharacters = "▐█▌";
        protected static readonly string verticaFixedScrollCharacters = "▴●▾";
        protected static readonly string horizontalFixedScrollCharacters = "◂●▸";
        
        public string scrollCharacters;

        public enum ScrollIconType { Streaching, Fixed }

        protected Misc.Boxes.BoxType boxType;
        public Misc.Boxes.BoxType BoxType
        {
            get
            {
                return boxType;
            }
            set
            {
                boxType = value;
            }
        }

        protected int size;
        public int Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
            }
        }

        public Scroll(string name) : base(name)
        {
            Size = 10;
        }

        public Scroll(string name, Containers.Container parent, Int2 position, int size) : base(name, parent, position)
        {
            Size = size;
            BoxType = parent.BoxType;
        }


    }
}
