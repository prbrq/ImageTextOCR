﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTextOCR
{
    static class SupportedExtensions
    {
        public static string[] Extensions { get; } = new string[]
        {
            ".jpg",
            "jpeg",
            ".png",
            ".bmp",
        };

        public static string ExplorerFilter
        {
            get
            {
                var explorerFilterString = String.Concat(Extensions.SelectMany(extension => "*" + extension + ";"));
                if (String.IsNullOrEmpty(explorerFilterString))
                    throw new ArgumentException("Extensions are configured incorrectly");
                else
                    return explorerFilterString;
            }
        }
    }
}
