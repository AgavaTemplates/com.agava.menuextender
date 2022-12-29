using UnityEditor;
using UnityEngine;

namespace Agava.MenuExtender.Samples
{
    [MenuWindow(Window.Game)]
    public class GameViewExtender : MenuExtender
    {
        private static readonly GUIContent _disabledButtonContent = EditorGUIUtility.TrTextContent("Disabled Button");
        private static string[] _nestedStatesSave;

        private const int NestedCount = 5;
        private const string TestButtonStateSave = nameof(TestButtonStateSave);
        private const string DisabledButtonStateSave = nameof(DisabledButtonStateSave);

        static GameViewExtender()
        {
            _nestedStatesSave = new string[NestedCount];
            for (int i = 0; i < NestedCount; i++)
                _nestedStatesSave[i] = $"NestedStateSave{i}";
        }

        public override void ConstructMenu(GenericMenu menu)
        {
            var customContent = EditorGUIUtility.TrTextContent("Test Button");
            var testButtonState = EditorPrefs.GetBool(TestButtonStateSave, false);
            menu.AddItem(customContent, testButtonState, OnTestButtonClick);

            menu.AddSeparator("");

            if (testButtonState == false)
                menu.AddDisabledItem(_disabledButtonContent);
            else
                menu.AddItem(_disabledButtonContent, EditorPrefs.GetBool(DisabledButtonStateSave), OnDisabledButtonClick);

            for (int i = 0; i < NestedCount; i++)
            {
                var indexCopy = i;
                var state = EditorPrefs.GetBool(_nestedStatesSave[i]);
                menu.AddItem(EditorGUIUtility.TrTextContent($"Nested/Nested{i}"), state, () => { OnNestedButtonClicked(indexCopy); });
            }

            menu.AddSeparator("");
        }

        private void OnTestButtonClick()
        {

            var state = EditorPrefs.GetBool(TestButtonStateSave);
            EditorPrefs.SetBool(TestButtonStateSave, !state);
            Debug.Log(nameof(OnTestButtonClick));
        }

        private void OnDisabledButtonClick()
        {
            var state = EditorPrefs.GetBool(DisabledButtonStateSave);
            EditorPrefs.SetBool(DisabledButtonStateSave, !state);
            Debug.Log(nameof(OnDisabledButtonClick));
        }

        private void OnNestedButtonClicked(int index)
        {
            var state = EditorPrefs.GetBool(_nestedStatesSave[index]);
            EditorPrefs.SetBool(_nestedStatesSave[index], !state);
            Debug.Log(nameof(OnNestedButtonClicked) + "[" + index + "]");
        }
    }
}
