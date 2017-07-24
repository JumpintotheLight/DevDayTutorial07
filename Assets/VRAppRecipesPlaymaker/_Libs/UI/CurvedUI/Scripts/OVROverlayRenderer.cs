using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using VR = UnityEngine.VR;

public class OVROverlayRenderer : MonoBehaviour
{

	#if UNITY_ANDROID && !UNITY_EDITOR
	const int maxInstances = 3;
	#else
	const int maxInstances = 15;
	#endif

	internal static OVROverlayRenderer[] instances = new OVROverlayRenderer[maxInstances];

	public Texture[] textures = new Texture[] { null, null };
	private Texture[] cachedTextures = new Texture[] { null, null };
	private IntPtr[] texNativePtrs = new IntPtr[] { IntPtr.Zero, IntPtr.Zero };

	private int layerIndex = -1;
	Renderer rend;

	void Awake()
	{
		Debug.Log("Overlay Awake");
		rend = GetComponent<Renderer>();
		for (int i = 0; i < 2; ++i)
		{
			// Backward compatibility
			if (rend != null && textures[i] == null)
				textures[i] = rend.material.mainTexture;

			if (textures[i] != null)
			{
				cachedTextures[i] = textures[i];
				texNativePtrs[i] = textures[i].GetNativeTexturePtr();
			}
		}
	}

	public void OverrideOverlayTextureInfo(Texture srcTexture, IntPtr nativePtr, VR.VRNode node)
	{
		int index = (node == VR.VRNode.RightEye) ? 1 : 0;

		textures[index] = srcTexture;
		cachedTextures[index] = srcTexture;
		texNativePtrs[index] = nativePtr;
	}


	void OnEnable()
	{
		if (!OVRManager.isHmdPresent)
		{
			enabled = false;
			return;
		}

		OnDisable();

		for (int i = 0; i < maxInstances; ++i)
		{
			if (instances[i] == null || instances[i] == this)
			{
				layerIndex = i;
				instances[i] = this;
				break;
			}
		}
	}

	void OnDisable()
	{
		if (layerIndex != -1)
		{
			// Turn off the overlay if it was on.
			OVRPlugin.SetOverlayQuad(true, false, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, OVRPose.identity.ToPosef(), Vector3.one.ToVector3f(), layerIndex);
			instances[layerIndex] = null;
		}
		layerIndex = -1;
	}

	void OnRenderObject()
	{
		#if !UNITY_ANDROID || UNITY_EDITOR
		Debug.LogWarning("Overlay shape is not supported on current platform");
		#endif

		for (int i = 0; i < 2; ++i)
		{
			if (i >= textures.Length)
				continue;
			
			if (textures[i] != cachedTextures[i])
			{
				cachedTextures[i] = textures[i];
				if (cachedTextures[i] != null)
					texNativePtrs[i] = cachedTextures[i].GetNativeTexturePtr();
			}
		}

		if (cachedTextures[0] == null || texNativePtrs[0] == IntPtr.Zero)
			return;

		bool overlay = true;
		bool headLocked = false;
		for (var t = transform; t != null && !headLocked; t = t.parent)
			headLocked |= (t == Camera.current.transform);

		OVRPose pose = (headLocked) ? transform.ToHeadSpacePose() : transform.ToTrackingSpacePose();
		Vector3 scale = transform.lossyScale;
		for (int i = 0; i < 3; ++i)
			scale[i] /= Camera.current.transform.lossyScale[i];


		// Cylinder overlay sanity checking
		float arcAngle = scale.x / scale.z / (float)Math.PI * 180.0f;
		if (arcAngle > 180.0f)
		{
			Debug.LogError("Cylinder overlay's arc angle has to be below 180 degree, current arc angle is " + arcAngle + " degree." );
			return ;
		}

		bool isOverlayVisible = OVRPlugin.SetOverlayQuad(overlay, headLocked, texNativePtrs[0], texNativePtrs[1], IntPtr.Zero, pose.flipZ().ToPosef(), scale.ToVector3f(), layerIndex, OVRPlugin.OverlayShape.Cylinder);
		if (rend) rend.enabled = !isOverlayVisible;
	}

}
