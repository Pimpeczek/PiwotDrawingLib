using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading;

namespace PiwotDrawingLib.Drawing
{
    public static class Synchronizer
    {
        private static List<Syncable> Syncables;
        private static Thread syncThread;
        public static int frameLength = 60;
        public static bool Alive;
        public static bool Working;
        public static bool IsReady { get; private set; }
        static Synchronizer()
        {
            Alive = true;
            Working = true;
            syncThread = new Thread(UpdatingLoop);
            syncThread.Start();
        }
        //RENDERER TO MOŻE SPOKOJNIE ROBIĆ, trzeba to tam dać i podzielić czas klatki na synchronizację i renderowanie.
        private static void UpdatingLoop()
        {
            Stopwatch stopwatch = new Stopwatch();
            while(Alive)
            {
                while(Working)
                {
                    IsReady = false;
                    stopwatch.Restart();
                    Update();
                    IsReady = true;
                    if (stopwatch.ElapsedMilliseconds < frameLength)
                    {
                        Thread.Sleep(frameLength - (int)stopwatch.ElapsedMilliseconds);
                    }
                }
            }
        }

        public static void Register(Syncable syncable)
        {
            if (!Syncables.Contains(syncable))
            {
                Syncables.Add(syncable);
                syncable.OnRegister();
            }
        }

        public static void UnRegister(Syncable syncable)
        {
            if (Syncables.Contains(syncable))
            {
                Syncables.Remove(syncable);
                syncable.OnUnRegister();
            }
        }

        public static void Update()
        {
            foreach(var s in Syncables)
            {
                if(s.IsOutdated)
                    s.Update();
            }
        }
    }
}
