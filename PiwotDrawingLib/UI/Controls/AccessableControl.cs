using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiwotDrawingLib.UI.Controls
{
    public abstract class AccessableControl : MenuControl
    {
        public AccessableControl(string name, string identificator) : base(name, identificator)
        {
            accessable = true;
        }

        /// <summary>
        /// Tells if this control can be highlighted.
        /// </summary>
        protected bool accessable;
        public bool Accessable
        {
            get
            {
                return accessable;
            }
            set
            {
                if (value == accessable)
                    return;

                if(visable)
                {
                    NeedsRedraw = true;
                }
                accessable = value;
            }
        }
        protected bool selected;
        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                if (value == selected || !Accessable)
                    return;

                if (visable)
                {
                    selected = true;
                }
                selected = value;
            }
        }
        public void Select()
        {
            Selected = true;
        }
        public void Unselect()
        {
            Selected = false;
        }
    }
}
