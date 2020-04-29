using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PiwotToolsLib.PMath;
using PiwotToolsLib.Data;
using PiwotDrawingLib.Drawing;

namespace PiwotDrawingLib.UI.Containers
{

    /// <summary>
    /// The basic UI container class.
    /// Can display <c>UIElement</c>s.
    /// </summary>
    public class Container: UIElement
    {
        #region Variables

        

        /// <summary>
        /// Position of the UI element.
        /// </summary>
        override public Int2 Position
        {
            get
            {
                return position;
            }
            set
            {
                if (value < Int2.Zero)
                    throw new Exceptions.InvalidContainerPositionException(this);

                WaitForDrawingEnd();

                position = value ?? throw new ArgumentNullException();
                contentLocalPosition = boxType != Misc.Boxes.BoxType.none ? Int2.One : Int2.Zero;
                contentPosition = position + contentLocalPosition;

            }
        }

        /// <summary>
        /// Size of the container.
        /// </summary>
        override public Int2 Size
        {
            get
            {
                return size;
            }
            set
            {
                if (value < Int2.One * 2)
                    throw new Exceptions.InvalidContainerSizeException(this);

                if (value == null)
                    throw new ArgumentNullException();
                WaitForDrawingEnd();
                if (IsVIsable)
                {
                    Erase();
                }
                size = value;
                contentSize = size - contentLocalPosition * 2;
                canvas.ResizeCanvas(size);

            }
        }

        protected string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                WaitForDrawingEnd();
                name = value;
            }
        }

        protected Int2 contentSize;
        public Int2 ContentSize
        {
            get
            {
                return new Int2(contentSize);
            }
        }
        protected Int2 contentPosition;
        protected Int2 contentLocalPosition;

        

        public bool IsVIsable { get; protected set; }
        protected string emptyLine;
        protected string fullEmptyLine;
        protected Misc.Boxes.BoxType boxType = Misc.Boxes.BoxType.normal;
        public virtual Misc.Boxes.BoxType BoxType
        {
            get
            {
                return boxType;
            }
            set
            {
                if (value == boxType)
                    return;

                WaitForDrawingEnd();

                boxType = value;
                boxCharacters = Misc.Boxes.GetBoxArray(boxType);
                contentLocalPosition = boxType == Misc.Boxes.BoxType.none ? Int2.Zero : Int2.One;
                Size = size;
            }
        }
        protected char[] boxCharacters = Misc.Boxes.GetBoxArray(Misc.Boxes.BoxType.normal);

        protected Drawing.Canvas canvas;

        protected List<UIElement> children;

        public List<UIElement> Children { get{ return children; } }
        public bool Registered { get; protected set; }

        #endregion




        /// <summary>
        /// Creates and instance of Container class.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="size"></param>
        /// <param name="name"></param>
        /// <param name="boxType"></param>
        public Container(Int2 position, Int2 size, string name, Misc.Boxes.BoxType boxType)
        {
            this.boxType = boxType;
            Position = position;
            canvas = new Drawing.Canvas(size);
            Size = size;
            Name = name;
            boxCharacters = Misc.Boxes.GetBoxArray(boxType);
            Registered = false;
            children = new List<UIElement>();
        }

        /// <summary>
        /// Draws menu window and controls.
        /// </summary>
        public virtual void Draw()
        {
            IsVIsable = true;
            DrawWindow();
            DrawContent();
            canvas.ApplyNewFrame();
            if (parent == null)
            {
                Drawing.Renderer.Draw(canvas, position.X, position.Y);
            }
        }

        public virtual void RefreshContent()
        {
            DrawContent();
            canvas.ApplyNewFrame();
            if (parent == null)
            {
                Drawing.Renderer.Draw(canvas, position.X, position.Y);
            }
        }

        /// <summary>
        /// Erases menu window and its content.
        /// </summary>
        override public void Erase()
        {
            if (!visable)
                return;
            WaitForDrawingEnd();
            //visable = false;
            if (parent != null)
            {
                parent.EraseChild(this);
            }
            else
            {
                Renderer.Draw(">=<", size.X + 10, 1);
                Renderer.RegisterForErase(this);
                //let the renderer know
            }

        }

        protected virtual void DrawWindow()
        {
            
            if (boxType != Misc.Boxes.BoxType.none)
            {
                Misc.Boxes.DrawBox(canvas, boxType, 0,0, size.X, size.Y, false);
                canvas.DrawOnCanvas(Name, (size.X - Name.Length) / 2, 0);
            }
            //Rendering.Renderer.Write(Name, Position.X + (Size.X - Name.Length) / 2, Position.Y);

        }


        protected virtual void DrawContent()
        {
            PrintChildren();
        }

        /// <summary>
        /// Prints children on this Container's canvas.
        /// </summary>
        protected virtual void PrintChildren()
        {
            for(int i = 0; i < children.Count; i++)
            {
                if(children[i].Visable)
                    children[i].PrintOnCanvas(canvas);
            }
        }

        /// <summary>
        /// Prints this container on a given canvas.
        /// </summary>
        /// <param name="canvas"></param>
        public override void PrintOnCanvas(Canvas canvas)
        {
            isBeingDrawn = true;
            DrawWindow();
            DrawContent();
            this.canvas.ApplyNewFrame();
            canvas.AddCanvas(this.canvas, position.X, position.Y);
            isBeingDrawn = false;
        }

        public virtual void AddChild(UIElement element)
        {
            if (children.Contains(element))
                return;
            WaitForDrawingEnd();
            children.Add(element);

            if (element.Parent != this)
                element.Parent = this;
        }

        public virtual void RemoveChild(UIElement element)
        {
            if (!children.Contains(element))
                return;
            WaitForDrawingEnd();
            children.Remove(element);

            if (element.Parent == this)
                element.Parent = null;
        }

        public void EraseChild(UIElement element)
        {
            WaitForDrawingEnd();
            canvas.Clear(element.Position, element.Size);
        }


        /// <summary>
        /// Registers this container in the <c>Renderer</c> async drawing registry.
        /// </summary>
        public void Register()
        {
            if (Registered)
                return;
            Registered = true;
            Renderer.RegisterContainer(this);
        }

        /// <summary>
        /// Unregisters this container from the <c>Renderer</c> async drawing registry.
        /// </summary>
        public void Unregister()
        {
            if (!Registered)
                return;
            Registered = false;
            Renderer.UnregisterContainer(this);
        }

    }
}
