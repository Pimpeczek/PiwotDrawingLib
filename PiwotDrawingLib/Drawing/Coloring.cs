using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace PiwotDrawingLib.Drawing
{
    public static class Coloring
    {
        #region Imports

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);
        #endregion


        private const string _formatStringStart = "\u001b[{0};2;";
        private const string _formatStringColor = "{1};{2};{3}m";
        private const string _formatStringContent = "{4}";
        private const string _formatStringEnd = "\u001b[0m";
        private static readonly string _formatStringFull = $"{_formatStringStart}{_formatStringColor}{_formatStringContent}{_formatStringEnd}";

        private const int outHandle = -11;
        private const uint virtualTerminal = 0x0004;
        private static bool isEnabled;
        private static readonly Regex _closeNestedPastelStringRegex1 = new Regex($"({_formatStringEnd.Replace("[", @"\[")})+");

        static Coloring()
        {
            Console.WriteLine(_closeNestedPastelStringRegex1.Replace(input, _formatStringEnd));
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                IntPtr iStdOut = GetStdHandle(outHandle);

                GetConsoleMode(iStdOut, out var outConsoleMode);
                SetConsoleMode(iStdOut, outConsoleMode | virtualTerminal);
            }
            isEnabled = Environment.GetEnvironmentVariable("NO_COLOR") == null;

        }


        public static string Color(this string text, Color color)
        {
            if (!isEnabled)
                return text;
            return text;
        }

        public static string Color(this string text, string hex)
        {
            if (!isEnabled)
                return text;
            return text;
        }

        public static string Color(this string text, int argb)
        {
            if (!isEnabled)
                return text;
            return text;
        }

        public static string ColorBackground(this string text, Color c)
        {
            if (!isEnabled)
                return text;
            return text;
        }
    }
}
