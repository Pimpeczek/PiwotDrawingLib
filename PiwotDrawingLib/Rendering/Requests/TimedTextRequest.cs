
namespace PiwotDrawingLib.Rendering.Requests
{
    /// <summary>
    /// Class representing request for a string to be shown on screen for a given amount of time.
    /// </summary>
    public class TimedTextRequest
    {

        /// <summary>
        /// Time until the text will be erased.
        /// </summary>
        public int lifeTime { get; protected set; }
        /// <summary>
        /// String to be printed.
        /// </summary>
        public string Text { get; protected set; }

        /// <summary>
        /// String to be printed stripped of Pastel wrapper.
        /// </summary>
        public string RawText { get; protected set; }
        /// <summary>
        /// Horisontal position.
        /// </summary>
        public int X { get; protected set; }
        /// <summary>
        /// Vertical position; 0 being on top.
        /// </summary>
        public int Y { get; protected set; }
        public string ClearingString { get; protected set; }
        public TimedTextRequest(string text, string clearingString, int lifeTime, int x, int y)
        {
            X = x;
            Y = y;
            Text = text;
            if (clearingString.Length < text.Length)
            {
                clearingString = clearingString.PadRight(text.Length - clearingString.Length, ' ');
            }
            ClearingString = clearingString;
            this.lifeTime = lifeTime;
        }

        public void Age(int time)
        {
            lifeTime -= time;
            if (lifeTime <= 0)
            {
                Renderer.EraseTimedText(this);
            }
        }
    }
}
