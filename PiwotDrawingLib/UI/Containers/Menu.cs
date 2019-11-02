using System;
using System.Collections.Generic;
using System.Drawing;
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
                Hide();
                if (verticalTextWrapping == Wrapping.wrapping)
                {
                    size = new Int2(size.X, controls.Count + 2);
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
            Setup();
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
                size = new Int2(size.X, controls.Count + 2);
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
                            if (controls[hPoint] is Controls.ISwitchable)
                                ((Controls.ISwitchable)controls[hPoint]).SwitchLeft();
                        }
                        else
                            LoopToAccesable(-1);
                        break;
                    case ConsoleKey.RightArrow:
                        if (hPoint < controls.Count)
                        {
                            if(controls[hPoint] is Controls.ISwitchable)
                                ((Controls.ISwitchable)controls[hPoint]).SwitchRight();
                        }
                        else
                            LoopToAccesable(1);
                        break;
                    case ConsoleKey.Enter:
                        if (hPoint < controls.Count)
                        {
                            if (controls[hPoint] is Controls.IPressable)
                                ((Controls.IPressable)controls[hPoint]).Press();
                        }
                        else
                            LoopToAccesable(1);
                        break;
                }



                DrawContent();

            } while (waitForInput);

            Hide();
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
            } while (CheckControlAccessability(controls[hPoint]) && overflowCounter < controls.Count);
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
            base.DrawWindow();
            Drawing.Renderer.Draw(Name, position.X + (size.X - Name.Length) / 2, position.Y);

        }

        

        /// <summary>
        /// Draws only controls.
        /// </summary>
        override protected void DrawContent()
        {
            int borderWidth = (boxType == Misc.Boxes.BoxType.none ? 0 : 1);
            int startHeight = Arit.Clamp((size.Y - 2 - controls.Count) / 2, 1, size.Y - borderWidth * 2);
            string printText;
            Controls.MenuControl tControl;
            Int2 pos;
            for (int i = 0; i < controls.Count && i < size.Y -  borderWidth; i++)
            {
                tControl = controls[i + scrollPoint];
                if (tControl.visable && tControl.NeedsRedraw)
                {
                    printText = controls[i + scrollPoint].PrintableText;
                    pos = new Int2(position.X + 1, position.Y + startHeight + i + 1);
                    Drawing.Renderer.Draw(emptyLine, pos.X, pos.Y);
                    pos = new Int2(position.X + (size.X - printText.Length) / 2, position.Y + startHeight + i + 1);
                    if (hPoint == i)
                    {
                        Drawing.Renderer.Draw($"<cb808080>{printText}</cb>", pos.X, pos.Y);
                    }
                    else
                    {
                        Drawing.Renderer.Draw(printText, pos.X, pos.Y);
                    }
                }
            }
            Drawing.Renderer.Print();
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

        protected bool CheckControlAccessability(Controls.MenuControl control)
        {
            return !(control is Controls.AccessableControl && ((Controls.AccessableControl)control).Accessable);
        }
    }
}
