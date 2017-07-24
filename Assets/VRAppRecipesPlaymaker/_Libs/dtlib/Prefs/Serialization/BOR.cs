using UnityEngine;
using System.Collections;
using System;

//
// high 3 bits:
//
// 0x00 - uint
// 0x20 - negint
// 0x40 - ascii string
// 0x60 - utf8 string
// 0x80 - string back reference
// 0xa0 - array
// 0xc0 - map
// 0xe0 - tag
//
// lower 5 bits:
//
// 0x00 - 0x1b
// 0x1c - 1 byte follows
// 0x1d - 2 byte follows
// 0x1e - 4 byte follows
// 0x1f - 8 byte follows
//
// tags:
//
// 0x00..0x02 - reserved
// 0x03 - System.DateTime
// 0x04 - vector2
// 0x05 - vector3
// 0x06 - vector4
// 0x07 - color32
// 0x08 - array of bool
// 0x09 - array of byte
// 0x0a - array of int
// 0x0b - array of float
// 0x0c - array of vector2
// 0x0d - array of vector3
// 0x0e - array of vector4
// 0x0f - array of color32
// 0x10..0x17 - reserved
// 0x18 - null
// 0x19 - false
// 0x1a - true
// 0x1b - 
// 0x1c - 
// 0x1d - float16
// 0x1e - float32
// 0x1f - float64
//

public class BOR
{
	byte[] data;
	string[] stringRef;
	int stringRefIndex;
	int index;

	public static byte[] Encode(object o)
	{
		return new BOR().EncodeValue(o);
	}

	public static object Decode(byte[] data)
	{
		if (data == null || data.Length < 1)
			return null;
		return new BOR().DecodeValue(data);
	}

	byte[] EncodeValue(object o)
	{
		data = new byte[6144 + 256];
		data[0] = 0; // reserved
		index = 1;
		stringRef = new string[256];
		stringRefIndex = 0;
		Add (o);
		System.Array.Resize<byte> (ref data, index);
		return data;
	}

	/*
	 * Encode
	 */
	void Add (object o)
	{
		if (o == null)
		{
			if (data.Length <= index)
				IncreaseDataSize (1);
			data[index++] = 0xf8;
			return;
		}

		Type t = o.GetType ();
		if (t == typeof(string))
			Add ((string)o);
		else if (t == typeof(int))
		{
			if (data.Length <= index + 9)
				IncreaseDataSize (9);
			int v = (int)o;
			if (v >= 0)
				Add (0, v);
			else
				Add (0x20, -v - 1);
		}
		else if (t == typeof(bool)) {
			if (data.Length <= index)
				IncreaseDataSize (1);
			data[index++] = (byte)((bool)o ? 0xfa : 0xf9);
		} else if (t == typeof(float))
			Add ((float)o);
		else if (t == typeof(Hashtable))
			Add ((Hashtable)o);
		else if (t == typeof(ArrayList))
			Add ((ArrayList)o);
		else if (t == typeof(Vector2))
			Add ((Vector2)o);
		else if (t == typeof(Vector3))
			Add ((Vector3)o);
		else if (t == typeof(Vector4))
			Add ((Vector4)o);
		else if (t == typeof(Color32))
			Add ((Color32)o);
		else if (t == typeof(double))
			Add ((double)o);
		else if (t == typeof(System.DateTime))
			Add ((System.DateTime)o);
		else if (t.IsArray) {
			Type et = t.GetElementType();
			if (et == typeof(bool))
				Add ((bool[])o);
			else if (et == typeof(byte))
				Add ((byte[])o);
			else if (et == typeof(int))
				Add ((int[])o);
			else if (et == typeof(float))
				Add ((float[])o);
			else if (et == typeof(Vector2))
				Add ((Vector2[])o);
			else if (et == typeof(Vector3))
				Add ((Vector3[])o);
			else if (et == typeof(Vector4))
				Add ((Vector4[])o);
			else if (et == typeof(Color32))
				Add ((Color32[])o);
			else if (et == typeof(string))
				Add ((string[])o);
			else
				Add ((object)null);
		} else
			Add ((object)null);
	}

	void Add (float v)
	{
		if (data.Length <= index + 5)
			IncreaseDataSize (5);
		data[index++] = 0xfe;
		AddValue (v);
	}

	void Add (double v)
	{
		if (data.Length <= index + 9)
			IncreaseDataSize (9);
		data[index++] = 0xff;
		byte[] b = BitConverter.GetBytes(v);
		System.Array.Copy (b, 0, data, index, b.Length);
		index+= b.Length;
	}

	void Add (string v)
	{
		if (v.Length > 1 && v.Length < 64)
		{
			for (int i = stringRefIndex + 255; i > stringRefIndex; i--)
			{
				string s = stringRef [i % 256];
				if (s == null)
					break;
				if (s == v)
				{
					if (data.Length <= index + 2)
						IncreaseDataSize (2);
					int backref = (stringRefIndex - i + 256) % 256;
					Add (0x80, backref);
					return;
				}
			}
		}

		if (data.Length <= index + v.Length + 9)
			IncreaseDataSize (v.Length + 9);

		int idx = index;
		Add (0x40, v.Length);

		bool unicode = false;
		foreach (char c in v) {
			if (c >= 0x80) {
				unicode = true;
				break;
			}
			data[index++] = (byte)c;
		}

		if (unicode) {
			index = idx;
			if (data.Length <= index + v.Length * 2 + 9)
				IncreaseDataSize (v.Length * 2 + 9);

			int initialLen = v.Length * 4;
			Add (0x60, initialLen);

			int start = index;
			foreach (char c in v) {
				if (c < 0x80)
					data[index++] = (byte)c;
				else {
					if (c < 0x800) {
						data[index++] = (byte)(0xc0 | (c >> 6 & 0x1f));
						data[index++] = (byte)(0x80 | (c & 0x3f));
					} else {
						data[index++] = (byte)(0xe0 | (c >> 12 & 0xf));
						data[index++] = (byte)(0x80 | (c >> 6 & 0x3f));
						data[index++] = (byte)(0x80 | (c & 0x3f));
					}
				}
			}
			int len = index - start;
			if (initialLen < 0x1c)
				data[idx] = (byte)(len | 0x60);
			else if (initialLen < 0x100) {
				data[idx] = (byte)(0x1c | 0x60);
				data[idx + 1] = (byte)len;
			} else if (initialLen < 0x10000) {
				data[idx] = (byte)(0x1d | 0x60);
				data[idx + 1] = (byte)(len & 0xff);
				data[idx + 2] = (byte)((len >> 8) & 0xff);
			} else {
				data[idx] = (byte)(0x1e | 0x60);
				data[idx + 1] = (byte)(len & 0xff);
				data[idx + 2] = (byte)((len >> 8) & 0xff);
				data[idx + 3] = (byte)((len >> 16) & 0xff);
				data[idx + 4] = (byte)((len >> 24) & 0xff);
			}
		}

		if (v.Length > 1 && v.Length < 64) {
			stringRef[stringRefIndex] = v;
			stringRefIndex = (stringRefIndex + 1) % 256;
		}
	}

	void Add (ArrayList v)
	{
		if (data.Length <= index + 9)
			IncreaseDataSize (9);
		Add (0xa0, v.Count);
		for (int i = 0; i < v.Count; i++)
			Add (v[i]);
	}
	
	void Add (Hashtable v) {
		if (data.Length <= index + 9)
			IncreaseDataSize (9);
		Add (0xc0, v.Count);
		foreach (DictionaryEntry e in v)
		{
			if (e.Key is string)
				Add ((string)e.Key);
			else
				Add (e.Key);
			Add (e.Value);
		}
	}

	void Add (System.DateTime dateTime) {
		if (data.Length <= index + 5)
			IncreaseDataSize (5);
		DateTime begin = new DateTime (1980, 1, 1, 0, 0, 0, 0);
		TimeSpan diff = dateTime.ToUniversalTime() - begin;
		uint t = (uint)(diff.TotalSeconds);
		data[index++] = 0xe3;
		data[index++] = (byte)(t & 0xff);
		data[index++] = (byte)((t >> 8) & 0xff);
		data[index++] = (byte)((t >> 16) & 0xff);
		data[index++] = (byte)((t >> 24) & 0xff);
	}

	void Add (Vector2 o) {
		if (data.Length <= index + 9)
			IncreaseDataSize (9);
		data[index++] = 0xe4;
		AddValue (o.x);
		AddValue (o.y);
	}
	
	void Add (Vector3 o) {
		if (data.Length <= index + 12)
			IncreaseDataSize (12);
		data[index++] = 0xe5;
		AddValue (o.x);
		AddValue (o.y);
		AddValue (o.z);
	}

	void Add (Vector4 o) {
		if (data.Length <= index + 16)
			IncreaseDataSize (16);
		data[index++] = 0xe6;
		AddValue (o.x);
		AddValue (o.y);
		AddValue (o.z);
		AddValue (o.w);
	}

	void Add (Color32 o) {
		if (data.Length <= index + 5)
			IncreaseDataSize (5);
		data[index++] = 0xe7;
		data[index++] = o.r;
		data[index++] = o.g;
		data[index++] = o.b;
		data[index++] = o.a;
	}

	void Add (bool[] o) {
		if (data.Length <= index + o.Length / 8 + 6)
			IncreaseDataSize (o.Length / 8 + 6);
		data[index++] = 0xe8;
		AddArrayCount (o.Length);
		int c = 7;
		int b = 0;
		for (int i = 0; i < o.Length; i++) {
			b = b | (o[i] ? 1 : 0) << c;
			c--;
			if (c < 0) {
				data[index++] = (byte)b;
				c = 7;
				b = 0;
			}
		}
		if (c != 7)
			data[index++] = (byte)b;
	}

	void Add (byte[] o) {
		if (data.Length <= index + o.Length + 5)
			IncreaseDataSize (o.Length + 5);
		data[index++] = 0xe9;
		AddArrayCount (o.Length);
		System.Array.Copy(o, 0, data, index, o.Length);
		index+= o.Length;
	}

	void Add (int[] o) {
		if (data.Length <= index + o.Length * 5 + 5)
			IncreaseDataSize (o.Length * 5 + 5);
		data[index++] = 0xea;
		AddArrayCount (o.Length);
		for (int i = 0; i < o.Length; i++) {
			int v = o[i];
			if (v >= 0)
				Add (0, v);
			else
				Add (0x20, -v - 1);
		}
	}

	void Add (float[] o) {
		if (data.Length <= index + o.Length * 4 + 5)
			IncreaseDataSize (o.Length * 4 + 5);
		data[index++] = 0xeb;
		AddArrayCount (o.Length);
		for (int i = 0; i < o.Length; i++)
			AddValue(o[i]);
	}

	void Add (Vector2[] o) {
		if (data.Length <= index + o.Length * 8 + 5)
			IncreaseDataSize (o.Length * 8 + 5);
		data[index++] = 0xec;
		AddArrayCount (o.Length);
		for (int i = 0; i < o.Length; i++) {
			Vector2 v = o[i];
			AddValue(v.x);
			AddValue(v.y);
		}
	}

	void Add (Vector3[] o) {
		if (data.Length <= index + o.Length * 12 + 5)
			IncreaseDataSize (o.Length * 12 + 5);
		data[index++] = 0xed;
		AddArrayCount (o.Length);
		for (int i = 0; i < o.Length; i++) {
			Vector3 v = o[i];
			AddValue(v.x);
			AddValue(v.y);
			AddValue(v.z);
		}
	}

	void Add (Vector4[] o) {
		if (data.Length <= index + o.Length * 16 + 5)
			IncreaseDataSize (o.Length * 16 + 5);
		data[index++] = 0xee;
		AddArrayCount (o.Length);
		for (int i = 0; i < o.Length; i++) {
			Vector4 v = o[i];
			AddValue(v.x);
			AddValue(v.y);
			AddValue(v.z);
			AddValue(v.w);
		}
	}

	void Add (Color32[] o) {
		if (data.Length <= index + o.Length * 4 + 5)
			IncreaseDataSize (o.Length * 4 + 5);
		data[index++] = 0xef;
		AddArrayCount (o.Length);
		for (int i = 0; i < o.Length; i++) {
			Color32 v = o[i];
			Add (v.r);
			Add (v.g);
			Add (v.b);
			Add (v.a);
		}
	}

	void Add (string[] o) {
		if (data.Length <= index + 9)
			IncreaseDataSize (9);
		Add (0xa0, o.Length);
		for (int i = 0; i < o.Length; i++)
			Add (o[i]);
	}

	void AddValue (float v)
	{
		byte[] d = BitConverter.GetBytes(v);
		for (int i=0; i<d.Length; i++)
			data[index++] = d[i];
	}

	void Add (byte b)
	{
		if (data.Length <= index)
			IncreaseDataSize (1);
		data[index++] = b;
	}

	void Add (byte b, long r)
	{
		if (r < 0x1c)
			data[index++] = (byte)(b | r);
		else {
			if (r <= 0xff) {
				data[index++] = (byte)(b | 0x1c);
				data[index++] = (byte)(r & 0xff);
			} else if (r <= 0xffff) {
				data[index++] = (byte)(b | 0x1d);
				data[index++] = (byte)(r & 0xff);
				data[index++] = (byte)((r >> 8) & 0xff);
			} else if (r <= 0xffffffff) {
				data[index++] = (byte)(b | 0x1e);
				data[index++] = (byte)(r & 0xff);
				data[index++] = (byte)((r >> 8) & 0xff);
				data[index++] = (byte)((r >> 16) & 0xff);
				data[index++] = (byte)((r >> 24) & 0xff);
			} else {
				data[index++] = (byte)(b | 0x1f);
				for (int i = 0; i < 64; i+= 8)
					data[index++] = (byte)((r >> i) & 0xff);
			}
		}
	}

	void AddArrayCount (int count)
	{
		if (count < 0x80)
			data[index++] = (byte)count;
		else if (count <= 0x3fff) {
			data[index++] = (byte)(0x80 | (count >> 8));
			data[index++] = (byte)(count & 0xff);
		} else {
			data[index++] = (byte)(0xc0 | (count >> 16));
			data[index++] = (byte)((count >> 8) & 0xff);
			data[index++] = (byte)(count & 0xff);
		}
	}

	void IncreaseDataSize (int requiredBytes) {
		int newSize = data.Length * 2;
		if (newSize < data.Length + requiredBytes)
			newSize = data.Length + requiredBytes;
		System.Array.Resize<byte>(ref data, newSize);
	}

	/*
	 * Decode
	 */
	object DecodeValue(byte[] data)
	{
		this.data = data;
		stringRef = new string[256];
		stringRefIndex = 0;
		index = 1;
		return Get ();
	}

	object Get()
	{
		byte b = data[index++];
		int type = b & 0xe0, mask = b & 0x1f;

		if (type == 0xe0)
		{
			switch (mask)
			{
			case 0x03: return GetDateTime ();
			case 0x04: return new Vector2 (GetFloat (), GetFloat ());
			case 0x05: return new Vector3 (GetFloat (), GetFloat (), GetFloat ());
			case 0x06: return new Vector4 (GetFloat (), GetFloat (), GetFloat (), GetFloat ());
			case 0x07: return GetColor32 ();
			case 0x08: return GetBoolArray ();
			case 0x09: return GetByteArray ();
			case 0x0a: return GetIntArray ();
			case 0x0b: return GetFloatArray ();
			case 0x0c: return GetVector2Array ();
			case 0x0d: return GetVector3Array ();
			case 0x0e: return GetVector4Array ();
			case 0x0f: return GetColor32Array ();
			case 0x18: return null;
			case 0x19: return false;
			case 0x1a: return true;
			case 0x1e: // float32
			{
				float res = BitConverter.ToSingle(data, index);
				index+= 4;
				return res;
			}
			case 0x1f: // float64
			{
				double res = BitConverter.ToDouble(data, index);
				index+= 8;
				return res;
			}
			}
			return null;
		}
		
		int r = -1;
		if (mask < 0x1c)
			r = mask;
		else
		{
			// 0x1c - uint8
			r = data[index++];
			if (mask >= 0x1d)
			{
				// 0x1d - uint16
				r|= data[index++] << 8;
				if (mask >= 0x1e)
				{
					// 0x1e - uint32
					r|= data[index++] << 16;
					r|= data[index++] << 24;
					if (mask >= 0x1f)
					{
						// 0x1f - uint64
						r|= data[index++] << 32;
						r|= data[index++] << 40;
						r|= data[index++] << 48;
						r|= data[index++] << 56;
					}
				}
			}
		}

		switch (type)
		{
		case 0x00: // uint
			return r;
		case 0x20: // negint
			return -(r + 1);
		case 0x40: // byte
		{
			string s = System.Text.Encoding.ASCII.GetString(data, index, r);
			index+= r;

			if (s.Length > 1 && s.Length < 64) {
				stringRef[stringRefIndex] = s;
				stringRefIndex = (stringRefIndex + 1) % 256;
			}
			return s;
		}
		case 0x60: // text
		{
			string s = System.Text.Encoding.UTF8.GetString(data, index, r);
			index+= r;

			if (s.Length > 1 && s.Length < 64) {
				stringRef[stringRefIndex] = s;
				stringRefIndex = (stringRefIndex + 1) % 256;
			}
			return s;
		}
		case 0x80:
			return stringRef[(stringRefIndex - r + 256) % 256];
		case 0xa0: // array
		{
			ArrayList a = new ArrayList(r);
			while (r-- > 0)
				a.Add(Get());
			return a;
		}
		case 0xc0: // map
		{
			Hashtable h = new Hashtable(r);
			while (r-- > 0) {
				object k = Get();
				h[k] = Get();
			}
			return h;
		}
		}
		return null;
	}

	bool[] GetBoolArray () {
		bool[] res = new bool[GetArrayCount ()];
		int c = 0;
		byte b = 0;
		for (int i = 0; i < res.Length; i++) {
			if (c == 0)
				b = data[index++];
			res[i] = (b & 0x80) != 0;
			b<<= 1;
			c = (c + 1) % 8;
		}
		return res;
	}

	byte[] GetByteArray () {
		byte[] res = new byte[GetArrayCount ()];
		System.Array.Copy(data, index, res, 0, res.Length);
		index+= res.Length;
		return res;
	}

	int[] GetIntArray () {
		int[] res = new int[GetArrayCount ()];
		for (int i = 0; i < res.Length; i++) {
			byte b = data[index++];
			int type = b & 0xe0, mask = b & 0x1f;
			int r = -1;
			if (mask < 0x1c)
				r = mask;
			else
			{
				// 0x1c - uint8
				r = data[index++];
				if (mask >= 0x1d)
				{
					// 0x1d - uint16
					r|= data[index++] << 8;
					if (mask >= 0x1e)
					{
						// 0x1e - uint32
						r|= data[index++] << 16;
						r|= data[index++] << 24;
					}
				}
			}
			res[i] = type == 0 ? r : -(r + 1);
		}
		return res;
	}

	float[] GetFloatArray () {
		float[] res = new float[GetArrayCount ()];
		for (int i = 0; i < res.Length; i++) {
			res[i] = BitConverter.ToSingle(data, index);
			index+= 4;
		}
		return res;
	}

	Vector2[] GetVector2Array () {
		Vector2[] res = new Vector2[GetArrayCount ()];
		for (int i = 0; i < res.Length; i++) {
			res[i] = new Vector2 (BitConverter.ToSingle(data, index), BitConverter.ToSingle(data, index + 4));
			index+= 8;
		}
		return res;
	}

	Vector3[] GetVector3Array () {
		Vector3[] res = new Vector3[GetArrayCount ()];
		for (int i = 0; i < res.Length; i++) {
			res[i] = new Vector3 (BitConverter.ToSingle(data, index), BitConverter.ToSingle(data, index + 4), BitConverter.ToSingle(data, index + 8));
			index+= 12;
		}
		return res;
	}

	Vector4[] GetVector4Array () {
		Vector4[] res = new Vector4[GetArrayCount ()];
		for (int i = 0; i < res.Length; i++) {
			res[i] = new Vector4 (BitConverter.ToSingle(data, index), BitConverter.ToSingle(data, index + 4), BitConverter.ToSingle(data, index + 8), BitConverter.ToSingle(data, index + 12));
			index+= 16;
		}
		return res;
	}

	Color32[] GetColor32Array () {
		Color32[] res = new Color32[GetArrayCount ()];
		for (int i = 0; i < res.Length; i++) {
			res[i] = new Color32 (data[index], data[index + 1], data[index + 2], data[index + 3]);
			index+= 4;
		}
		return res;
	}

	Color32 GetColor32 () {
		Color32 res = new Color32(data[index], data[index + 1], data[index + 2], data[index + 3]);
		index+= 4;
		return res;
	}

	System.DateTime GetDateTime () {
		uint t = (uint)data[index++] | ((uint)data[index++] << 8) | ((uint)data[index++] << 16) | ((uint)data[index++] << 24);
		System.DateTime dateTime = new DateTime(1980, 1, 1, 0, 0, 0, 0).AddSeconds(t).ToLocalTime();
		return dateTime;
	}

	float GetFloat () {
		float res = BitConverter.ToSingle(data, index);
		index+= 4;
		return res;
	}

	int GetArrayCount ()
	{
		byte b = data[index++];
		if (b < 0x80)
			return b;
		if (b < 0xc0)
			return (b & 0x3f) << 8 | data[index++];
		int b1 = data[index++] << 8;
		return (b & 0x3f) << 16 | b1 | data[index++];
	}
}
