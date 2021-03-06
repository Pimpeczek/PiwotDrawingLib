﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PiwotToolsLib.PMath;

namespace PiwotDrawingLib.UI
{
    /// <summary>
    /// The base class for all UI elements.
    /// </summary>
    public abstract class UIElement
    {
        protected bool isBeingDrawn;

        public bool IsBeingDrawn
        {
            get
            {
                return isBeingDrawn;
            }

            protected set
            {
                isBeingDrawn = value;
            }
        }

        protected bool contentRedrawNeeded;

        public bool ContentRedrawNeeded
        {
            get
            {
                return contentRedrawNeeded;
            }

            set
            {
                contentRedrawNeeded = value;
            }
        }

        /// <summary>
        /// Determines if a given element is visable.
        /// </summary>
        protected bool visable;

        /// <summary>
        /// Determines if a given element is visable.
        /// </summary>
        public bool Visable
        {
            get
            {
                return visable;
            }
            protected set
            {

                visable = value;
            }
        }

        /// <summary>
        /// The parent of this element.
        /// </summary>
        protected Containers.Container parent;

        /// <summary>
        /// The parent of this element.
        /// </summary>
        public Containers.Container Parent
        {
            get
            {
                return parent;
            }
            set
            {
                WaitForDrawingEnd();
                if (parent != null)
                {
                    parent.RemoveChild(this);
                }
                parent = value;
                if (parent != null)
                {
                    parent.AddChild(this);
                }
                
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
                WaitForDrawingEnd();
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
                WaitForDrawingEnd();
                size = value;
            }
        }

        protected UIElement()
        {
            size = Int2.One;
            position = Int2.Zero;
            visable = true;
        }

        /// <summary>
        /// Prints this UI element on a given canvas.
        /// </summary>
        abstract public void PrintOnCanvas(Drawing.Canvas canvas);

        /// <summary>
        /// Clears the element.
        /// </summary>
        virtual public void Clear()
        {
            if (parent != null)
            {
                parent.EraseChild(this);
            }
            
        }

        /// <summary>
        /// Erases the element from the screen.
        /// </summary>
        virtual public void Erase()
        {
            WaitForDrawingEnd();
            if (!visable)
                return;
            visable = false;
            if (parent != null)
                parent.EraseChild(this);
        }

        /// <summary>
        /// Stops the thread until drawing of this UIElement is finished.
        /// </summary>
        protected void WaitForDrawingEnd()
        {
            while (isBeingDrawn) { }
        }
    }
}
