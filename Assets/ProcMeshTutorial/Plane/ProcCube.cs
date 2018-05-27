using UnityEngine;
using System.Collections;

/// <summary>
/// A cube mesh with a single quad on each side.
/// </summary>
public class ProcCube : ProcBase
{
	//the width, length and height of the cube:
	public float m_Width = 1.0f;
	public float m_Length = 1.0f;
	public float m_Height = 1.0f;

	//Build the mesh:
	public override Mesh BuildMesh()
	{
		//Create a new mesh builder:
		MeshBuilder meshBuilder = new MeshBuilder();

		//calculate directional vectors for all 3 dimensions of the cube:
		Vector3 upDir = Vector3.up * m_Height;
		Vector3 rightDir = Vector3.right * m_Width;
		Vector3 forwardDir = Vector3.forward * m_Length;

		//calculate the positions of two corners opposite each other on the cube:

		//positions that will place the pivot at the corner of the cube:
		Vector3 nearCorner = Vector3.zero;
		Vector3 farCorner = upDir + rightDir + forwardDir;

		////positions that will place the pivot at the centre of the cube:
		//Vector3 farCorner = (upDir + rightDir + forwardDir) / 2;
		//Vector3 nearCorner = -farCorner;

		//build the 3 quads that originate from nearCorner:
		BuildQuad(meshBuilder, nearCorner, forwardDir, rightDir);
		BuildQuad(meshBuilder, nearCorner, rightDir, upDir);
		BuildQuad(meshBuilder, nearCorner, upDir, forwardDir);

		//build the 3 quads that originate from farCorner:
		BuildQuad(meshBuilder, farCorner, -rightDir, -forwardDir);
		BuildQuad(meshBuilder, farCorner, -upDir, -rightDir);
		BuildQuad(meshBuilder, farCorner, -forwardDir, -upDir);

		//initialise the Unity mesh and return it:
		return meshBuilder.CreateMesh();
	}
}
