using System;
using System.Collections.Generic;

namespace Blackcat.Configuration
{
    internal class ConfigFile
    {
        public Metadata Metadata { get; set; }
        public List<ConfigElement> Configs { get; set; }
    }

    internal class ConfigElement
    {
        public string Key { get; set; }
        public object Data { get; set; }
    }

    internal class Metadata
    {
        public DateTime Modified { get; set; }
    }
}