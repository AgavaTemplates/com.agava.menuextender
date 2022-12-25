using System;

namespace Agava.MenuExtender
{
    public class MenuWindowAttribute : Attribute
    {
        public Window Window { get; }
        public MenuWindowAttribute(Window window) => Window = window;
    }
}
