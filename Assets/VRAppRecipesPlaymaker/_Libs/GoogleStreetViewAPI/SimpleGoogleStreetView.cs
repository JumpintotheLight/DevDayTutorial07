using UnityEngine;
using System.Collections;
using System.IO;

namespace ZefirVR {

	public class SimpleGoogleStreetView : MonoBehaviour 
	{
		public bool loadOnStart = false;
		public string m_location = "48.857507,2.294989";
		public string m_panoId = "";

		[Tooltip("Provide GOOGLE STREET VIEW API KEY OR LEAVE EMPTY")]
		public string API_KEY = "";

		public Material skybox;
		private SkyboxMesh skyboxmesh;

		void Awake()
		{
			skyboxmesh = GetComponent<SkyboxMesh> ();
		}

		void Start() 
		{
			if (loadOnStart) ShowPanorama(m_location, m_panoId);	
		}

		public void ShowPanorama(string location, string panoId) 
		{
			m_location = location;
			m_panoId = panoId;
			StartCoroutine(LoadImages());
		}

		IEnumerator LoadImages() 
		{
			Debug.Log ("Loading location " + m_location + "pano: " + m_panoId);
			object[][] directions = new object[][] {
				new object[] {  0,  0, "_FrontTex"},
				new object[] { 90,  0, "_LeftTex"},
				new object[] {180,  0, "_BackTex"},
				new object[] {270,  0, "_RightTex"},
				new object[] {  0, 90, "_UpTex"},
				new object[] {  0,-90, "_DownTex"}
			};

			foreach (object[] dir in directions) {
				ArrayList allParams = new ArrayList();
				allParams.AddRange(m_location.Split('&'));
				string locationParam = (string)allParams[0];
				string path = "Cache/" + locationParam.Replace(",", "_") + (string)dir[2];
				string cachePath = Application.persistentDataPath + "/" + locationParam.Replace(",", "_") + (string)dir[2] + ".png";

				// read from cache
				
				Texture2D tex;
				if (File.Exists(cachePath)) {
					Debug.Log("Reading from cache " + cachePath);
					// read from cache
					tex = new Texture2D(2, 2);
					tex.LoadImage(File.ReadAllBytes(cachePath));
				} else {
					// download
					string url = GetURL ((int)dir [0], (int)dir [1]);
					if (!string.IsNullOrEmpty (API_KEY)) url += "&key=" + API_KEY;
					print ("url: " + url);
					WWW www = new WWW(url);
					yield return www;
					tex = www.texture;

					var bytes = tex.EncodeToPNG();
					System.IO.File.WriteAllBytes(cachePath, bytes);
				}

				tex.wrapMode = TextureWrapMode.Clamp;
				skybox.SetTexture((string)dir[2], tex);
			}
			skyboxmesh.UpdateSkybox();
		}

		string GetURL(int heading, int pitch) {
			if (string.IsNullOrEmpty(m_panoId)) {
				return "http://maps.googleapis.com/maps/api/streetview?size=2048x2048&heading=" + heading + "&pitch=" + pitch + "&location=" + m_location + "&sensor=true";
			} else {
				return "http://maps.googleapis.com/maps/api/streetview?size=2048x2048&heading=" + heading + "&pitch=" + pitch + "&pano=" + m_panoId + "&sensor=false";
			}
		}
	}
}