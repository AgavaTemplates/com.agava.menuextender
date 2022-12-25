using System;
using System.Reflection;
using HarmonyLib;

namespace Agava.MenuExtender
{
    static class WindowExtention
    {
        public static MethodInfo Method(this Window window)
        {
            switch (window)
            {
                case Window.Game:
                    var type = AccessTools.TypeByName("UnityEditor.GameView");
                    return AccessTools.Method(type, "AddItemsToMenu");
                case Window.Hierarhy:
                    type = AccessTools.TypeByName("UnityEditor.SceneHierarchy");
                    return AccessTools.Method(type, "AddItemsToWindowMenu");
                default:
                    throw new NotImplementedException($"{window} has no implementation");
            }
        }
    }
}
