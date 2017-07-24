using UnityEngine;
using System;
using System.Collections;

public class CBOR
{
	byte[] data;
	byte[] tmpBuf;
	string[] stringRef;
	int stringRefIndex;
	int index;
	bool stringReferences;

	public static byte[] Encode(object o, bool stringReferences=true)
	{
		return new CBOR().EncodeValue(o, stringReferences);
	}

	public static object Decode(byte[] data)
	{
		if (data == null || data.Length < 1)
			return null;
		return new CBOR().DecodeValue(data);
	}

	byte[] EncodeValue(object o, bool stringReferences)
	{
		this.stringReferences = stringReferences;
		data = new byte[6144];
		index = 0;
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
			data[index++] = 0xf6;
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
			data[index++] = (byte)((bool)o ? 0xf5 : 0xf4);
		} else if (t == typeof(float))
			Add ((float)o);
		else if (t == typeof(double))
			Add ((float)(double)o);
		else if (t == typeof(Hashtable))
			Add ((Hashtable)o);
		else if (t == typeof(ArrayList))
			Add ((ArrayList)o);
		else
			Add ((object)null);
	}

	void Add (float v)
	{
		if (index + 5 >= data.Length)
			IncreaseDataSize (5);
		data[index++] = 0xfa;
		byte[] b = System.BitConverter.GetBytes(v);
		if (System.BitConverter.IsLittleEndian)
			System.Array.Reverse(b);
		System.Array.Copy (b, 0, data, index, b.Length);
		index+= b.Length;
	}

	void Add (double v)
	{
		if (index + 9 >= data.Length)
			IncreaseDataSize (9);
		data[index++] = 0xfb;
		byte[] b = System.BitConverter.GetBytes(v);
		if (System.BitConverter.IsLittleEndian)
			System.Array.Reverse(b);
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
					if (index + 2 >= data.Length)
						IncreaseDataSize (2);
					int backref = (stringRefIndex - i + 256) % 256;
					if (backref < 20)
						data[index++] = (byte)(0xe0 + backref);
					else
					{
						data[index++] = (byte)(0xe0 + 24);
						data[index++] = (byte)backref;
					}
					return;
				}
			}
		}

		byte[] b = System.Text.Encoding.UTF8.GetBytes (v);
		if (index + b.Length + 9 >= data.Length)
			IncreaseDataSize (b.Length + 9);
		bool unicode = b.Length != v.Length;
		Add ((byte)(unicode ? 0x40 : 0x60), b.Length);
		System.Array.Copy (b, 0, data, index, b.Length);
		index+= b.Length;

		if (!unicode && stringReferences && v.Length > 1 && v.Length < 64) {
			stringRef[stringRefIndex] = v;
			stringRefIndex = (stringRefIndex + 1) % 256;
		}
	}

	void Add (ArrayList v)
	{
		if (data.Length <= index + 9)
			IncreaseDataSize (9);
		Add (0x80, v.Count);
		for (int i = 0; i < v.Count; i++)
			Add (v[i]);
	}
	
	void Add(Hashtable v) {
		if (data.Length <= index + 9)
			IncreaseDataSize (9);
		Add (0xa0, v.Count);
		foreach (DictionaryEntry e in v)
		{
			if (e.Key is string)
				Add((string)e.Key);
			else
				Add(e.Key);
			Add(e.Value);
		}
	}

	void Add (byte b, long r)
	{
		if (r < 24)
			data[index++] = (byte)(b | r);
		else {
			if (r <= 0xff)
				data[index++] = (byte)(b | 24);
			else {
				if (r <= 0xffff)
					data[index++] = (byte)(b | 25);
				else {
					if (r <= 0xffffffff)
						data[index++] = (byte)(b | 26);
					else
					{
						data[index++] = (byte)(b | 27);
						data[index++] = (byte)((r >> 56) & 0xff);
						data[index++] = (byte)((r >> 48) & 0xff);
						data[index++] = (byte)((r >> 40) & 0xff);
						data[index++] = (byte)((r >> 32) & 0xff);
					}
					data[index++] = (byte)((r >> 24) & 0xff);
					data[index++] = (byte)((r >> 16) & 0xff);
				}
				data[index++] = (byte)((r >> 8) & 0xff);
			}
			data[index++] = (byte)(r & 0xff);
		}
	}

	void IncreaseDataSize(int requiredBytes) {
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
		tmpBuf = new byte[8];
		stringRef = new string[256];
		stringRefIndex = 0;
		index = 0;
		return Get ();
	}

	object Get()
	{
		//if (index >= data.Length)
		//	return null;

		byte b = data[index++];
		int type = b & 0xe0, mask = b & 0x1f;
		//Debug.Log ("GOT " + (b >> 5) + " " + mask + "    at " + index);

		if (type == 0xe0)
		{
			if (mask < 20 || mask == 24)
			{
				if (mask == 24 && index >= data.Length)
					return null;
				int backref = mask < 20 ? mask : data[index++];
				return stringRef[(stringRefIndex - backref + 256) % 256];
			}

			switch (mask)
			{
			case 20: return false;
			case 21: return true;
			case 22: return null;
			case 23: return null;
			case 25: // float16
			{
				int half = (data[index] << 8) | data[index + 1];
				index+= 2;

				int exp = (half >> 10) & 0x1f;
				int mant = half & 0x3ff;

				float val;
				if (exp == 0) val = mant * Mathf.Pow(2, -24); // ldexp(mant, -24);
				else if (exp != 31) val = (mant + 1024) * Mathf.Pow(2, exp - 25); // ldexp(mant + 1024, exp - 25);
				else val = mant == 0 ? float.PositiveInfinity : float.NaN;
				return (half & 0x8000) != 0 ? -val : val;
			}
			case 26: // float32
			{
				float res;
				if (System.BitConverter.IsLittleEndian) {
					for (int i = 0; i < 4; i++)
						tmpBuf[3 - i] = data[index++];
					res = System.BitConverter.ToSingle(tmpBuf, 0);
				} else {
					res = System.BitConverter.ToSingle(data, index);
					index+= 4;
				}
				return res;
			}
			case 27: // float64
			{
				double res;
				if (System.BitConverter.IsLittleEndian) {
					for (int i = 0; i < 8; i++)
						tmpBuf[7 - i] = data[index++];
					res = System.BitConverter.ToDouble(tmpBuf, 0);
				} else {
					res = System.BitConverter.ToDouble(data, index);
					index+= 8;
				}
				return res;
			}
			}
			return null;
		}

		int r = -1;
		if (mask < 24)
			r = mask;
		else if (mask < 28)
		{
			// 24 - uint8
			r = data[index++];
			if (mask >= 25)// && index < data.Length)
			{
				// 25 - uint16
				r = (r << 8) | data[index++];
				if (mask >= 26)// && index + 1 < data.Length)
				{
					// 26 - uint32
					r = (r << 8) | data[index++];
					r = (r << 8) | data[index++];
					if (mask >= 27)// && index + 3 < data.Length)
					{
						// 27 - uint64
						r = (r << 8) | data[index++];
						r = (r << 8) | data[index++];
						r = (r << 8) | data[index++];
						r = (r << 8) | data[index++];
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
			bool isIndefinite = mask == 31;
			if (isIndefinite) {
				int i = index;
				while (i < data.Length && data[i] != 0xff)
					i++;
				r = i - index;
			}

			string s = System.Text.Encoding.ASCII.GetString(data, index, r);
			index+= r;

			if (s.Length > 1 && s.Length < 64) {
				stringRef[stringRefIndex] = s;
				stringRefIndex = (stringRefIndex + 1) % 256;
			}

			if (isIndefinite && index < data.Length && data[index] == 0xff)
				index++;
			return s;
		}
		case 0x60: // text
		{
			bool isIndefinite = mask == 31;
			if (isIndefinite) {
				int i = index;
				while (i < data.Length && data[i] != 0xff)
					i++;
				r = i - index;
			}

			string s = System.Text.Encoding.UTF8.GetString(data, index, r);
			index+= r;

			if (isIndefinite && index < data.Length && data[index] == 0xff)
				index++;
			return s;
		}
		case 0x80: // array
		{
			bool isIndefinite = mask == 31;
			ArrayList a = new ArrayList(r);
			int i = 0;
			while (index < data.Length && (isIndefinite ? data[index] == 0xff : i < r)) {
				a.Add(Get());
				i++;
			}
			if (isIndefinite && index < data.Length && data[index] == 0xff)
				index++;
			return a;
		}
		case 0xa0: // map
		{
			bool isIndefinite = mask == 31;
			Hashtable h = new Hashtable(r);
			int i = 0;
			while (index < data.Length && (isIndefinite ? data[index] == 0xff : i < r)) {
				string k = Get() as string;
				object v = Get();
				h[k] = v;
				i++;
			}
			if (isIndefinite && index < data.Length && data[index] == 0xff)
				index++;
			return h;
		}
		case 0xc0: // tags
			break;
		}

		return null;
	}
}
