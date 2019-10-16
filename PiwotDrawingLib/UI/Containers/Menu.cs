using System;
using System.Collections.Generic;
using System.Drawing;
using Pastel;
using PiwotToolsLib.PMath;
using PiwotToolsLib.Data;

namespace PiwotDrawingLib.UI.Containers
{
    /// <summary>
    /// Container that can store MenuControls and read inputs.
    /// </summary>
    class Menu
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

        public enum Wrapping { scrolling, wrapping, none };
        protected Wrapping verticalTextWrapping;

        /// <summary>
        /// Tells how the container should manage too many controls.
        /// </summary>
        public Wrapping VerticalTextWrapping
        {
            get
            {
                return verticalTextWrapping;
            }
            set
            {
                if (verticalTextWrapping == value)
                    return;
                Erase();
                if (verticalTextWrapping == Wrapping.wrapping)
                {
                    Size = new Int2(Size.X, controls.Count + 2);
                }
                verticalTextWrapping = value;
                DrawWindow();
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

                if(IsVIsable)
                {
                    DrawWindow();
                }

            }
        }
        protected bool waitForInput;

        /// <summary>
        /// True if the container is waiting for input.
        /// </summary>
        public bool WaitingForInput{ get { return waitForInput; } protected set { } }

        Misc.Boxes.BoxType boxType;

        List<Controls.MenuControl> controls;

        int hPoint;
        int scrollPoint;

        string emptyLine;

        List<(ConsoleKey, Func<Events.MenuEvent, bool>)> bindings;

        public bool IsVIsable { get; protected set; }
        #endregion

        #region Setup

        public Menu()
        {
            IsVIsable = false;
            Setup(new Int2(), new Int2(10, 10), "Menu", Misc.Boxes.BoxType.doubled);
            IsVIsable = true;
        }

        public Menu(Int2 position, Int2 size, string name, Misc.Boxes.BoxType boxType)
        {
            Setup(position, size, name, boxType);
        }

        void Setup(Int2 position, Int2 size, string name, Misc.Boxes.BoxType boxType)
        {
            waitForInput = false;
            controls = new List<Controls.MenuControl>();
            bindings = new List<(ConsoleKey, Func<Events.MenuEvent, bool>)>();
            hPoint = 0;
            scrollPoint = 0;
            Position = position;
            Size = size;
            Name = name;
            emptyLine = Stringer.GetFilledString(size.X - 2, ' ');
            this.boxType = boxType;
        }

        #endregion


        /// <summary>
        /// Adds new control to the list of controls.
        /// </summary>
        /// <param name="control">The control to be added.</param>
        /// <param name="action">The action to be performed if ENTER was pressed when control was selected.</param>
        /// <returns></returns>
        public Controls.MenuControl AddControl(Controls.MenuControl control, Func<Events.MenuEvent, bool> action)
        {
            for (int i = 0; i < controls.Count; i++)
            {
                if (controls[i].Identificator == control.Identificator)
                {
                    controls[i] = control;
                    return control;
                }
            }
            control.SetParent(this);
            controls.Add(control);

            if(action != null)
            {
                control.AddAction(action);
            }

            if (verticalTextWrapping == Wrapping.wrapping)
            {
                Size = new Int2(Size.X, controls.Count + 2);
            }
            return control;
        }

        /// <summary>
        /// Adds new control to the list of controls.
        /// </summary>
        /// <param name="control">The control to be added.</param>
        public void AddControl(Controls.MenuControl control)
        {
            AddControl(control, null);
        }

        /// <summary>
        /// Returns value of a controll as a integer. If no control was found throws IdentificatorNotFoundException
        /// </summary>
        /// <param name="identificator"></param>
        /// <returns></returns>
        public int GetValue(string identificator)
        {
            for (int i = 0; i < controls.Count; i++)
            {
                if (controls[i].Identificator == identificator)
                {
                    return controls[i].GetValue();
                }
            }
            throw new Exceptions.IdentificatorNotFoundException();
        }

        /// <summary>
        /// Method used to exit WaitForInput loop.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool Exit(Events.MenuEvent e)
        {
            waitForInput = false;
            
            return true;
        }

        public void WaitForInput()
        {
            waitForInput = true;
            ConsoleKey response;
            DrawWindow();
            hPoint = -1;
            LoopToAccesable(1);
            DrawControls();

            
            do
            {
                response = Console.ReadKey(true).Key;
                RunBindings(response);
                switch (response)
                {
                    case ConsoleKey.UpArrow:
                        LoopToAccesable(-1);
                        break;
                    case ConsoleKey.DownArrow:
                        LoopToAccesable(1);
                        break;
                    case ConsoleKey.LeftArrow:
                        if (hPoint < controls.Count)
                        {
                            controls[hPoint].SwitchLeft();
                        }
                        else
                            LoopToAccesable(-1);
                        break;
                    case ConsoleKey.RightArrow:
                        if (hPoint < controls.Count)
                        {
                            controls[hPoint].SwitchRight();
                        }
                        else
                            LoopToAccesable(1);
                        break;
                    case ConsoleKey.Enter:
                        if (hPoint < controls.Count)
                        {
                            controls[hPoint].Enter();
                        }
                        else
                            LoopToAccesable(1);
                        break;
                }



                DrawControls();

            } while (waitForInput);

            Erase();
        }

        void LoopToAccesable(int direction)
        {
            direction = Arit.Clamp(direction, -1, 1);
            if(direction == 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            int overflowCounter = 0;
            do
            {
                hPoint += direction;
                if(hPoint < 0)
                {
                    hPoint = controls.Count - 1;
                }
                else if( hPoint >= controls.Count)
                {
                    hPoint = 0;
                }
                overflowCounter++;
            } while (!controls[hPoint].accessable && overflowCounter < controls.Count);
            if (overflowCounter >= controls.Count)
                hPoint = 0;
        }

        void RunBindings(ConsoleKey key)
        {
            for (int i = 0; i < bindings.Count; i++)
            {
                if(key == bindings[i].Item1)
                    bindings[i].Item2.Invoke(new Events.MenuBindingEvent(this, key));
            }
        }

        public void Draw()
        {
            DrawWindow();
            DrawControls();
        }

        public void DrawWindow()
        {
            IsVIsable = true;
            Console.ForegroundColor = ConsoleColor.White;
            Misc.Boxes.DrawBox(boxType, Position.X, Position.Y, Size.X, Size.Y);
            PiwotDrawingLib.Rendering.Renderer.Write(Name, Position.X + (Size.X - Name.Length) / 2, Position.Y);

        }
        public void Erase()
        {
            IsVIsable = false;
            string fullEmptyLine = emptyLine + "  ";

            for(int y = 0; y < Size.Y; y++)
            {
                PiwotDrawingLib.Rendering.Renderer.Write(fullEmptyLine, Position.X, Position.Y + y);
            }
            

        }


        void DrawControls()
        {
            int startHeight = Arit.Clamp((Size.Y - 2 - controls.Count) / 2, 0, Size.Y);
            string printText;
            Int2 pos;
            for (int i = 0; i < controls.Count && i < Size.Y - 2; i++)
            {
                if (controls[i + scrollPoint].visable)
                {
                    printText = controls[i + scrollPoint].PrintableText;
                    pos = new Int2(Position.X + 1, Position.Y + startHeight + i + 1);
                    PiwotDrawingLib.Rendering.Renderer.Write(emptyLine, pos);
                    pos = new Int2(Position.X + (Size.X - printText.Length) / 2, Position.Y + startHeight + i + 1);
                    if (hPoint == i)
                    {
                        PiwotDrawingLib.Rendering.Renderer.Write(printText.PastelBg(Color.Gray), pos);
                    }
                    else
                    {
                        PiwotDrawingLib.Rendering.Renderer.Write(printText, pos);
                    }
                }
            }

        }

        public Controls.MenuControl GetControll(string identificator)
        {
            for(int i = 0; i < controls.Count; i++)
            {
                if (identificator == controls[i].Identificator)
                    return controls[i];
            }
            return null;
        }
    }


    
}
