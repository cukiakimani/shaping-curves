using UnityEngine;
using System.Collections;

/// <summary>
/// A "terrain" mesh. Or rather, a plane using a random height offset in each vertex.
/// 
/// Note: The “bumpiness” in this mesh is done by assigning a random height to each vertex. 
/// It’s done this way because it makes the code nice and simple, not because it makes a good 
/// looking terrain. If you’re serious about building a terrain mesh, you might try using a 
/// heightmap, or perlin noise, or looking into algorithms such as diamond-square.
/// 
/// http://en.wikipedia.org/wiki/Heightmap
/// http://en.wikipedia.org/wiki/Perlin_noise
/// http://en.wikipedia.org/wiki/Diamond-square_algorithm
/// </summary>
public class ProcGround : ProcBase
{
	//The width and length of each segment:
	public float m_Width = 1.0f;
	public float m_Length = 1.0f;

	//The maximum height of the mesh:
	public float m_Height = 1.0f;

	//The number of segments in each dimension (the plane will be m_SegmentCount * m_SegmentCount in area):
	public int m_SegmentCount = 10;

	//Build the mesh:
	public override Mesh BuildMesh()
	{
		//Create a new mesh builder:
		MeshBuilder meshBuilder = new MeshBuilder();

		//Loop through the rows:
		for (int i = 0; i <= m_SegmentCount; i++)
		{
			//incremented values for the Z position and V coordinate:
			float z = m_Length * i;
			float v = (1.0f / m_SegmentCount) * i;

			//Loop through the collumns:
			for (int j = 0; j <= m_SegmentCount; j++)
			{
				//incremented values for the X position and U coordinate:
				float x = m_Width * j;
				float u = (1.0f / m_SegmentCount) * j;

				//The position offset for this quad, with a random height between zero and m_MaxHeight:
				Vector3 offset = new Vector3(x, Random.Range(0.0f, m_Height), z);

				////Build individual quads:
				//BuildQuad(meshBuilder, offset);

				//build quads that share vertices:
				Vector2 uv = new Vector2(u, v);
				bool buildTriangles = i > 0 && j > 0;

				BuildQuadForGrid(meshBuilder, offset, uv, buildTriangles, m_SegmentCount + 1);
			}
		}

		//create the Unity mesh:
		Mesh mesh = meshBuilder.CreateMesh();

		//have the mesh calculate its own normals:
		mesh.RecalculateNormals();

		//return the new mesh:
		return mesh;
	}
}
