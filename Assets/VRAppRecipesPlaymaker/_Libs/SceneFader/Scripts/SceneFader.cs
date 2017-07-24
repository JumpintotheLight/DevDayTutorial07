using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour {

	public static SceneFader Instance;

	public bool fadeInOnStart = true;
	public float fadeTime = 2.0f;
	public Material faderMaterial;
	
	public Color fadeColor = new Color(0.01f, 0.01f, 0.01f, 1.0f);
	private bool isFading = false;
    public string nextSceneName = null;

	void Awake()
	{
		Instance = this;
		if (faderMaterial==null) Debug.LogError("fader material not found");
	}
	
	void OnEnable()
	{
		if (fadeInOnStart) StartCoroutine(FadeInRoutine());
	}
	
	void Start()
	{
		if (fadeInOnStart) StartCoroutine(FadeInRoutine());
	}
	
	public void FadeIn()
	{
		StartCoroutine(FadeInRoutine());
	}
	
	/// <summary>
	/// Fades alpha from 1.0 to 0.0
	/// </summary>
	IEnumerator FadeInRoutine()
	{
		float elapsedTime = 0.0f;
		faderMaterial.color = fadeColor;
		Color color = fadeColor;
		isFading = true;
		while (elapsedTime < fadeTime)
		{
			yield return null;
			elapsedTime += Time.deltaTime;
			color.a = 1.0f - Mathf.Clamp01(elapsedTime / fadeTime);
			faderMaterial.color = color;
		}
		isFading = false;
	}


	
	public void FadeOut()
	{
		StartCoroutine(FadeOutRoutine());
	}
	
	IEnumerator FadeOutRoutine()
	{
		float elapsedTime = 0.0f;
		faderMaterial.color = fadeColor;
		Color color = fadeColor;
		isFading = true;
		while (elapsedTime < fadeTime)
		{
			yield return null;
			elapsedTime += Time.deltaTime;
			color.a = Mathf.Clamp01(elapsedTime / fadeTime);
			faderMaterial.color = color;
		}
		isFading = false;
        if(!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadSceneAsync(nextSceneName);
	}	

} 