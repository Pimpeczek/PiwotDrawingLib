using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PiwotToolsLib.PMath;
using PiwotToolsLib.Data;
using PiwotDrawingLib.Drawing;

namespace PiwotDrawingLib.UI.Containers
{
    public abstract class Container: UIElement
    {
        #region Variables

        public enum ContentHandling { ResizeContent, FitContent, CropContent}

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

                position = value ?? throw new ArgumentNullException();
                contentPosition = position + (boxType != Misc.Boxes.BoxType.none ? Int2.One : Int2.Zero);

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
                
                
                size = value ?? throw new ArgumentNullException();
                contentSize = size - (boxType != Misc.Boxes.BoxType.none ? Int2.One * 2 : Int2.Zero);
                canvas.ResizeCanvas(size);
                Erase();
                if (IsVIsable)
                    Draw();
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
                name = value;

                if (IsVIsable)
                {
                    Draw();
                }

            }
        }

        protected Int2 contentSize;
        protected Int2 contentPosition;


        public bool IsVIsable { get; protected set; }
        protected string emptyLine;
        protected string fullEmptyLine;
        protected Misc.Boxes.BoxType boxType = Misc.Boxes.BoxType.normal;
        public Misc.Boxes.BoxType BoxType
        {
            get
            {
                return boxType;
            }
            set
            {
                if (value == boxType)
                    return;

                boxType = value;
                boxCharacters = Misc.Boxes.GetBoxArray(boxType);
            }
        }
        protected char[] boxCharacters = Misc.Boxes.GetBoxArray(Misc.Boxes.BoxType.normal);

        protected Drawing.Canvas canvas;

        protected List<UIElement> children;


        #endregion



       

        public Container(Int2 position, Int2 size, string name, Misc.Boxes.BoxType boxType)
        {
            this.boxType = boxType;
            Position = position;
            canvas = new Drawing.Canvas(size);
            Size = size;
            Name = name;
            boxCharacters = Misc.Boxes.GetBoxArray(boxType);

        }

        /// <summary>
        /// Draws menu window and controls.
        /// </summary>
        public void Draw()
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
        public void Hide()
        {
            
            IsVIsable = false;
            Erase();
        }

        override public void Erase()
        {

        }

        protected virtual void DrawWindow()
        {
            
            if (boxType != Misc.Boxes.BoxType.none)
            {
                Misc.Boxes.DrawBox(canvas, boxType, position.X, position.Y, size.X, size.Y);
                canvas.DrawOnCanvas(Name, (size.X - Name.Length) / 2, 0);
            }
            //Rendering.Renderer.Write(Name, Position.X + (Size.X - Name.Length) / 2, Position.Y);

        }
        protected virtual void DrawContent()
        {
            PrintChildren();
        }

        protected virtual void PrintChildren()
        {
            for(int i = 0; i < children.Count; i++)
            {
                if(children[i].Visable)
                    children[i].PrintOnCanvas(canvas);
            }
        }

        public override void PrintOnCanvas(Canvas canvas)
        {
            DrawWindow();
            DrawContent();
            canvas.ApplyNewFrame();
            canvas.AddCanvas(this.canvas, position.X, position.Y);
        }

        public void AddChild(UIElement element)
        {
            if (children.Contains(element))
                return;
            children.Add(element);
            if (element.Parent != this)
                element.Parent = this;
        }

        public void RemoveChild(UIElement element)
        {
            if (!children.Contains(element))
                return;
            children.Remove(element);

            if (element.Parent == this)
                element.Parent = null;
        }

        public void EraseChild(UIElement element)
        {
            canvas.Clear(element.Position, element.Size);
        }

    }
}
