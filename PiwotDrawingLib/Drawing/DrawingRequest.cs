using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiwotDrawingLib.Drawing
{
    class DrawingRequest
    {
        public string Text { get; }
        public int FID { get; }
        public int BID { get; }
        public int X { get; }
        public int Y { get; }
        public DrawingRequest(string text, int fid, int bid, int x, int y)
        {
            Text = text;
            FID = fid;
            BID = bid;
            X = x;
            Y = y;
        }
    }
}
