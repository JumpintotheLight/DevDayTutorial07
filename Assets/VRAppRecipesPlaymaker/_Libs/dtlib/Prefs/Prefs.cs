using UnityEngine;
using System;
using System.Collections;

/*
 * Usage:
 *   set value:
 *     Prefs.Set("key", 123);
 * 
 *   get value of any type:
 *     object = Prefs.Get("key");
 * 
 *   get value:
 *     int = Prefs.Get<int>("key");
 * 
 *   check key existence:
 *     bool = Prefs.HasKey("key");
 * 
 *   delete key:
 *     Prefs.DeleteKey("key");
 *
 *   delete all keys:
 *     Prefs.DeleteAll();
 * 
 *   save changes:
 *     Prefs.Save();
 * 
 *   Hashtable h = Prefs.Load<Hashtable>("myhash");
 *   ...
 *   Prefs.Save("myhash", h);
 * 
 *   var myprefs = Prefs.Load("myprefs");
 *   myprefs["key"] = "any data";
 *   myprefs["key2"] = new int[] {2, 3, 4};
 *   string str = myprefs.Get<string>("key");
 *   myprefs.Get("key");
 *   myprefs.DeleteAll();
 *   myprefs.DeleteKey("key");
 *   myprefs.Save();
 * 
 */
public class Prefs {
	public class Data : IEnumerable {
		public bool dirty = false;

		string id;
		bool temporary = false;
		Hashtable data;
		int initialSize;

		public Data(string id, bool temporary=false) {
			this.id = id;
			this.temporary = temporary;
			byte[] bytes = Read(id, temporary);
			initialSize = bytes != null ? bytes.Length : 256;
			data = BinMsg.Decode(bytes) as Hashtable;
			if (data == null)
				data = new Hashtable(16);
			dirty = false;
		}

		public void SetData(Hashtable data) {
			if (this.data.Count == 0 && data.Count == 0)
				return;
			this.data = data;
			dirty = true;
		}

		public Hashtable GetData() {
			return data;
		}
		
		public void DeleteAll() {
			if (data.Count > 0) {
				data.Clear();
				dirty = true;
			}
		}

		public void DeleteKey(string key) {
			if (data.Count > 0) {
				data.Remove(key);
				dirty = true;
			}
		}

		public bool HasKey(string key) {
			return data.ContainsKey(key);
		}

		public object this[string key] {
			get {
				return data[key];
			}
			set {
				dirty = true;
				data[key] = value;
			}
		}

		public void Set(string key, object value) {
			data[key] = value;
			dirty = true;
		}

		public T Get<T>(string key) {
			object o = data[key];
			if (o == null)
				return default(T);
			return (T)o;
		}

		public T Get<T>(string key, T defaultValue) {
			object o = data[key];
			if (o == null || !(o is T))
				return defaultValue;
			return (T)o;
		}

		public void Save() {
			if (dirty) {
				Write(id, BinMsg.Encode(data, initialSize + 32), temporary);
				dirty = false;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return data.GetEnumerator();
		}
	}

	static Data _defaultPrefs = null;
	static Data defaultPrefs {
		get {
			if (_defaultPrefs == null)
				_defaultPrefs = Load("defaults");
			return _defaultPrefs;
		}
	}

	public static void DeleteAll() {
		defaultPrefs.DeleteAll();
	}

	public static void DeleteKey(string key) {
		defaultPrefs.DeleteKey(key);
	}

	public static bool HasKey(string key) {
		return defaultPrefs.HasKey(key);
	}

	public static void Set(string key, object value) {
		defaultPrefs[key] = value;
	}

	public static object Get(string key) {
		return defaultPrefs[key];
	}

	public static T Get<T>(string key) {
		return defaultPrefs.Get<T>(key);
	}

	public static T Get<T>(string key, T defaultValue) {
		return defaultPrefs.Get<T>(key, defaultValue);
	}

	public static void Save(bool force=false) {
		defaultPrefs.dirty = defaultPrefs.dirty || force;
		defaultPrefs.Save();
	}

	public static Data Load(string id, bool temporary=false) {
		return new Data(id, temporary);
	}
	
	public static T Load<T>(string id, bool temporary=false) {
		object o = BinMsg.Decode(Read(id, temporary));
		if (o == null)
			return default(T);
		return (T)o;
	}

	public static void Save(string id, object o, bool temporary=false) {
		Write(id, BinMsg.Encode(o), temporary);
	}

	public static string GetPath(string id, bool temporary=false) {
		#if UNITY_WEBGL || UNITY_WEBPLAYER
		return "_prefs" + (id != null ? "-" + id : "");
		#else
		string folder;
		if (temporary)
			folder = Application.temporaryCachePath;
		else
			folder = Application.persistentDataPath;
		return folder + "/prefs" + (id != null ? "-" + id : "") + ".txt";
		#endif
	}

	public static void Remove(string id, bool temporary=false) {
		if (!string.IsNullOrEmpty(id)) {
			#if UNITY_WEBGL || UNITY_WEBPLAYER
			#else
				string path = GetPath(id, temporary);
				#if UNITY_WEBPLAYER
				PlayerPrefs.DeleteKey(path);
				#else
				if (System.IO.File.Exists(path))
					System.IO.File.Delete(path);
				#endif
			#endif
		}
	}

	static byte[] Read(string id, bool temporary=false) {
		byte[] bytes = null;
		if (!string.IsNullOrEmpty(id)) {
		#if UNITY_WEBGL || UNITY_WEBPLAYER
		#else
			string path = GetPath(id, temporary);
			#if UNITY_WEBPLAYER
			string text = PlayerPrefs.GetString(path);
			if (text.Length > 0)
				bytes = System.Convert.FromBase64String(text);
			#else
			if (System.IO.File.Exists(path))
				bytes = System.IO.File.ReadAllBytes(path);
			#endif
		#endif
		}
		return bytes;
	}

	static void Write(string id, byte[] bytes, bool temporary=false) {
		if (!string.IsNullOrEmpty(id)) {
		#if UNITY_WEBGL || UNITY_WEBPLAYER
		#else
			string path = GetPath(id, temporary);
			#if UNITY_WEBPLAYER
			PlayerPrefs.SetString(path, System.Convert.ToBase64String(bytes));
			PlayerPrefs.Save();
			#else
			System.IO.File.WriteAllBytes(path, bytes);
			#endif
		#endif
		}
	}
}
