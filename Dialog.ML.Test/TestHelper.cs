﻿using System;
using System.IO;
using System.Reflection;

namespace Dialog.ML.Test
{
    public static class TestHelper
    {
        public static string directory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}
