using Kingdom_of_Creation.Dtos;
using Kingdom_of_Creation.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;

namespace Kingdom_of_Creation.Definitions
{
    public static class ColorDefinitions
    {
        public static readonly Color_4 White = new Color_4(1f, 1f, 1f, 1f);
        public static readonly Color_4 Black = new Color_4(0f, 0f, 0f, 1f);
        public static readonly Color_4 Gray = new Color_4(0.5f, 0.5f, 0.5f, 1f);
        public static readonly Color_4 LightGray = new Color_4(0.75f, 0.75f, 0.75f, 1f);
        public static readonly Color_4 DarkGray = new Color_4(0.25f, 0.25f, 0.25f, 1f);
        public static readonly Color_4 Red = new Color_4(1f, 0f, 0f, 1f);
        public static readonly Color_4 Green = new Color_4(0f, 1f, 0f, 1f);
        public static readonly Color_4 Blue = new Color_4(0f, 0f, 1f, 1f);
        public static readonly Color_4 Yellow = new Color_4(1f, 1f, 0f, 1f);
        public static readonly Color_4 Cyan = new Color_4(0f, 1f, 1f, 1f);
        public static readonly Color_4 Magenta = new Color_4(1f, 0f, 1f, 1f);
        public static readonly Color_4 Transparent = new Color_4(0f, 0f, 0f, 0f);
    }
}
