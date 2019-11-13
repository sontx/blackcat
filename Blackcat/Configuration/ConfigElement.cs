using System;
using System.Collections.Generic;

namespace Blackcat.Configuration
{
    public class ConfigFile
    {
        public static readonly ConfigFile Empty = new ConfigFile
        {
            Metadata = new Metadata
            {
                Modified = DateTime.Now
            },
            Configs = new List<ConfigElement>(0)
        };

        public Metadata Metadata { get; set; }

        public List<ConfigElement> Configs { get; set; }
    }

    public class ConfigElement
    {
        public string Key { get; set; }
        public object Data { get; set; }
    }

    public class Metadata
    {
        public DateTime Modified { get; set; }
    }
}