using System;

namespace UwpApp.Models
{
    public record NavPageItem(string Tag,Type PageType)
    {
        public string Tag { get; } = Tag;
        public Type PageType { get; } = PageType;
    }
}