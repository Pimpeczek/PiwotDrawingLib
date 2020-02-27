using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PiwotToolsLib.PMath;

namespace PiwotDrawingLib.UI.Controls
{
    public abstract class ContainerControl
    {
        protected Int2 position;
        public Int2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value ?? throw new ArgumentNullException();
            }
        }

        protected Containers.Container parent;

        public Containers.Container Parent
        {
            get
            {
                return parent;
            }
            protected set
            {
                parent = value;
            }
        }

        protected int Layer { get; set; }

        protected string name;

        public string Name
        {
            get
            {
                return name;
            }
            protected set
            {
                name = value;
            }
        }

        public ContainerControl(string name, Containers.Container parent, Int2 position)
        {
            Name = name;
            Position = position;
            Parent = parent;
        }

        public ContainerControl(string name)
        {
            Name = name;
            Position = Int2.Zero;
        }

        public abstract void Draw(Drawing.Canvas canvas);

    }
}
