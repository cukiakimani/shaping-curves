using UnityEngine;
using System.Collections;

/// <summary>
/// A simple procedural quad mesh, generated without the use of the MeshBuilder class.
/// </summary>
public class ProcQuadSimple : MonoBehaviour
{
	//The width and length of the quad:
	public float m_Width = 1.0f;
	public float m_Length = 1.0f;

	//Initialisation:
	private void Start()
	{
		//Initialise the arrays to contain our mesh data.
		//A quad contains 4 vertices and 2 triangles:
		Vector3[] vertices = new Vector3[4];
		Vector3[] normals = new Vector3[4];
		Vector2[] uv = new Vector2[4];

		int[] indices = new int[6]; //2 triangles at 3 indices each


		//initialise the vertices, arranged in a rectangular shape with [0,0] at the first corner:
		vertices[0] = new Vector3(0.0f, 0.0f, 0.0f);
		uv[0] = new Vector2(0.0f, 0.0f);
		normals[0] = Vector3.up;

		vertices[1] = new Vector3(0.0f, 0.0f, m_Length);
		uv[1] = new Vector2(0.0f, 1.0f);
		normals[1] = Vector3.up;

		vertices[2] = new Vector3(m_Width, 0.0f, m_Length);
		uv[2] = new Vector2(1.0f, 1.0f);
		normals[2] = Vector3.up;

		vertices[3] = new Vector3(m_Width, 0.0f, 0.0f);
		uv[3] = new Vector2(1.0f, 0.0f);
		normals[3] = Vector3.up;


		//initialise the triangles, with the vertex indices ordered clockwise (when viewed from above):
		indices[0] = 0;
		indices[1] = 1;
		indices[2] = 2;

		indices[3] = 0;
		indices[4] = 2;
		indices[5] = 3;


		//Create an instance of the Unity Mesh class:
		Mesh mesh = new Mesh();

		//add our vertex and triangle values to the new mesh:
		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.uv = uv;
		mesh.triangles = indices;

		//have the mesh recalculate its bounding box (required for proper rendering):
		mesh.RecalculateBounds();

		//Look for a MeshFilter component attached to this GameObject:
		MeshFilter filter = GetComponent<MeshFilter>();

		//If the MeshFilter exists, attach the new mesh to it.
		//Assuming the GameObject also has a renderer attached, our new mesh will now be visible in the scene.
		if (filter != null)
		{
			filter.sharedMesh = mesh;
		}
	}
}
