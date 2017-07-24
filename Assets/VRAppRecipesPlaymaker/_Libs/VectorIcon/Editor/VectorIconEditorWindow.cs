#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ZefirVR
{
	// Vector Icon Picker Window
    class VectorIconEditorWindow : EditorWindow
    {
        private string searchInput;
        private Vector2 scrollPos = Vector2.zero;

		public string fontFileName;
		public string fontListFileName;

		private Dictionary<string, string> icons;
		private List<string> iconNamesList;

		// Create a new picker window
		public static void ShowWindow(VectorIcon.IconFont iconFont)
        {
			var window = CreateInstance <VectorIconEditorWindow> ();
			window.ShowAuxWindow();
			window.minSize = new Vector2(397, 446);
			window.fontFileName = VectorIcon.fontFileName (iconFont);
			window.fontListFileName = VectorIcon.fontListFileName(iconFont);
			window.titleContent = new GUIContent("Icon Picker");
			window.FindIconNames ();
        }

        void OnGUI()
        {
            DrawSearchField();
            DrawIconPicker();
        }

		// Show search field
        private void DrawSearchField()
        {
            EditorGUILayout.BeginHorizontal();
            searchInput = EditorGUILayout.TextField(searchInput, GUI.skin.FindStyle("ToolbarSeachTextField"));
            if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton"))) {
                searchInput = "";
                GUI.FocusControl(null);
            }
            EditorGUILayout.EndHorizontal();
        }

		// Show Icon Picker area
        private void DrawIconPicker()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
			FindIconNames ();
            DrawIcons();
            EditorGUILayout.EndScrollView();
        }

        private void DrawIcons()
        {
            string name, value;
            float elementWidth = 50;
            int index = 0;
            GUIStyle iconStyle = new GUIStyle(GUI.skin.button);
			iconStyle.font = Resources.Load<Font>(fontFileName) as Font;
            iconStyle.border = new RectOffset(5, 5, 5, 5);
            iconStyle.contentOffset = new Vector2(0, 0);
            iconStyle.alignment = TextAnchor.MiddleCenter;
            iconStyle.normal.textColor = (EditorGUIUtility.isProSkin ? Color.white : new Color(0.2f, 0.2f, 0.2f));

            while (iconNamesList.Count > 0) {
                index++;
                name = iconNamesList[0];
                iconNamesList.RemoveAt(0);

                EditorGUILayout.BeginHorizontal();
                float remainingSpace = EditorGUIUtility.currentViewWidth - EditorGUIUtility.currentViewWidth / 10;
                while (iconNamesList.Count > 0 && remainingSpace > elementWidth) {
                    Color oldColor = GUI.backgroundColor;
                    GUI.backgroundColor = Color.clear;
                    if (Selection.gameObjects != null) {
                        for (int i = 0; i < Selection.gameObjects.Length; i++) {
                            GameObject go = Selection.gameObjects[i];
                            if (go.GetComponent<VectorIcon>() != null) {
                                VectorIcon icon = go.GetComponent<VectorIcon>();
                                if (name.Equals(icon.iconName)) {
                                    GUI.backgroundColor = new Color(1, 1, 1, 0.5f);
                                }
                            }
                        }
                    }
					if (GUILayout.Button(new GUIContent(VectorIcon.DecodeString("\\u" + icons[name]), "Icon: " + name + " " + icons[name]), iconStyle, GUILayout.Width(elementWidth), GUILayout.Height(50)))
                    {
                        if (Selection.gameObjects != null) {
                            for (int i = 0; i < Selection.gameObjects.Length; i++) {
                                GameObject go = Selection.gameObjects[i];
                                if (go.GetComponent<VectorIcon>() != null) {
                                    VectorIcon icon = go.GetComponent<VectorIcon>();
                                    Undo.RecordObject(icon, "Changed icon of " + icon.name);
                                    icon.text = VectorIcon.DecodeString("\\u" + icons[name]);
                                    icon.iconName = name;
                                    EditorUtility.SetDirty(icon);
                                }
                            }
                        }
                    }
                    GUI.backgroundColor = oldColor;
                    remainingSpace -= elementWidth;
                    index++;
                    name = iconNamesList[0];
                    iconNamesList.RemoveAt(0);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

		// Find icons by names and values that match search input string
		private void FindIconNames()
		{
			icons = VectorIcon.GetIcons(fontListFileName);
			iconNamesList = new List<string>(icons.Keys);
			if (!string.IsNullOrEmpty(searchInput)) {
				string inputValue = searchInput.ToLower ();
				for (int i = 0; i < iconNamesList.Count; i++) {
					if (!iconNamesList[i].ToLower().Contains(inputValue)) {
						iconNamesList.RemoveAt(i);
						i--;
					}
				}
			}			
		}
    }
}
#endif