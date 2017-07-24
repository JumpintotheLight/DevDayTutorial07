using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using HutongGames.PlayMaker;
using ZefirVR;

// Load and play srt file
public class SubtitlesPlayer : MonoBehaviour
{
	public PlayMakerFSM fsm;

	// subtitle file path
	public string subtitleFileUrl;
	public bool fromStreamingAssets;
	public bool loop = true;

	// settings
	public Text text;
	public Color textColor = Color.white;

	public ScrollingTextController scroller;
	public SimpleVideoPlayer videoPlayer;
	public bool updateTimerWithVideoPlayer = false;
	[HideInInspector] public bool loopOneLine = false;
	[HideInInspector] public int loopIndex = -1;

	// call when finished loading subtitles
	public Action onLoadedSubtitles;

	private bool isPlaying = false;
	private float timeToFade;
	private float movieTime = 0f;
	private List<SubtitleItem> subtitles;
	private int subsIndex = 0;
	private SubtitleItem subtitleItem;
	private string subtitlesLoadedEvent = "SubtitlesLoaded";

	private TextFileLoader textFileLoader;

	void Awake()
	{
		if (scroller!=null && !scroller.isActiveAndEnabled) scroller = null;
		textFileLoader = gameObject.AddComponent<TextFileLoader> ();
		if (text == null) {
			text = GetComponent<Text> ();
		}
		loopOneLine = false;
	}

	// Load subtitles from Playmaker
	public void LoadSubtitles(string subtitleFileName)
	{
		Load(subtitleFileName, () => {
			if (fsm!=null) fsm.SendEvent (subtitlesLoadedEvent);
			else PlayMakerFSM.BroadcastEvent (subtitlesLoadedEvent);
		});
	}

	// Load subtitles from a file in streaming assets or a folder
	public void Load(string url, Action OnComplete) 
	{
		Stop ();
		subtitles = null;

		if (string.IsNullOrEmpty (url.Trim ())) url = subtitleFileUrl;

		// form url
		if (fromStreamingAssets) url = Application.streamingAssetsPath + "/" + url;

		if (!url.StartsWith ("http") && !url.StartsWith ("file") && !url.StartsWith ("jar:")) url = "file://" + url; // add 'file' prefix for local file system
		Debug.Log ("Play video: " + url);

		// Load File
		textFileLoader.LoadTextFile (url, (result) => {
			// Parse subtitle file
			Stream stream = GenerateStreamFromString(result);
			if(stream != null) {
				SubtitlesParser parser = new SubtitlesParser();
				subtitles = parser.ParseStream(stream, Encoding.UTF8);
				SubIndex = 0;
			} else {
				Debug.Log("Failed to load subtitles from " + url);
			}
			print("Loaded subtitles from " + url);
			if (OnComplete!=null) OnComplete();
		});
	}

	private MemoryStream GenerateStreamFromString(string value) { return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));}

	public void Stop(){
		isPlaying = false;
		SubIndex = 0;
		movieTime = 0;
	}

	public void Play(){
		isPlaying = true;
		SubIndex = 0;
	}

	// used to trigger play and pause modes
	public void PlayPauseButton()
	{
		if (isPlaying) Pause();
		else UnPause ();
	}

	public void Pause(){
		isPlaying = false;
	}

	public void UnPause(){
		isPlaying = true;
		SubIndex = 0;
	}

	// Change movie time, used when movie time was changed by user
	public void SeekTo(float seekValue) {
		movieTime = seekValue;
	}

	// call this when there is a need to update from video seek position
	public void UpdateSeek()
	{
		if (videoPlayer!=null) SeekTo (videoPlayer.GetSeek ());
	}

	public void setTimestamp(float timestamp){
		movieTime = timestamp;
		if (subtitles != null) {
			DisplaySubtitle ();
		}
	}

	// Update subtitle display every frame
	void Update() {
		if (isPlaying && subtitles != null) {
			// sync with video player in case it will play with different speed
			if (updateTimerWithVideoPlayer) UpdateSeek ();

			movieTime += Time.deltaTime;
			DisplaySubtitle ();
		}
	}

	bool displayedCurrent = false;

	void SendEvent(string eventName)
	{
		if (fsm!=null) fsm.SendEvent (eventName);
		else PlayMakerFSM.BroadcastEvent (eventName);
	}

	void DisplaySubtitle() 
	{
		if (text!=null) text.color = textColor;
		if (subtitleItem == null) {
			subtitleItem = (SubtitleItem)subtitles [subsIndex];
		}
		while (movieTime > subtitleItem.endTime) {
			if (subsIndex < subtitles.Count - 1) {
				subsIndex++;
				subtitleItem = (SubtitleItem)subtitles [subsIndex];
				displayedCurrent = false;
			} else {
				// finished playing file
				if (loop) {
					// play again if looped
					movieTime = 0;
					displayedCurrent = false;
					Play ();
				}
			}
		}
		while (movieTime < subtitleItem.startTime && subsIndex > 0) {
			subsIndex--;
			subtitleItem = (SubtitleItem)subtitles [subsIndex];
			displayedCurrent = false;
		}
		if (movieTime >= subtitleItem.startTime && movieTime < subtitleItem.endTime) {
			if (!displayedCurrent) {
				if (loopOneLine && subsIndex>loopIndex) {
					// repeat previous line
					var videoSeek = PlayMakerGlobals.Instance.Variables.GetFsmFloat ("videoSeekValue");
					videoSeek.Value = subtitles[loopIndex].startTime;
					//print ("go back to subtitle: " + (subsIndex-1) + " at: " + subtitles[subsIndex-1].startTime);
					PlayMakerFSM.BroadcastEvent ("SeekTo");
				} else {
					// show next line
					//print ("next subtitle: " + subtitles [subsIndex].startTime);

					// add to scroller list
					if (scroller!=null) scroller.Add (subtitles[subsIndex], subsIndex);

					// show text
					if (text!=null) {
						string currentSubText = "";
						for (int i = 0; i < subtitleItem.Lines.Count; i++) {
							currentSubText += subtitleItem.Lines [i];
							if (i + 1 < subtitleItem.Lines.Count) {
								currentSubText += "\r\n";
							}
						}
						text.text = currentSubText;
					}
					SendEvent ("ShowSubtitleDisplay");
					displayedCurrent = true;
				}
			}
		} else {
			if (text!=null) text.text = "";
			SendEvent ("HideSubtitleDisplay");
		}
	}

	// call this to trigger one line loop mode
	public void LoopOneLine(bool loopLine)
	{
		if (loopLine) loopIndex = subsIndex;
		else loopIndex = -1;
		loopOneLine = loopLine;
	}
		
	public int SubIndex {
		get {
			return subsIndex;
		}
		set {
			subsIndex = value;
			if(subtitles != null && subsIndex < subtitles.Count) {
				subtitleItem = (SubtitleItem)subtitles[subsIndex];
			}
		}
	}
}