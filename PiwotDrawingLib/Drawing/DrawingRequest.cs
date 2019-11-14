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
        public System.Drawing.color FID { get; }
        public string BID { get; }
        public int X { get; }
        public int Y { get; }
        public DrawingRequest(string text, string fid, string bid, int x, int y)
        {
            Text = text;
            FID = fid;
            BID = bid;
            X = x;
            Y = y;
        }
    }
}
