using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PiwotToolsLib.PMath;

namespace PiwotDrawingLib.UI
{
    public abstract class UIElement
    {

        protected bool visable;

        public bool Visable
        {
            get
            {
                return visable;
            }
            set
            {
                if (value)
                {
                    visable = true;
                }
                else
                {
                    Erase();
                }
            }
        }

        protected Containers.Container parent;

        public Containers.Container Parent
        {
            get
            {
                return parent;
            }
            set
            {
                if(parent != null)
                {
                    parent.RemoveChild(this);
                    parent = value;
                }
                if (value != null)
                    value.AddChild(this);
            }
        }
        /// <summary>
        /// Position of the UI element.
        /// </summary>
        protected Int2 position;

        /// <summary>
        /// Position of the UI element.
        /// </summary>
        virtual public Int2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        /// <summary>
        /// Size of the UI element.
        /// </summary>
        protected Int2 size;

        /// <summary>
        /// Size of the UI element.
        /// </summary>
        virtual public Int2 Size
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

        protected UIElement()
        {
            size = Int2.One;
            position = Int2.Zero;
        }

        /// <summary>
        /// Prints this UI element on a given canvas.
        /// </summary>
        virtual public void PrintOnCanvas(Drawing.Canvas canvas)
        {
            visable = true;
        }

        virtual public void Erase()
        {
            if (!visable)
                return;
            visable = false;
            if (parent != null)
                parent.EraseChild(this);
        }


    }
}
