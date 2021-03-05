using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiwotDrawingLib.Drawing
{
    public abstract class Syncable
    {
        public abstract bool IsOutdated { get; protected set; }
        public abstract void Update();
        public abstract void OnRegister();
        public abstract void OnUnRegister();
        public abstract void Unregister();
        public abstract void Register();
    }
}
