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
        public Int2 Position { get; protected set; }

        /// <summary>
        /// Size of the container.
        /// </summary>
        public Int2 Size { get; protected set; }

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
                    DrawWindow();
                }

            }
        }
        public bool IsVIsable { get; protected set; }
        protected string emptyLine;
        protected Misc.Boxes.BoxType boxType;
        #endregion


        public Container(Int2 position, Int2 size, string name, Misc.Boxes.BoxType boxType)
        {
            Position = position;
            Size = size;
            Name = name;
            emptyLine = Stringer.GetFilledString(size.X - 2, ' ');
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
        public void Erase()
        {
            IsVIsable = false;
            string fullEmptyLine = emptyLine + "  ";

            for (int y = 0; y < Size.Y; y++)
            {
                Rendering.Renderer.Write(fullEmptyLine, Position.X, Position.Y + y);
            }
        }

        protected virtual void DrawWindow()
        {
            IsVIsable = true;
            Console.ForegroundColor = ConsoleColor.White;
            if (boxType != Misc.Boxes.BoxType.none)
                Misc.Boxes.DrawBox(boxType, Position.X, Position.Y, Size.X, Size.Y);
            //Rendering.Renderer.Write(Name, Position.X + (Size.X - Name.Length) / 2, Position.Y);

        }
        protected abstract void DrawContent();

    }
}
