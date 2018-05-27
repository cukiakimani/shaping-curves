using UnityEngine;
using System.Collections;

/// <summary>
/// A sphere mesh.
/// </summary>
public class ProcSphere : ProcBase
{
	//the radius of the sphere:
	public float m_Radius = 0.5f;

	//the number of radial segments:
	public int m_RadialSegmentCount = 10;

	//Build the mesh:
	public override Mesh BuildMesh()
	{
		//Create a new mesh builder:
		MeshBuilder meshBuilder = new MeshBuilder();

		//height segments need to be half m_RadialSegmentCount for the sphere to be even horizontally and vertically:
		int heightSegmentCount = m_RadialSegmentCount / 2;

		//the angle increment per height segment:
		float angleInc = Mathf.PI / heightSegmentCount;

		for (int i = 0; i <= heightSegmentCount; i++)
		{
			Vector3 centrePos = Vector3.zero;

			//calculate a height offset and radius based on a vertical circle calculation:
			centrePos.y = -Mathf.Cos(angleInc * i) * m_Radius;
			float radius = Mathf.Sin(angleInc * i) * m_Radius;

			float v = (float)i / heightSegmentCount;

			//build the ring:
			BuildRingForSphere(meshBuilder, m_RadialSegmentCount, centrePos, radius, v, i > 0);
		}

		return meshBuilder.CreateMesh();
	}
}
