using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using HutongGames.PlayMaker;

public class TextTyper : MonoBehaviour
{	
	public bool playOnStart = true;

	public Text targetTextElement;
	public bool typeWords = false;
	public string skipCharacters = "\n "; 
	private string wordSpacingCharacters = "\n ";

	public AudioSource typingSound;
	 
	public float typingInterval = 0.05f;
	public float typingIntervalVariation = 0.02f;

	bool isPlaying;
	string textContent;
	int contentLength = 0;
	int currentIndex = 0;
	
	float IntervalTimer = 0;
	
	bool SoundHasBeenMadeThisFrame = false;
	Action OnComplete = null;
	
	void Start ()
	{
		isPlaying = false;
		if (playOnStart) Play(targetTextElement.text);
		else targetTextElement.enabled = false;
	}

	// Action to be used in Playmaker
	public void PlayPM(string textToPlay, PlayMakerFSM targetFsm, string onCompleteEvent)
	{
		Play(textToPlay, ()=> {
			if (targetFsm!=null) targetFsm.SendEvent (onCompleteEvent);
			else PlayMakerFSM.BroadcastEvent (onCompleteEvent);
		});
	}

	public void Play(string textToPlay, Action whenCompleted = null)
	{
		OnComplete = whenCompleted;

		// if text is empty use data from targetText
		if (!string.IsNullOrEmpty (textToPlay.Trim ())) textContent = textToPlay;
		else textContent = targetTextElement.text;

		contentLength = textContent.Length;
		targetTextElement.text = "";
		targetTextElement.enabled = true;
		isPlaying = true;
	}

	public void Play(bool playTyper)
	{
		isPlaying = playTyper;
	}
	
	void Update () 
	{
		if (isPlaying) TyperUpdate();
	}
	
	void TyperUpdate()
	{
		if(currentIndex < contentLength) {
			IntervalTimer = UpdateTimer(IntervalTimer);
			SoundHasBeenMadeThisFrame = false;
			
			while(IntervalTimer > typingInterval) {
				IntervalTimer -= typingInterval;
				
				if(typeWords) typeOneWord();
				else typeOneCharacter();
			}			
		} else {
			isPlaying = false;
			if (OnComplete!=null) OnComplete();
		}
	}
	
	void typeOneWord()
	{		
		do  {
			if (currentIndex < contentLength) {				
				PlaySoundEffect();			
				targetTextElement.text += textContent[currentIndex];
				currentIndex++;
			}
		} while (currentIndex < contentLength && !ContainsCharacter(wordSpacingCharacters, textContent[currentIndex]));
	}
	
	void typeOneCharacter()
	{
		if(currentIndex < contentLength) {					
			PlaySoundEffect();
			
			targetTextElement.text += textContent[currentIndex];
			currentIndex++;
		}
	}
	
	void PlaySoundEffect()
	{
		if(!SoundHasBeenMadeThisFrame) {
			if(!ContainsCharacter(skipCharacters, textContent[currentIndex])) {
				if(typingSound != null) {
					// play typing sound effect
					SoundHasBeenMadeThisFrame = true;
					typingSound.PlayOneShot(typingSound.clip);
				}
			}
		}
	}
	
	float UpdateTimer(float Timer)
	{
		if (Time.timeScale != 1 ) {
			return Timer + (Time.deltaTime/Time.timeScale);
		} else {
			return Timer + Time.deltaTime;
		}
	}
	
	// returns true if the string contains a character
	bool ContainsCharacter(string IsIn, char LookFor)
	{
		for(int i = 0; i < IsIn.Length; i++) {
			if(LookFor == IsIn[i]) {
				return true;
			}			
		}
		return false;
	}
}