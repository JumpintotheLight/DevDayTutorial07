using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System;
using UnityEngine.Events;

// Download and cache file and save it to local data folder
public class Downloader : MonoBehaviour {

	public string fileUrl;
    public string filePath;
	public GameObject loading;
	public GameObject errorLoading;
	public Slider progressSlider = null;

    private Action<string> OnDownloadedCallback;

	void Start()
	{
		ShowLoading (false);
		ShowErrorLoading (false);
		if(progressSlider != null) progressSlider.gameObject.SetActive (false);
	}

	public void DownloadAndShowImage(string imageUrl, Image image)
	{
		fileUrl = imageUrl;
		Download ((filePath) => {
			StartCoroutine (ShowLocalImage (filePath,image));
		});
	}

	public void ShowImage(string path, Image image)
	{
		Debug.Log ("show image: " + path);
		StartCoroutine (ShowLocalImage (path,image));
	}

	public void ShowRawImage(string path, RawImage image){
		StartCoroutine (ShowLocalRawImage (path,image));
	}

	public void ShowTexture(string path, Material targetTexture){
		StartCoroutine (ShowLocalTexture (path,targetTexture));
	}

	IEnumerator ShowLocalImage(string path, Image image)
	{
		// create sprite from image file
		WWW www = new WWW ( "file:///" + path);
		yield return www;

		if (string.IsNullOrEmpty(www.error)) {
			// assign sprite
			Texture2D texture = www.texture;
			Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), texture.width);
			image.sprite = newSprite;
		};
	}

	IEnumerator ShowLocalRawImage(string path, RawImage image)
	{
		// create sprite from image file
		WWW www = new WWW ( "file:///" + path);
		yield return www;

		if (string.IsNullOrEmpty(www.error)) {
			//www.LoadImageIntoTexture (image.texture);
			image.texture = www.texture;
			image.SizeToParent (0);
		};
	}

	IEnumerator ShowLocalTexture(string path, Material targetTexture)
	{
		// create sprite from image file
		WWW www = new WWW ( "file:///" + path);
		yield return www;

		if (string.IsNullOrEmpty(www.error)) {
			Texture2D texture = new Texture2D (16, 16, TextureFormat.RGB24, true);
			www.LoadImageIntoTexture (texture);
			texture.Apply ();
			targetTexture.mainTexture = texture;
		};
	}


	public void Download(Action<string> callBack)
    {
        OnDownloadedCallback = callBack;
		ShowLoading (false);
		ShowErrorLoading (false);

		// Check cached files and download if not available yet
        //string saveToFile = System.IO.Path.GetFileName(new Uri(fileUrl).LocalPath);
		string saveToFile = fileUrl;

        // remove all special character so that we can save downloaded file into local storage
		saveToFile = saveToFile.Replace("://", "_");
		saveToFile = saveToFile.Replace (".", "_");
		saveToFile = saveToFile.Replace("/", "_");
		saveToFile = saveToFile.Replace("?", "_");
		saveToFile = saveToFile.Replace(":", "_");
        filePath = Path.Combine(Application.persistentDataPath, saveToFile);

        if (File.Exists(this.filePath))
        {
            //Debug.Log("File " + filePath + " already exists");
			if (this.isActiveAndEnabled) {
                // invoke callback function
				OnDownloadedCallback(filePath);
			}
            return;
        }

        // Start downloading coroutine
		StartCoroutine ("StartDownload");
    }

    // Call this if you want to restart download
	public void RetryDownload()
	{
		Debug.Log("Restart downloading");
		StartCoroutine ("StartDownload");
	}

    public IEnumerator StartDownload()
    {

        // Show loading indicator
		ShowLoading (true);
        
        // Show progress bar
		if (progressSlider != null) progressSlider.gameObject.SetActive (true);

        // Start downloading
		WWW www = new WWW(fileUrl);

        // Keep control until file is downloaded or error happens
		while (!www.isDone)
		{
            // processing the download - update progress bar
			PartDownloaded(www.progress);
            
            // check for errors
			if (www.error != null) DownloadError(www.error);

            // let the app do other stuff, skip the frame
			yield return null;
		}

        // Hide progress bar
		if(progressSlider != null)  progressSlider.gameObject.SetActive(false);

        // Check the download result
        if (www.error!=null) DownloadError(www.error);
		else {
			// Save file
			Debug.Log ("save image into file: " + fileUrl + " path: " + filePath);
			File.WriteAllBytes(filePath, www.bytes);			

            // Hide loading panel
			ShowLoading (false);

            // Call callback function to finish after we have downloader the file
			OnDownloadedCallback(filePath);
			yield return null;
		}
    }

	// Update progress slider
	void PartDownloaded(float progress)
	{
		if (progressSlider!=null) progressSlider.value = progress;
	}

    // If you need to force download to stop, call this
    public void StopDownload()
    {
        // Stop the coroutine
        StopCoroutine("StartDownload");

        // Remove incomplete file
        if (File.Exists(this.filePath)) File.Delete(this.filePath);
		ShowLoading (false);
    }

    // If an error happened, call this
    void DownloadError(string message)
    {
		Debug.Log ("Download Error: " + message);

        // stop downloading, in case it's stil working
        StopCoroutine("StartDownload");

        // delete incomplete file
        if (File.Exists(this.filePath)) File.Delete(this.filePath);

        // Show error panel and retry button
		ShowErrorLoading(true);

        // Hide progress bar
		if(progressSlider != null)	progressSlider.gameObject.SetActive (false);
    }

    // Show/Hide Error loading panel
	void ShowErrorLoading(bool show)
	{
		if (errorLoading!=null) errorLoading.SetActive(show);
	}

    // Show/Hide Loading panel and indicator
    void ShowLoading(bool show)
	{
		if (loading!=null) loading.SetActive(show);
	}

	// Stop all loaders
	void OnDestroy() {
		StopAllCoroutines();
	}

	void OnDisable() {
		StopAllCoroutines ();
	}
}
