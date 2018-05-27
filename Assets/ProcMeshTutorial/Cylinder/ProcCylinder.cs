using UnityEngine;
using System.Collections;

/// <summary>
/// A basic cylinder mesh.
/// </summary>
public class ProcCylinder : ProcBase
{
	//the radius and height of the cylinder:
	public float m_Radius = 0.5f;
	public float m_Height = 2.0f;

	//the number of radial segments:
	public int m_RadialSegmentCount = 10;

	//the number of height segments:
	public int m_HeightSegmentCount = 4;

	//Build the mesh:
	public override Mesh BuildMesh()
	{
		//Create a new mesh builder:
		MeshBuilder meshBuilder = new MeshBuilder();

		////one-segment cylinder (build two rings, one at the bottom and one at the top):
		//BuildRing(meshBuilder, m_RadialSegmentCount, Vector3.zero, m_Radius, 0.0f, false);
		//BuildRing(meshBuilder, m_RadialSegmentCount, Vector3.up * m_Height, m_Radius, 1.0f, true);

		//multi-segment cylinder:
		float heightInc = m_Height / m_HeightSegmentCount;

		for (int i = 0; i <= m_HeightSegmentCount; i++)
		{
			//centre position of this ring:
			Vector3 centrePos = Vector3.up * heightInc * i;

			//V coordinate is based on height:
			float v = (float)i / m_HeightSegmentCount;

			BuildRing(meshBuilder, m_RadialSegmentCount, centrePos, m_Radius, v, i > 0);
		}

		////caps:
		//BuildCap(meshBuilder, Vector3.zero, true);
		//BuildCap(meshBuilder, Vector3.up * m_Height, false);

		return meshBuilder.CreateMesh();
	}

	/// <summary>
	/// Adds a cap to the top or bottom of the cylinder.
	/// </summary>
	/// <param name="meshBuilder">The mesh builder currently being added to.</param>
	/// <param name="centre">The postion at the centre of the cap.</param>
	/// <param name="reverseDirection">Should the normal and winding order of the cap be reversed? (Should be true for bottom cap, false for the top)</param>
	private void BuildCap(MeshBuilder meshBuilder, Vector3 centre, bool reverseDirection)
	{
		//the normal will either be up or down:
		Vector3 normal = reverseDirection ? Vector3.down : Vector3.up;

		//add one vertex in the center:
		meshBuilder.Vertices.Add(centre);
		meshBuilder.Normals.Add(normal);
		meshBuilder.UVs.Add(new Vector2(0.5f, 0.5f));

		//store the index of the vertex we just added for later reference:
		int centreVertexIndex = meshBuilder.Vertices.Count - 1;

		//build the vertices around the edge:
		float angleInc = (Mathf.PI * 2.0f) / m_RadialSegmentCount;

		for (int i = 0; i <= m_RadialSegmentCount; i++)
		{
			float angle = angleInc * i;

			Vector3 unitPosition = Vector3.zero;
			unitPosition.x = Mathf.Cos(angle);
			unitPosition.z = Mathf.Sin(angle);

			meshBuilder.Vertices.Add(centre + unitPosition * m_Radius);
			meshBuilder.Normals.Add(normal);

			Vector2 uv = new Vector2(unitPosition.x + 1.0f, unitPosition.z + 1.0f) * 0.5f;
			meshBuilder.UVs.Add(uv);

			//build a triangle:
			if (i > 0)
			{
				int baseIndex = meshBuilder.Vertices.Count - 1;

				if (reverseDirection)
					meshBuilder.AddTriangle(centreVertexIndex, baseIndex - 1, baseIndex);
				else
					meshBuilder.AddTriangle(centreVertexIndex, baseIndex, baseIndex - 1);
			}
		}
	}

}
