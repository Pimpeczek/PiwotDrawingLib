using System;
using System.Collections.Generic;

namespace PiwotDrawingLib.UI.Controls
{
    class MenuControl
    {
        protected string name;
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
        public Containers.Menu parentMenu { get; protected set; }
        public List<Func<Events.MenuEvent, bool>> actions;
        public bool accessable;
        public bool visable;
        public bool NeedsRedraw { get; set; }
        public MenuControl(string name, string identificator)
        {
            NeedsRedraw = true;
            visable = true;
            accessable = true;
            actions = new List<Func<Events.MenuEvent, bool>>();
            Name = name;
            Identificator = identificator;

        }

        public virtual void AddAction(Func<Events.MenuEvent, bool> action)
        {
            actions.Add(action);
        }
        public virtual void AddAction(IEnumerable<Func<Events.MenuEvent, bool>> action)
        {
            actions.AddRange(action);
        }

        public virtual void SetParent(Containers.Menu menu)
        {
            parentMenu = menu;
        }

        public virtual void SwitchLeft()
        {
            NeedsRedraw = true;
            //RunActions();
        }
        public virtual void SwitchRight()
        {
            NeedsRedraw = true;
            //RunActions();
        }
        public virtual void Enter()
        {
            NeedsRedraw = true;
            RunActions();
        }

        protected virtual void RunActions()
        {
            for (int i = 0; i < actions.Count; i++)
            {
                actions[i].Invoke(new Events.MenuControllEvent(parentMenu, this));
            }
        }

        public virtual int GetValue()
        {
            return 0;
        }

    }
}
