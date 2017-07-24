using UnityEngine;
using System.Collections;

public class CylinderGenerator : MonoBehaviour {

	public int widthTess = 300;
	public int heightTess = 2;
	public float cylinderWidthArc = 24.5f;
	public float cylinderHeight = 5f;
	public float radius = 11f;

	private Vector3[] Vertices;
	private Vector2[] UV;
	private int[] Triangles;

	private MeshCollider collider;
	private MeshFilter mFilter;

	void Awake()
	{
		collider = GetComponent <MeshCollider> ();
		mFilter = GetComponent<MeshFilter> ();
	}

	void Start () {
		GenerateCylinder();
		if (mFilter!=null) {
			mFilter.mesh.vertices = Vertices;
			mFilter.mesh.uv = UV;
			mFilter.mesh.triangles = Triangles;
		}

		if (collider!=null) {
			collider.sharedMesh = new Mesh ();
			collider.sharedMesh.vertices = Vertices;
			collider.sharedMesh.uv = UV;
			collider.sharedMesh.triangles = Triangles;

			// little hack to update the collider
			collider.enabled = false;
			collider.enabled = true;
		}
	}

	void GenerateCylinder() {
		int vertexCount = widthTess * heightTess;
		int indexCount = (widthTess - 1) * (heightTess - 1) * 6;

		Vertices = new Vector3[vertexCount];
		UV = new Vector2[vertexCount];
		Triangles = new int[indexCount];

		float arcAngle = cylinderWidthArc / radius;
		for (int j = 0; j < heightTess; j++) {
			for (int i = 0; i < widthTess; i++) {
				float currentAngle = -arcAngle / 2.0f + arcAngle * i / (float)(widthTess - 1);
				Vertices[j * widthTess + i].x = Mathf.Sin(currentAngle) * radius;
				Vertices[j * widthTess + i].y = 0.5f * cylinderHeight - j / (float)(heightTess - 1) * cylinderHeight;
				Vertices[j * widthTess + i].z = Mathf.Cos(currentAngle) * radius;
				UV[j * widthTess + i].x = (float)i / (float)(widthTess - 1);
				UV[j * widthTess + i].y = 1 - (float)j / (float)(heightTess - 1);
			}
		}

		for (int j = 0; j < heightTess - 1; j++) {
			for (int i = 0; i < widthTess - 1; i++) {
				Triangles[j * (widthTess - 1) * 6 + i * 6 + 0] = j * widthTess + i;
				Triangles[j * (widthTess - 1) * 6 + i * 6 + 1] = j * widthTess + i + 1;
				Triangles[j * (widthTess - 1) * 6 + i * 6 + 2] = (j + 1) * widthTess + (i + 1);
				Triangles[j * (widthTess - 1) * 6 + i * 6 + 3] = j * widthTess + i;
				Triangles[j * (widthTess - 1) * 6 + i * 6 + 4] = (j + 1) * widthTess + (i + 1);
				Triangles[j * (widthTess - 1) * 6 + i * 6 + 5] = (j + 1) * widthTess + i;
			}
		}
	}
}

