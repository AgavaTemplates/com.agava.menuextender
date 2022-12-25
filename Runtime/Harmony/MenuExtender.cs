using HarmonyLib;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Agava.MenuExtender
{
    [InitializeOnLoad]
    public abstract class MenuExtender
    {
        internal static Type[] DerivedTypes =>
            (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
             from type in domainAssembly.GetTypes()
             where type.IsSubclassOf(typeof(MenuExtender))
             select type).ToArray();

        static MenuExtender()
        {
            if (DerivedTypes.Length == 0)
                return;

            var harmony = new Harmony("com.agava.menuextenderpatch");
            harmony.PatchAll();
        }

        public abstract void ConstructMenu(GenericMenu menu);
    }
}
