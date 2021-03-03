using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace PiwotDrawingLib.Drawing
{
    static class Coloring
    {


        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        private const int stdOutputHandle = -11;
        private const uint enableVirtualTerminalProcessingFlag = 0x0004;

        private delegate string ColorFormat(string input, Color color);
        private delegate string HexFormat(string input, string color);

        private enum Layer : byte
        {
            Front,
            Back
        }

        private const string formatBegin = "\u001b[{0};2;";
        private const string formatColor = "{1};{2};{3}m";
        private const string formatText = "{4}";
        private const string formatEnd = "\u001b[0m";
        private static readonly string formatFull = $"{formatBegin}{formatColor}{formatText}{formatEnd}";


        private static readonly ReadOnlyDictionary<Layer, string> layerIndicator = new ReadOnlyDictionary<Layer, string>(new Dictionary<Layer, string>
        {
            [Layer.Front] = "38",
            [Layer.Back] = "48"
        });



        private static readonly Regex nestedRegex1 = new Regex($"({formatEnd.Replace("[", @"\[")})+", RegexOptions.Compiled);
        private static readonly Regex nestedRegex2 = new Regex($"(?<!^)(?<!{formatEnd.Replace("[", @"\[")})(?<!{string.Format($"{formatBegin.Replace("[", @"\[")}{formatColor}", new[] { $"(?:{layerIndicator[Layer.Front]}|{layerIndicator[Layer.Back]})" }.Concat(Enumerable.Repeat(@"\d{1,3}", 3)).Cast<object>().ToArray())})(?:{string.Format(formatBegin.Replace("[", @"\["), $"(?:{layerIndicator[Layer.Front]}|{layerIndicator[Layer.Back]})")})", RegexOptions.Compiled);

        private static readonly ReadOnlyDictionary<Layer, Regex> nestedRegex3 = new ReadOnlyDictionary<Layer, Regex>(new Dictionary<Layer, Regex>
        {
            [Layer.Front] = new Regex($"(?:{formatEnd.Replace("[", @"\[")})(?!{string.Format(formatBegin.Replace("[", @"\["), layerIndicator[Layer.Front])})(?!$)", RegexOptions.Compiled),
            [Layer.Back] = new Regex($"(?:{formatEnd.Replace("[", @"\[")})(?!{string.Format(formatBegin.Replace("[", @"\["), layerIndicator[Layer.Back])})(?!$)", RegexOptions.Compiled)
        });



        private static readonly Func<string, int> hexToInt = hex => int.Parse(hex, NumberStyles.HexNumber);

        private static readonly Func<string, Color, Layer, string> colorFormat = (t, c, l) => string.Format(formatFull, layerIndicator[l], c.R, c.G, c.B, NestingCommands(t, c, l));
        private static readonly Func<string, string, Layer, string> hexFormat = (t, h, l) => colorFormat(t, Color.FromArgb(hexToInt(h)), l);

        private static readonly ColorFormat disabledColorFormat = (t, c) => t;
        private static readonly HexFormat disabledHexFormat = (t, h) => t;

        private static readonly ColorFormat frontColorFormat = (t, c) => colorFormat(t, c, Layer.Front);
        private static readonly HexFormat frontHexFormat = (t, h) => hexFormat(t, h, Layer.Front);

        private static readonly ColorFormat backColorFormat = (t, c) => colorFormat(t, c, Layer.Back);
        private static readonly HexFormat backHexFormat = (t, h) => hexFormat(t, h, Layer.Back);



        private static readonly ReadOnlyDictionary<bool, ReadOnlyDictionary<Layer, ColorFormat>> colorFormats = new ReadOnlyDictionary<bool, ReadOnlyDictionary<Layer, ColorFormat>>(new Dictionary<bool, ReadOnlyDictionary<Layer, ColorFormat>>
        {
            [false] = new ReadOnlyDictionary<Layer, ColorFormat>(new Dictionary<Layer, ColorFormat>
            {
                [Layer.Front] = disabledColorFormat,
                [Layer.Back] = disabledColorFormat
            }),
            [true] = new ReadOnlyDictionary<Layer, ColorFormat>(new Dictionary<Layer, ColorFormat>
            {
                [Layer.Front] = frontColorFormat,
                [Layer.Back] = backColorFormat
            })
        });
        private static readonly ReadOnlyDictionary<bool, ReadOnlyDictionary<Layer, HexFormat>> hexFormats = new ReadOnlyDictionary<bool, ReadOnlyDictionary<Layer, HexFormat>>(new Dictionary<bool, ReadOnlyDictionary<Layer, HexFormat>>
        {
            [false] = new ReadOnlyDictionary<Layer, HexFormat>(new Dictionary<Layer, HexFormat>
            {
                [Layer.Front] = disabledHexFormat,
                [Layer.Back] = disabledHexFormat
            }),
            [true] = new ReadOnlyDictionary<Layer, HexFormat>(new Dictionary<Layer, HexFormat>
            {
                [Layer.Front] = frontHexFormat,
                [Layer.Back] = backHexFormat
            })
        });

        private static bool enabled;

        static Coloring()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                IntPtr iStdOut = GetStdHandle(stdOutputHandle);

                GetConsoleMode(iStdOut, out var outConsoleMode);
                SetConsoleMode(iStdOut, outConsoleMode | enableVirtualTerminalProcessingFlag);
            }


            if (Environment.GetEnvironmentVariable("NO_COLOR") == null)
            {
                Enable();
            }
            else
            {
                Disable();
            }
        }

        private static string NestingCommands(string input, Color color, Layer layer)
        {
            string closedString = nestedRegex1.Replace(input, formatEnd);
            closedString = nestedRegex2.Replace(closedString, $"{formatEnd}$0");
            input = string.Format($"{formatBegin}{formatColor}",
                                  layerIndicator[layer],
                                  color.R, color.G, color.B);
            closedString = nestedRegex3[layer].Replace(closedString, $"$0{input}");
            return closedString;
        }

        public static string Colors(this string input, Color front, Color back)
        {
            return Back(Front(input, front), back);
        }

        public static string Colors(this string input, string front, string back)
        {
            return Back(Front(input, front), back);
        }

        public static string Front(this string input, Color color)
        {
            return colorFormats[enabled][Layer.Front](input, color);
        }

        public static string Front(this string input, string hexColor)
        {
            return hexFormats[enabled][Layer.Front](input, hexColor);
        }

        public static string Back(this string input, Color color)
        {
            return colorFormats[enabled][Layer.Back](input, color);
        }

        public static string Back(this string input, string hexColor)
        {
            return hexFormats[enabled][Layer.Back](input, hexColor);
        }

        public static void Enable()
        {
            enabled = true;
        }

        public static void Disable()
        {
            enabled = false;
        }
    }
}
