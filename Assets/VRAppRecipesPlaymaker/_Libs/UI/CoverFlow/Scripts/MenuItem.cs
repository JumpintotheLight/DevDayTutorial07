using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ZefirVR {
	
	[Serializable]
	public class MenuItem {
		public string title;
		public string coverImageUrl;
		public Texture2D cover;

		[NonSerialized]
		public int index;
	}

}