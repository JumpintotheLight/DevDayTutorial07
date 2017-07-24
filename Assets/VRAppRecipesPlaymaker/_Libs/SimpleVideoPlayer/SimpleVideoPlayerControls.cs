using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace ZefirVR {
	public class SimpleVideoPlayerControls : MonoBehaviour {

		public SimpleVideoPlayer player;

		public Text progressLabel;
		public Slider progressSlider;
		public Slider seekSlider;

		public GameObject replayButton;
		public Image playPauseButtonImage;
		public Sprite playButtonSprite;
		public Sprite pauseButtonSprite;

		public Text speedLabel;

		int playbackSpeedIndex = 0;
		float[] playbackValues = new float[] {1.0f, 1.25f, 1.5f, 2.0f};
	

		void Awake()
		{
			if (player==null) player = GetComponent<SimpleVideoPlayer> ();
			if (replayButton!=null) {
				player.videoPlayer.loopPointReached += LoopReached;
				replayButton.SetActive (false);
			}
		}

		// Catch moment when video is finished and offer replay
		void LoopReached(VideoPlayer source)
		{
			print ("loop reached");
			replayButton.SetActive (true);
			player.videoPlayer.Pause ();
		}

		// Replay video
		public void ReplayVideo()
		{
			print ("replay");
			replayButton.SetActive (false);
			player.videoPlayer.Play ();
		}

		// Update label and progress slider
		void Update()
		{
			if (player.videoPlayer.isPlaying) {
				float totalTime = (float)(player.videoPlayer.frameCount / player.videoPlayer.frameRate);
				float currentTime = (float)(player.videoPlayer.frame / player.videoPlayer.frameRate);
				progressLabel.text = Mathf.Floor(currentTime/60.0f).ToString("00") + ":" + Mathf.Floor(currentTime % 60).ToString("00") + " / " +
					Mathf.Floor(totalTime/60.0f).ToString("00") + ":" + Mathf.Floor(totalTime% 60).ToString("00");
				progressSlider.value = Mathf.Clamp01 (currentTime / totalTime);
			}
		}

		// Call this to seek to seekSlider value
		public void SeekTo()
		{
			player.videoPlayer.frame = (long)((float)player.videoPlayer.frameCount * seekSlider.value);
			progressSlider.value = seekSlider.value;
		}

		// Call this to trigger Play/Pause behavior
		public void PlayPauseButtonClick()
		{
			bool playing = player.videoPlayer.isPlaying;
			if (playing) {
				player.videoPlayer.Pause ();
			} else {
				player.videoPlayer.Play ();
			}
			UpdatePlayPauseButton ();
		}

		void UpdatePlayPauseButton()
		{
			bool playing = player.videoPlayer.isPlaying;
			if (playing) {
				playPauseButtonImage.sprite = pauseButtonSprite;
			} else {
				playPauseButtonImage.sprite = playButtonSprite;
			}
		}

		public void ChangeSpeed()
		{
			playbackSpeedIndex++;
			if (playbackSpeedIndex>=playbackValues.Length) playbackSpeedIndex = 0;
			speedLabel.text = "x" + playbackValues [playbackSpeedIndex].ToString ();
			player.videoPlayer.playbackSpeed = playbackValues [playbackSpeedIndex];
		}

		public void Mute()
		{
			player.audioSource.mute = true;
		}

		public void UnMute()
		{
			player.audioSource.mute = false;
		}

	}
}
