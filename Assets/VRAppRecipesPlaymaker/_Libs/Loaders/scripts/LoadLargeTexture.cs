using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Download and assign Large (upto 8K) images from png and jpg files
public class LoadLargeTexture : MonoBehaviour {

	public string url = "http://eoimages.gsfc.nasa.gov/images/imagerecords/79000/79793/city_lights_africa_8k.jpg";
	public bool downloadOnStart = false;
	public bool loadFromStreamingAssets= false;

	[HideInInspector] public Texture2D m_myTexture;
	public Renderer[] targetRenderers;  // if not null, assign loaded texture to this renderer

	void Start()
	{
		//Application.runInBackground = true;
		if (downloadOnStart) StartCoroutine(downloadTexture());
	}

	public void Download(string imageUrl = null)
	{
		if (imageUrl!=null) url = imageUrl;
		StartCoroutine(downloadTexture());
	}

	IEnumerator downloadTexture()
	{
		if (loadFromStreamingAssets) {
			url = Application.streamingAssetsPath + "/" + url;
		}

		// add prefeix if this is a local file 
		if (!url.StartsWith ("http") && !url.StartsWith ("file")) url = "file://" + url;

		print ("Download image from: " + url);
		WWW www = new WWW(url);
		yield return www;

		Debug.Log("Downloaded Texture. Now copying it");

		StartCoroutine(copyTextureAsync(www.texture, false, finishedCopying));
	}

	float maxWidth = 4096f;
	float maxHeight = 4096f;

	IEnumerator copyTextureAsync(Texture2D source, bool useMipMap = false, System.Action callBack = null)
	{
		const int LOOP_TO_WAIT = 400000; //Waits every 400,000 loop, Reduce this if still freezing
		int loopCounter = 0;

		int heightSize = source.height;
		int widthSize = source.width;

		print ("Finished donwnloading image. format: " + source.format + "width: " + widthSize + " height: " + heightSize);

		if (widthSize>4096 || heightSize>4096) {
			if (widthSize > heightSize) {
				heightSize = Mathf.CeilToInt((float)heightSize * maxWidth / (float)widthSize);
				widthSize = (int)maxWidth;
			} else {
				widthSize = Mathf.CeilToInt((float)widthSize * maxHeight / (float)heightSize);
				heightSize = (int)maxHeight;
			}
			print ("large texture, resizing it to: " + widthSize + " / " + heightSize);
			#if UNITY_ANDROID || UNITY_IOS
			TextureScale.Bilinear (source, widthSize, heightSize);
			#else
			TextureScaler.scale(source, widthSize, heightSize);
			#endif
		}

		m_myTexture = new Texture2D(widthSize, heightSize, source.format, useMipMap);

		for (int y = 0; y < heightSize; y++)
		{
			for (int x = 0; x < widthSize; x++)
			{
				//Get color/pixel at x,y pixel from source Texture
				Color tempSourceColor = source.GetPixel(x, y);

				//Set color/pixel at x,y pixel to destintaion Texture
				m_myTexture.SetPixel(x, y, tempSourceColor);

				loopCounter++;

				if (loopCounter % LOOP_TO_WAIT == 0)
				{
					//Debug.Log("Copying");
					yield return null; //Wait after every LOOP_TO_WAIT 
				}
			}
		}
		//Apply changes to the Texture
		m_myTexture.Apply();

		if (targetRenderers != null && targetRenderers.Length > 0) {
			for(int i=0;i<targetRenderers.Length;i++) targetRenderers[i].material.mainTexture = m_myTexture;
		}

		//Let our optional callback function know that we've done copying Texture
		if (callBack != null)
		{
			callBack.Invoke();
		}
	}

	void finishedCopying()
	{
		Debug.Log("Finished Copying Texture");
		//Do something else
	}
}
