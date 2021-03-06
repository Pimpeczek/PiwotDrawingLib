﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiwotDrawingLib.Misc
{
    public static class Boxes
    {
        public enum BoxType { light, round, normal, doubled, dashed, dashedLight, none };
        public static readonly Char cor_b_lu = '╔';
        public static readonly Char cor_b_ru = '╗';
        public static readonly Char cor_b_rl = '╝';
        public static readonly Char cor_b_ll = '╚';
        public static readonly Char cor_b_notd = '╩';
        public static readonly Char cor_b_notu = '╦';
        public static readonly Char cor_b_notl = '╠';
        public static readonly Char cor_b_notr = '╣';
        public static readonly Char cor_b_cross = '╬';
        public static readonly Char wal_b_h = '║';
        public static readonly Char wal_b_v = '═';

        public static readonly Char cor_lu = '┏';
        public static readonly Char cor_ru = '┓';
        public static readonly Char cor_rl = '┛';
        public static readonly Char cor_ll = '┗';
        public static readonly Char cor_notd = '┻';
        public static readonly Char cor_notu = '┳';
        public static readonly Char cor_notl = '┣';
        public static readonly Char cor_notr = '┫';
        public static readonly Char cor_cross = '╋';
        public static readonly Char wal_h = '┃';
        public static readonly Char wal_v = '━';

        public static readonly Char[] doubleBoxChars = { '╗', ' ', '═', '╔', '╦', ' ', '╝', '║', '╣', '╚', '╩', '╠', '╬' };
        public static readonly Char[] boxChars = { '┓', ' ', '━', '┏', '┳', ' ', '┛', '┃', '┫', '┗', '┻', '┣', '╋' };
        public static readonly Char[] lightBoxChars = { '┐', ' ', '─', '┌', '┬', ' ', '┘', '│', '┤', '└', '┴', '├', '┼' };
        public static readonly Char[] roundBoxChars = { '╮', ' ', '─', '╭', '┬', ' ', '╯', '│', '┤', '╰', '┴', '├', '┼' };
        public static readonly Char[] dashedLightBoxChars = { '┐', ' ', '┄', '┌', '┬', ' ', '┘', '┊', '┤', '└', '┴', '├', '┼' };
        public static readonly Char[] dashedBoxChars = { '┓', ' ', '┅', '┏', '┳', ' ', '┛', '┋', '┫', '┗', '┻', '┣', '╋' };

        public static Char GetBoxChar(int id)
        {
            id -= 3;
            if (id < 0 || id > 12)
                return ' ';
            return boxChars[id];
        }
        public static Char GetDoubleBoxChar(int id)
        {
            id -= 3;
            if (id < 0 || id > 12)
                return ' ';
            return doubleBoxChars[id];
        }
        public static Char GetLightBoxChar(int id)
        {
            id -= 3;
            if (id < 0 || id > 12)
                return ' ';
            return lightBoxChars[id];
        }
        public static Char GetDashedBoxChar(int id)
        {
            id -= 3;
            if (id < 0 || id > 12)
                return ' ';
            return dashedBoxChars[id];
        }

        public static int GetIdByneighbours(bool up, bool right, bool down, bool left)
        {
            return (up ? 1 : 0) + (right ? 2 : 0) + (down ? 4 : 0) + (left ? 8 : 0);
        }

        /*
        protected char GetBorderChar(Int2 pos, char[] boxes)
        {
            int id = 0;
            if (pos.x > 0 && borderMap[pos.x - 1, pos.y])
                id += 1;
            if (pos.y < Size.y - 1 && borderMap[pos.x, pos.y + 1])
                id += 2;
            if (pos.x < Size.x - 1 && borderMap[pos.x + 1, pos.y])
                id += 4;
            if (pos.y > 0 && borderMap[pos.x, pos.y - 1])
                id += 8;
            id -= 3;
            return boxes[id];
        }
        */

        public static Char[] GetBoxArray(BoxType boxType)
        {
            Char[] bs;
            switch (boxType)
            {
                case BoxType.dashed:
                    bs = dashedBoxChars;
                    break;
                case BoxType.dashedLight:
                    bs = dashedLightBoxChars;
                    break;
                case BoxType.doubled:
                    bs = doubleBoxChars;
                    break;
                case BoxType.light:
                    bs = lightBoxChars;
                    break;
                case BoxType.round:
                    bs = roundBoxChars;
                    break;
                case BoxType.normal:
                    bs = boxChars;
                    break;
                default:
                    bs = null;
                    break;
            }
            return bs;
        }

        public static void DrawBox(BoxType boxType, int x, int y, int sx, int sy)
        {
            DrawBox(GetBoxArray(boxType), x, y, sx, sy);
        }

        public static void DrawBox(Char[] bs, int x, int y, int sx, int sy)
        {
            if (bs == null)
                throw new ArgumentNullException();
            Drawing.Renderer.DrawFormated($"{bs[3]}{"".PadLeft(sx - 2, bs[2])}{bs[0]}", x, y);
            string midBorder = $"{bs[7]}{"".PadLeft(sx - 2)}{bs[7]}";
            for (int i = 1; i < sy - 1; i++)
            {
                Drawing.Renderer.DrawFormated(midBorder, x, y + i);

            }
            Drawing.Renderer.DrawFormated($"{bs[9]}{"".PadLeft(sx - 2, bs[2])}{bs[6]}", x, y + sy - 1);

        }


        public static void DrawBox(Drawing.Canvas canvas, BoxType boxType, int x, int y, int sx, int sy)
        {
            DrawBox(canvas, GetBoxArray(boxType), x, y, sx, sy, true);
        }

        public static void DrawBox(Drawing.Canvas canvas, BoxType boxType, int x, int y, int sx, int sy, bool fill)
        {
            DrawBox(canvas, GetBoxArray(boxType), x, y, sx, sy, fill);
        }
        public static void DrawBox(Drawing.Canvas canvas, Char[] bs, int x, int y, int sx, int sy, bool fill)
        {
            if (bs == null)
                throw new ArgumentNullException();
            canvas.DrawOnCanvas($"{bs[3]}{"".PadLeft(sx - 2, bs[2])}{bs[0]}", x, y);
            if (fill)
            {
                string midBorder = $"{bs[7]}{"".PadLeft(sx - 2)}{bs[7]}";
                for (int i = 1; i < sy - 1; i++)
                {
                    canvas.DrawOnCanvas(midBorder, x, y + i);
                }
            }
            else
            {
                for (int i = 1; i < sy - 1; i++)
                {
                    canvas.DrawOnCanvas(bs[7], x, y + i);
                    canvas.DrawOnCanvas(bs[7], x + sx - 1, y + i);
                }
            }
            canvas.DrawOnCanvas($"{bs[9]}{"".PadLeft(sx - 2, bs[2])}{bs[6]}", x, y + sy - 1);
        }

        public static string GetBoxName(string name, BoxType boxType)
        {
            return GetBoxName(name, GetBoxArray(boxType));
        }

        public static string GetBoxName(string name, Char[] boxes)
        {
            if (boxes == null)
                throw new ArgumentNullException();
            return $"{boxes[8]}{name}{boxes[11]}";
        }
    }
}
