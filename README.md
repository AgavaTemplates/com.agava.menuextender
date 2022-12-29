# Unity Menu Extender

### Implements the expansion of the "Game", "Scene", "Inspector "and "Hierarchy" window, but it is potentially possible to expand any window.  

![Imgur](https://imgur.com/X5hUCdx.png)

## Usage
Simple example:
```C#
[MenuWindow(Window.Game)]
public class GameViewExtender : MenuExtender
{
      private bool _isOn = false;

      public override void ConstructMenu(GenericMenu menu)
      {
          var customContent = EditorGUIUtility.TrTextContent("Test Button");
          menu.AddItem(customContent, _isOn, OnTestButtonClick);
          menu.AddSeparator("");
      }

      private void OnTestButtonClick()
      {
          _isOn = !_isOn;
      }
  }
```
You can use EditorPrefs to save button states.
## Installation

Make sure you have standalone Git installed first. Reboot after installation.  
In Unity, open "Window" -> "Package Manager".  
Click the "+" sign on top left corner -> "Add package from git URL..."  
Paste this: https://github.com/AgavaTemplates/com.agava.menuextender.git#1.0.1  
See minimum required Unity version in the package.json file.  
Find "Samples" in the package window and click the "Import" button. Use it as a guide.  
To update the package, simply add it again while using a different version tag.  
