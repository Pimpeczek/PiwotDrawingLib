using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiwotDrawingLib.UI.Controls
{
    /// <summary>
    /// Abstract class representing a MenuControl that can invoke actions.
    /// </summary>
    abstract class ActionControl: AccessableControl
    {
        /// <summary>
        /// List of actions performed when that control is selected.
        /// </summary>
        public Dictionary<string, Func<Events.MenuEvent, bool>> actions;

        public ActionControl(string name, string identificator) : base(name, identificator) { actions = new Dictionary<string, Func<Events.MenuEvent, bool>>(); }

        /// <summary>
        /// Adds an executable action to this control.
        /// </summary>
        /// /// <param name="identificator">Identificator of the action bo be added.</param>
        /// <param name="action">The action bo be added.</param>
        public void AddAction(string identificator, Func<Events.MenuEvent, bool> action)
        {
            actions[identificator] = action;
        }

        /// <summary>
        /// Removes an existing action. Returns true if removal was succesfull.
        /// </summary>
        /// <param name="identificator">Identificator of the action bo be removed.</param>
        public bool RemoveAction(string identificator)
        {
            if (actions.ContainsKey(identificator))
            {
                actions.Remove(identificator);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns an existing action.
        /// </summary>
        /// /// <param name="identificator">Identificator of the action bo be found.</param>
        public Func<Events.MenuEvent, bool> GetAction(string identificator)
        {
            if (actions.ContainsKey(identificator))
            {
                return actions[identificator];
            }
            return null;
        }

        /// <summary>
        /// Invokes all assigned actions.
        /// </summary>
        /// <param name="menuControllEvent"></param>
        protected virtual void RunActions(Events.MenuControllEvent menuControllEvent)
        {
            foreach(string action in actions.Keys)
            {
                actions[action].Invoke(menuControllEvent);
            }
        }
    }
}
