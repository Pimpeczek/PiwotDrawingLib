using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PiwotToolsLib.PMath;
using PiwotToolsLib.Data;

namespace PiwotDrawingLib.UI.Containers
{
    abstract class Container
    {
        #region Variables
        /// <summary>
        /// Position of the container.
        /// </summary>
        protected Int2 position;
        public Int2 Position
        {
            get
            {
                return position;
            }
            set
            {
                if (value < Int2.Zero)
                    throw new Exceptions.InvalidContainerPositionException(this);

                if (IsVIsable)
                    Erase();
                position = value ?? throw new ArgumentNullException();
                contentPosition = position + (boxType != Misc.Boxes.BoxType.none ? Int2.One : Int2.Zero);
                if (IsVIsable)
                    Draw();

            }
        }

        /// <summary>
        /// Size of the container.
        /// </summary>
        protected Int2 size;
        public Int2 Size
        {
            get
            {
                return size;
            }
            set
            {
                if (value < Int2.One * 2)
                    throw new Exceptions.InvalidContainerSizeException(this);
                
                
                if (IsVIsable)
                {
                    Rendering.Renderer.Write("Erase", 0, 1);
                    Erase();
                }
                else
                {
                    Rendering.Renderer.Write("NotErase", 0, 1);
                }
                size = value ?? throw new ArgumentNullException();
                contentSize = size - (boxType != Misc.Boxes.BoxType.none ? Int2.One * 2 : Int2.Zero);
                Rendering.Renderer.Write($"Size {Size.ToString()} {DateTime.Now.Millisecond}  ", 0, 0);
                emptyLine = Stringer.GetFilledString(contentSize.X, ' ');
                fullEmptyLine = Stringer.GetFilledString(size.X, ' ');
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
        protected Misc.Boxes.BoxType boxType;
        #endregion


        public Container(Int2 position, Int2 size, string name, Misc.Boxes.BoxType boxType)
        {
            Position = position;
            Size = size;
            Name = name;
            
            this.boxType = boxType;
        }

        /// <summary>
        /// Draws menu window and controls.
        /// </summary>
        public void Draw()
        {
            DrawWindow();
            DrawContent();
        }

        /// <summary>
        /// Erases menu window and its content.
        /// </summary>
        public void Hide()
        {
            
            IsVIsable = false;
            Erase();
        }

        protected void Erase()
        {
            Rendering.Renderer.Write("EraseREAL" + DateTime.Now.Millisecond, 0, 2);
            for (int y = 0; y < size.Y; y++)
            {
                Rendering.Renderer.Write(fullEmptyLine, position.X, position.Y + y);
            }
        }

        protected virtual void DrawWindow()
        {
            IsVIsable = true;
            Console.ForegroundColor = ConsoleColor.White;
            if (boxType != Misc.Boxes.BoxType.none)
                Misc.Boxes.DrawBox(boxType, position.X, position.Y, size.X, size.Y);
            //Rendering.Renderer.Write(Name, Position.X + (Size.X - Name.Length) / 2, Position.Y);

        }
        protected abstract void DrawContent();

    }
}
