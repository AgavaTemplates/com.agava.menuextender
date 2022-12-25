using UnityEditor;
using UnityEngine;

namespace Agava.MenuExtender.Samples
{
    [MenuWindow(Window.Game)]
    public class GameViewExtender : MenuExtender
    {
        private bool _isOn = false;

        public override void ConstructMenu(GenericMenu menu)
        {
            var customContent = EditorGUIUtility.TrTextContent("MyButton");
            menu.AddItem(customContent, _isOn, Handler);
        }

        private void Handler()
        {
            _isOn = !_isOn;
        }
    }
}
