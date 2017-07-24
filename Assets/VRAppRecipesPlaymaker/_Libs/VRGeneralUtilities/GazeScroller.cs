using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GazeScroller : MonoBehaviour 
{
	public bool autoScroll = false;
    public GazeEventTrigger upTrigger;
    public GazeEventTrigger downTrigger;
    public ScrollRect scrollRect;
    float scrollDirection;
    float scrollRange;
    float scrollPower = 60;
    private bool scrollEnabled = true;

    void Start()
    {
        if (scrollRect==null) scrollRect = GetComponent<ScrollRect>();
		if (scrollRect==null) Debug.LogError ("Scroll Rect not found");

        upTrigger.onEnter.AddListener(StartScrollUp);
        upTrigger.onExit.AddListener(StopScroll);
        downTrigger.onEnter.AddListener(StartScrollDown);
        downTrigger.onExit.AddListener(StopScroll);
        RefreshContentSize();

		if (autoScroll) {
			scrollEnabled = true;
			StartScrollDown ();
		}
    }
		
    public void StartScrollUp()
    {
        scrollDirection = 1;
    }

    public void StopScroll()
    {
        if (!autoScroll) scrollDirection = 0;
    }

    public void StartScrollDown()
    {
        scrollDirection = -1;
    }

    public void SetEnabled(bool enabled)
    {
        scrollEnabled = enabled;
    }

    void OnEnable()
    {
        if (!autoScroll) scrollDirection = 0;
    }

    void RefreshContentSize()
    {
        float scrollRectHeight = GetComponent<RectTransform>().rect.height;
        float contentRectHeight = scrollRect.content.GetComponent<RectTransform>().rect.height;
        if (contentRectHeight != 0)
        {
            scrollRange = contentRectHeight - scrollRectHeight;
        }
    }

    public void GotoTop()
    {
        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 1;
    }

	public void GotoBottom()
	{
		if (scrollRect != null)
			scrollRect.verticalNormalizedPosition = 0;
	}

	void Update () {
        RefreshContentSize();
        bool canGoUp = scrollRect.verticalNormalizedPosition*scrollRange < (scrollRange -0.01f);
        bool canGoDown = scrollRect.verticalNormalizedPosition*scrollRange > 0.01f; 
        
		if ((scrollDirection > 0 &&  canGoUp) || (scrollDirection < 0 && canGoDown)) {
            scrollRect.verticalNormalizedPosition = scrollRect.verticalNormalizedPosition + Time.deltaTime * scrollDirection * scrollPower * scrollRect.scrollSensitivity / scrollRange;
        }
        
		if (!autoScroll) {
			if (!scrollEnabled) {
				canGoDown = canGoUp = false;
			}
			upTrigger.gameObject.SetActive(canGoUp);
			downTrigger.gameObject.SetActive(canGoDown);

			if (scrollDirection > 0 && !canGoUp) {
				scrollDirection = 0;
			}

			if (scrollDirection < 0 && !canGoDown) {
				scrollDirection = 0;
			}
		}
	}
}
