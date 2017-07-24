using UnityEngine;
using System.Collections;

public class CurvedDisplayController : MonoBehaviour {
	public Camera uiCamera;
	public Renderer targetDisplayRenderer;

	public Vector3 displayScale = new Vector3 (24.5f, 7f, 11f);

	private OVROverlayRenderer overlayRenderer;
	private Vector3 startPos;
	private Transform cylinderSpaceTransform;
	private CylinderGenerator generator;

	void Awake()
	{
		transform.localScale = displayScale;
		transform.localPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y, displayScale.z);

		if (targetDisplayRenderer!=null) {
			generator = targetDisplayRenderer.transform.GetComponent<CylinderGenerator> ();
			generator.cylinderWidthArc = displayScale.x;
			generator.cylinderHeight = displayScale.y;
			generator.radius = displayScale.z;
		}		
	}

	void Start () {
		overlayRenderer = GetComponent<OVROverlayRenderer>();

		Setup();

		var connector = uiCamera.gameObject.GetComponent<OVRRTOverlayConnector> ();
		if (connector!=null) connector.enabled = true;
	}

	void Setup() {
		float overlayWidth = displayScale.x;
		float overlayHeight = displayScale.y;
		float overlayRadius = displayScale.z;
		float singleEyeScreenPhysicalResX = Screen.width * 0.5f;
		float singleEyeScreenPhysicalResY = Screen.height;

		// Calculate RT Height     
		// screenSizeYInWorld : how much world unity the full screen can cover at overlayQuad's location vertically
		// pixelDensityY: pixels / world unit ( meter )
		float halfFovY = Camera.main.fieldOfView / 2;
		float screenSizeYInWorld = 2 * overlayRadius * Mathf.Tan(Mathf.Deg2Rad * halfFovY);
		float pixelDensityYPerWorldUnit = singleEyeScreenPhysicalResY / screenSizeYInWorld;
		float renderTargetHeight = pixelDensityYPerWorldUnit * overlayWidth;

		// Calculate RT width
		float renderTargetWidth = 0.0f;
			
		// For cylinder the resolution can be distributed uniformly along the angle.
		// So we use the angle coverage to calculate the required resolution
		float pixelDensityXPerRadian = singleEyeScreenPhysicalResX / (Camera.main.fieldOfView * Camera.main.aspect * Mathf.Deg2Rad);
		float pixelDensityXPerWorldUnit = pixelDensityXPerRadian / overlayRadius;
		renderTargetWidth = pixelDensityXPerWorldUnit * overlayWidth;
	
		Debug.Log("Screen Res: " + Screen.width + " x " + Screen.height + " RT Res: " + renderTargetWidth + " x " + renderTargetHeight);

		// Compute the orthographic size for the camera
		float orthographicSize = overlayHeight / 2.0f;
		float orthoCameraAspect = overlayWidth / overlayHeight;
		uiCamera.orthographicSize = orthographicSize;
		uiCamera.aspect = orthoCameraAspect;

		RenderTexture overlayRT = new RenderTexture(
			   (int)renderTargetWidth,
			   (int)renderTargetHeight,
			   24,
			   RenderTextureFormat.ARGB32,
			   RenderTextureReadWrite.sRGB);
		overlayRT.hideFlags = HideFlags.DontSave;
		uiCamera.GetComponent<Camera>().targetTexture = overlayRT;

		#if UNITY_5_5_OR_NEWER
		overlayRT.autoGenerateMips = true;
		#else
		overlayRT.generateMips = true;
		#endif

		targetDisplayRenderer.material.SetTexture("_MainTex", overlayRT);
	}
}

