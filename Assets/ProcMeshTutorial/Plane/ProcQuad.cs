using UnityEngine;
using System.Collections;

/// <summary>
/// A simple procedural quad mesh, generated using the MeshBuilder class.
/// </summary>
public class ProcQuad : MonoBehaviour
{
	//The width and length of the quad:
	public float m_Width = 1.0f;
	public float m_Length = 1.0f;

	//Initialisation:
	private void Start()
	{
		//Create a new mesh builder:
		MeshBuilder meshBuilder = new MeshBuilder();

		//Add the vertices:
		meshBuilder.Vertices.Add(new Vector3(0.0f, 0.0f, 0.0f));
		meshBuilder.UVs.Add(new Vector2(0.0f, 0.0f));
		meshBuilder.Normals.Add(Vector3.up);

		meshBuilder.Vertices.Add(new Vector3(0.0f, 0.0f, m_Length));
		meshBuilder.UVs.Add(new Vector2(0.0f, 1.0f));
		meshBuilder.Normals.Add(Vector3.up);

		meshBuilder.Vertices.Add(new Vector3(m_Width, 0.0f, m_Length));
		meshBuilder.UVs.Add(new Vector2(1.0f, 1.0f));
		meshBuilder.Normals.Add(Vector3.up);

		meshBuilder.Vertices.Add(new Vector3(m_Width, 0.0f, 0.0f));
		meshBuilder.UVs.Add(new Vector2(1.0f, 0.0f));
		meshBuilder.Normals.Add(Vector3.up);

		//Add the triangles:
		meshBuilder.AddTriangle(0, 1, 2);
		meshBuilder.AddTriangle(0, 2, 3);

		//Create the mesh:
		Mesh mesh = meshBuilder.CreateMesh();

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
