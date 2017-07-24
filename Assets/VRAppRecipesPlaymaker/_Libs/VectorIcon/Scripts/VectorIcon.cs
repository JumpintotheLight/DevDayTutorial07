using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ZefirVR
{
    [ExecuteInEditMode, Serializable]
    public class VectorIcon : Text
    {
		[Serializable]
		public enum IconFont {
			FontAwesome,
			MaterialIcons
		}

		public IconFont iconFont = IconFont.FontAwesome;
		public bool maxSize = true;
		public int iconSize = 24;

        private static Dictionary<string, string> _icons;
		private static Dictionary<string, Dictionary<string, string>> allIcons = new Dictionary<string, Dictionary<string, string>>();

		private string loadedFontFile = null;

        [SerializeField]
        public string iconName = "";

        protected override void Start()
        {
            base.Start();
			UpdateFont ();

            this.alignment = TextAnchor.MiddleCenter;
			if (maxSize) {
				this.fontSize = (int)Mathf.Floor(Mathf.Min(this.rectTransform.rect.width, this.rectTransform.rect.height));
			} else this.fontSize = iconSize;
        }

		protected override void OnEnable()
		{
			base.OnEnable();
			UpdateFont ();
			UpdateSize ();
			SetAllDirty();
		}

		public void Refresh()
		{
			UpdateFont ();
		}

		private void UpdateFont()
		{
			#if UNITY_EDITOR
			this.font = Resources.Load<Font>(fontFileName(this.iconFont)) as Font;
			#endif
		}

		// Font file names
		public static string fontFileName(IconFont m_IconFont)
		{
			string fileName = null;
			switch(m_IconFont) {
			case IconFont.FontAwesome:
				fileName = "Fonts/FontAwesome/FontAwesome";
				break;
			case IconFont.MaterialIcons:
				fileName = "Fonts/MaterialIcons/MaterialIcons-Regular";
				break;
			}
			return fileName;
		}

		// Glyph list with names and codes
		public static string fontListFileName(IconFont m_IconFont)
		{
			string fileName  = null;
			switch(m_IconFont) {
			case IconFont.FontAwesome:
				fileName = "Fonts/FontAwesome/FontAwesomeList";
				break;
			case IconFont.MaterialIcons:
				fileName = "Fonts/MaterialIcons/MaterialIconsList";
				break;
			}
			return fileName;
		}

		// Update font size when rect transform is resized
        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
			UpdateSize ();
        }

		private void UpdateSize()
		{
			if (maxSize) {
				this.fontSize = (int)Mathf.Floor(Mathf.Min(this.rectTransform.rect.width, this.rectTransform.rect.height));
			} else this.fontSize = iconSize;
		}

		// Load glyph names
		public static Dictionary<string, string> LoadGlyphNames(string fileName)
        {
			var _icons = new Dictionary<string, string> ();
			TextAsset txt = (TextAsset)Resources.Load(fileName, typeof(TextAsset));
            string[] lines = txt.text.Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.None);

            string key, value;
            foreach (string line in lines)
            {
                if (!line.StartsWith("#") && line.IndexOf("=") >= 0)
                {
                    key = line.Substring(0, line.IndexOf("="));
                    if (!_icons.ContainsKey(key))
                    {
                        value = line.Substring(line.IndexOf("=") + 1,
                            line.Length - line.IndexOf("=") - 1);
                        _icons.Add(key, value);
                    }
                }
            }
			return _icons;
        }

		public static Dictionary<string, string> GetIcons(string fileName)
        {
			if (allIcons.ContainsKey (fileName)) {
				_icons = allIcons [fileName];
			} else {
				_icons = LoadGlyphNames (fileName);
				allIcons.Add (fileName, _icons);
			}
				
            return _icons;
        }

        public static string DecodeString(string value)
        {
			string code = new Regex(@"\\u(?<Value>[a-zA-Z0-9]{4})").Replace(value, m => ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString());
			return code;
        }
    }
}
