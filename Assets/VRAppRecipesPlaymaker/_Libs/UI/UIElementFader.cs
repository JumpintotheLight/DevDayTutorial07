using UnityEngine;
using UnityEngine.UI;

// Show, Hide and Dim UI Element with fade 
public class UIElementFader : MonoBehaviour
{
	[Tooltip("Start fading on awake")]
	public bool fadeInOnAwake = false;

	[Tooltip("How long to fade element")]
	public float timeToFade = 1.0f;

	[Tooltip("Hide and destroy in seconds, 0 to never hide")]
	public float dimTime = 0.0f;

	[Tooltip("Dimmed element alpha")]
	public float dimAlpha = 0.5f;

	[Tooltip("Hide and destroy in seconds, 0 to never hide")]
	public float hideTime = 0.0f;

	private float timeElapsed = 0.0f;

	private Graphic target = null;
	private bool isFadingIn = false;
	private bool isFadingOut = false;
	private bool isDimming = false;
	private float fadeInStart = 0.0f;
	private float fadeOutStart = 0.0f;
	private float dimmingStart = 0.0f;

	private float fadeOutStartAlpha = 1.0f;
	private bool isHidden = false;

	private Color initialColor = Color.white;
	private float awakeTimer = 0.0f;

	void Awake()
	{
		target = GetComponent <Graphic> ();
	}

	private void Start()
	{
		timeElapsed = 0.0f;

		if (fadeInOnAwake && target!=null) {
			target.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0.0f);
		}
	}

	private void Update()
	{
		if (!isHidden) timeElapsed += Time.deltaTime;

		if (fadeInOnAwake) {
			awakeTimer += (Time.deltaTime / timeToFade);
			target.color = new Color(initialColor.r, initialColor.g, initialColor.b, Mathf.Clamp01(awakeTimer));
			if (awakeTimer > timeToFade) fadeInOnAwake = false;
		}

		// Dim when it's time to do that
		if (!isDimming && timeElapsed>=dimTime && dimTime>0f) Dim();

		// Start fading out when it's time to hide
		if (!isFadingOut && timeElapsed>=hideTime && hideTime>0f) FadeOut ();

		if (isFadingIn) {
			float fElapsed = Time.time - fadeInStart;
			if (fElapsed < timeToFade && target != null)
			{
				Color c = target.color;
				c.a = 1.0f - fElapsed / timeToFade;
				target.color = c;
			}
		}

		if (isFadingOut) {
			float fElapsed = Time.time - fadeOutStart;
			if (fElapsed < timeToFade && target != null)
			{
				Color c = target.color;
				c.a = fadeOutStartAlpha - Mathf.Lerp (0f, fadeOutStartAlpha, fElapsed / timeToFade);
				target.color = c;
			} else {
				isFadingOut = false;
				hideTime = 0f;
			}
		}

		if (isDimming) {
			float fElapsed = Time.time - dimmingStart;
			if (fElapsed < timeToFade && target != null)
			{
				Color c = target.color;
				c.a = Mathf.Lerp (1.0f, dimAlpha, fElapsed / timeToFade);
				target.color = c;
			} else {
				isDimming = false;
				dimTime = 0f;
			}
		}
	}

	// Fade in UI element
	public void FadeIn()
	{
		isFadingIn = true;
		isFadingOut = false;
		isDimming = false;
		fadeInStart = Time.time;
	}

	// Fade out UI element
	public void FadeOut()
	{
		isFadingOut = true;
		isFadingIn = false;
		isDimming = false;
		fadeOutStartAlpha = target.color.a;
		fadeOutStart = Time.time;
	}

	// Dim UI element
	public void Dim()
	{
		isDimming = true;
		isFadingOut = false;
		isFadingIn = false;
		dimmingStart = Time.time;
	}
}