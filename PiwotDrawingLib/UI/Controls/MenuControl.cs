using System;
using System.Collections.Generic;

namespace PiwotDrawingLib.UI.Controls
{

    /// <summary>
    /// Represents a basic menu control used to display one line of text.
    /// </summary>
    abstract class MenuControl
    {
        #region Variables
        protected string name;

        /// <summary>
        /// Control name as well as its display text.
        /// </summary>
        public virtual string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                NeedsRedraw = true;
                PrintableText = $"{name}";
            }
        }
        public string Identificator { get; protected set; }
        public string PrintableText { get; protected set; }

        /// <summary>
        /// Menu that this control belongs to.
        /// </summary>
        public Containers.Menu parentMenu { get; protected set; }

        /// <summary>
        /// Tells if this control is shown on the menu.
        /// </summary>
        public bool visable;

        /// <summary>
        /// Tells if this control must be redrawn.
        /// </summary>
        public bool NeedsRedraw { get; set; }
        #endregion

        public MenuControl(string name, string identificator)
        {
            NeedsRedraw = true;
            visable = true;
            Name = name;
            Identificator = identificator;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="menu">The new parent</param>
        public virtual void SetParent(Containers.Menu menu)
        {
            parentMenu = menu;
        }



    }
}
