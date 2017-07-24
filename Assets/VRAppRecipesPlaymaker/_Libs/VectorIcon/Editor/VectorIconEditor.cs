#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace ZefirVR
{
	// Custom Inspector Editor for Vector Icon
    [CustomEditor(typeof(VectorIcon)), CanEditMultipleObjects]
    public class VectorIconEditor : Editor
    {
		private SerializedProperty iconFont;
		private SerializedProperty maxSize;
		private SerializedProperty iconSize;

		void OnEnable()
		{
			iconFont = serializedObject.FindProperty("iconFont");
			maxSize = serializedObject.FindProperty ("maxSize");
			iconSize = serializedObject.FindProperty ("iconSize");
		}

        public override void OnInspectorGUI()
        {
            VectorIcon icon = target as VectorIcon;
            serializedObject.Update();

			EditorGUILayout.PropertyField(serializedObject.FindProperty("iconFont"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("maxSize"));
			if (!maxSize.boolValue) {
				EditorGUILayout.PropertyField(serializedObject.FindProperty("iconSize"));
			}

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Color"));

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Icon", GUILayout.Width(EditorGUIUtility.labelWidth));

            GUIStyle iconStyle = new GUIStyle(GUI.skin.label);
            iconStyle.font = icon.font;
            iconStyle.border = new RectOffset(5, 5, 5, 5);
            iconStyle.contentOffset = new Vector2(0, 0);
            iconStyle.alignment = icon.alignment;
            iconStyle.normal.textColor = (EditorGUIUtility.isProSkin ? Color.white : new Color(0.2f, 0.2f, 0.2f));

            if (Selection.gameObjects.Length < 2) {
                GUILayout.Label(new GUIContent(VectorIcon.DecodeString(icon.text), "Icon: " + icon.iconName), iconStyle, GUILayout.Width(20));
            }
            if (GUILayout.Button("Pick Icon")) {
				VectorIconEditorWindow.ShowWindow(icon.iconFont);
            }

            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

		// Create new vector icon game object
        [UnityEditor.MenuItem("GameObject/UI/Vector Icon")]
        public static void CreateVectorIcon()
        {
            GameObject go = new GameObject("Vectror Icon");
            go.AddComponent<VectorIcon>();

            GameObject selectedObj = Selection.activeGameObject;
            if (selectedObj != null) {
                go.transform.SetParent(selectedObj.transform, false);
            }
            Selection.activeGameObject = go;
        }

		[UnityEditor.MenuItem("Component/UI/Vector Icon")]
        public static void AppendVectorIconComponent()
        {
            if (Selection.activeGameObject != null) Selection.activeGameObject.AddComponent<VectorIcon>();
        }

		[UnityEditor.MenuItem("Component/UI/Vector Icon", true)]
        public static bool ValidateAppendVectorIconComponent()
        {
            return Selection.activeTransform != null;
        }
    }
}
#endif
