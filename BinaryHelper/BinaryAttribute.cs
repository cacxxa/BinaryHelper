using System;
using System.Runtime.CompilerServices;

namespace BinaryHelper
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class BinaryAttribute : Attribute
    {
        public BinaryAttribute([CallerLineNumber]int order = 0)
        {
            Order = order;
        }

        public int Order { get; set; }
        public uint Skip { get; set; }
        public uint Count { get; set; }
        public TextEncoding Encoding { get; set; } = TextEncoding.Utf8;
        public bool Reverse { get; set; }
        public bool IgnoreOnRead { get; set; }
        public bool IgnoreOnWrite { get; set; }
        public CustomType CustomType { get; set; }
    }
}
