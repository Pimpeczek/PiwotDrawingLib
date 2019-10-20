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
    class Menu : Container
    {
        #region Variables
        
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
       
        protected bool waitForInput;

        /// <summary>
        /// True if the container is waiting for input.
        /// </summary>
        public bool WaitingForInput{ get { return waitForInput; } protected set { } }

        

        List<Controls.MenuControl> controls;

        int hPoint;
        int scrollPoint;

        

        List<(ConsoleKey, Func<Events.MenuEvent, bool>)> bindings;

        
        #endregion

        #region Setup

        public Menu():base(new Int2(), new Int2(10, 10), "Menu", Misc.Boxes.BoxType.doubled)
        {
            IsVIsable = false;
            Setup();
            IsVIsable = true;
        }

        public Menu(Int2 position, Int2 size, string name, Misc.Boxes.BoxType boxType):base(position, size, name, boxType)
        {
            Setup();
        }

        void Setup()
        {
            waitForInput = false;
            controls = new List<Controls.MenuControl>();
            bindings = new List<(ConsoleKey, Func<Events.MenuEvent, bool>)>();
            hPoint = 0;
            scrollPoint = 0;
           
        }

        #endregion


        /// <summary>
        /// Adds new control to the list of controls.
        /// </summary>
        /// <param name="control">The control to be added.</param>
        /// <param name="action">The action to be performed if ENTER was pressed when control was selected.</param>
        /// <returns></returns>
        public Controls.MenuControl AddControl(Controls.MenuControl control)
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

            if (verticalTextWrapping == Wrapping.wrapping)
            {
                Size = new Int2(Size.X, controls.Count + 2);
            }
            return control;
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


        /// <summary>
        /// Waits for inputs from keyboard and acts accordingly to defined actions.
        /// </summary>
        public void WaitForInput()
        {
            waitForInput = true;
            ConsoleKey response;
            DrawWindow();
            hPoint = -1;
            LoopToAccesable(1);
            DrawContent();

            
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
                            if (controls[hPoint] is Controls.Switchable)
                                ((Controls.Switchable)controls[hPoint]).SwitchLeft();
                        }
                        else
                            LoopToAccesable(-1);
                        break;
                    case ConsoleKey.RightArrow:
                        if (hPoint < controls.Count)
                        {
                            if(controls[hPoint] is Controls.Switchable)
                                ((Controls.Switchable)controls[hPoint]).SwitchRight();
                        }
                        else
                            LoopToAccesable(1);
                        break;
                    case ConsoleKey.Enter:
                        if (hPoint < controls.Count)
                        {
                            if (controls[hPoint] is Controls.Pressable)
                                ((Controls.Pressable)controls[hPoint]).Press();
                        }
                        else
                            LoopToAccesable(1);
                        break;
                }



                DrawContent();

            } while (waitForInput);

            Erase();
        }

        /// <summary>
        /// Moves highlight to the next or previous selectable control. 
        /// </summary>
        /// <param name="direction"></param>
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

        /// <summary>
        /// Performs actions assigned to pressed button.
        /// </summary>
        /// <param name="key"></param>
        void RunBindings(ConsoleKey key)
        {
            for (int i = 0; i < bindings.Count; i++)
            {
                if(key == bindings[i].Item1)
                    bindings[i].Item2.Invoke(new Events.MenuBindingEvent(this, key));
            }
        }


      
        /// <summary>
        /// Draws menu window.
        /// </summary>
        override protected void DrawWindow()
        {
            IsVIsable = true;
            Console.ForegroundColor = ConsoleColor.White;
            if (boxType != Misc.Boxes.BoxType.none)
                Misc.Boxes.DrawBox(boxType, Position.X, Position.Y, Size.X, Size.Y);
            Rendering.Renderer.Write(Name, Position.X + (Size.X - Name.Length) / 2, Position.Y);

        }

        

        /// <summary>
        /// Draws only controls.
        /// </summary>
        override protected void DrawContent()
        {
            int borderWidth = (boxType == Misc.Boxes.BoxType.none ? 0 : 1);
            int startHeight = Arit.Clamp((Size.Y - 2 - controls.Count) / 2, 1, Size.Y - borderWidth * 2);
            string printText;
            Int2 pos;
            for (int i = 0; i < controls.Count && i < Size.Y -  borderWidth; i++)
            {
                if (controls[i + scrollPoint].visable)
                {
                    printText = controls[i + scrollPoint].PrintableText;
                    pos = new Int2(Position.X + 1, Position.Y + startHeight + i + 1);
                    Rendering.Renderer.Write(emptyLine, pos);
                    pos = new Int2(Position.X + (Size.X - printText.Length) / 2, Position.Y + startHeight + i + 1);
                    if (hPoint == i)
                    {
                        Rendering.Renderer.Write(printText.PastelBg(Color.Gray), pos);
                    }
                    else
                    {
                        Rendering.Renderer.Write(printText, pos);
                    }
                }
            }

        }

        /// <summary>
        /// Returns control with the given identificator. If nothing was found returns null.
        /// </summary>
        /// <param name="identificator">Identificator of a control to be found.</param>
        /// <returns></returns>
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
