﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Dota_Geek.DataTypes
{
    internal static class NameTruncate
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength) + "..";
        }
    }
}
