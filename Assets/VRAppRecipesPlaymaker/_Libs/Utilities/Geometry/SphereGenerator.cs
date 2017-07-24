using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SphereGenerator : MonoBehaviour {
	public int Latitude = 32;
	public int Longitude = 32;
	public float Radius = 1.0f;

	void Start () {
		GenerateSphere(Latitude, Longitude, Radius);
	}
	
	void GenerateSphere(int latNum, int longNum, float radius) {
		int vertexNum   = (longNum + 1) * (latNum + 2);
		int meshNum     = (longNum) * (latNum + 1);
		int triangleNum = meshNum * 2;

		Vector3[] vertices = new Vector3[vertexNum];
		float PI = Mathf.PI;
		float PI2 = PI * 2.0f;
		int latIdxMax = latNum + 1;     
		int longVertNum = longNum + 1;  
		float preComputeV = PI / latIdxMax;
		float preComputeH = PI2 / longNum;
		for (int i = 0; i <= latIdxMax; i++) {
			float thetaV = i * preComputeV;     
			float sinV = Mathf.Sin(thetaV);
			float cosV = Mathf.Cos(thetaV);
			int lineStartIdx = i * longVertNum;
			for (int j = 0; j <= longNum; j++) {
				float thetaH = j * preComputeH; 
				vertices[lineStartIdx + j] = new Vector3(
					Mathf.Cos(thetaH) * sinV,
					cosV,
					Mathf.Sin(thetaH) * sinV
				) * radius;
			}
		}

		//	Calculate normals
		Vector3[] normals = new Vector3[vertices.Length];
		for (int i = 0; i < vertices.Length; i++) {
			normals[i] = vertices[i].normalized;
			normals[i] *= -1.0f;
		}

		//	Calculate UVS
		Vector2[] uvs = new Vector2[vertices.Length];
		for (int i = 0; i <= latIdxMax; i++) {
			int lineStartIdx = i * longVertNum;
			float vVal = (float) i / latIdxMax;
			vVal = 1.0f - vVal;
			for (int j = 0; j <= longNum; j++) {
				float uVal = (float) j / longNum;
				uVal = 1.0f - uVal;
				uvs[lineStartIdx + j] = new Vector2(uVal, vVal);
			}
		}

		//	Calculate triangles
		int[] triangles = new int[triangleNum * 3];
		int index = 0;
		for (int i = 0; i <= latNum; i++) {
			for (int j = 0; j < longNum; j++) {
				int curVertIdx = i * longVertNum + j;
				int nextLineVertIdx = curVertIdx + longVertNum;

				triangles[index++] = curVertIdx;
				triangles[index++] = nextLineVertIdx + 1;
				triangles[index++] = curVertIdx + 1;
				triangles[index++] = curVertIdx;
				triangles[index++] = nextLineVertIdx;
				triangles[index++] = nextLineVertIdx + 1;
			}
		}

		//	Assign to mesh
		MeshFilter filter = gameObject.GetComponent<MeshFilter>();
		Mesh mesh = filter.mesh;
		mesh.Clear();
		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.uv = uvs;
		mesh.triangles = triangles;

		//MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
		//renderer.material.mainTexture = Texture2D.blackTexture;
		//print ("finished generating sphere");
	}
}