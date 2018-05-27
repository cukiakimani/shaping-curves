using UnityEngine;
using System.Collections;

/// <summary>
/// Base class for procedural meshes. Contains generic initialisation code and shared methods such as BuildQuad() and BuildRing()
/// </summary>
public abstract class ProcBase : MonoBehaviour
{
	/// <summary>
	/// Method for building a mesh. Called in Start()
	/// </summary>
	/// <returns>The completed mesh</returns>
	public abstract Mesh BuildMesh();

	/// <summary>
	/// Initialisation. Build the mesh and assigns it to the object's MeshFilter
	/// </summary>
	private void Start()
	{
		//Build the mesh:
		Mesh mesh = BuildMesh();

		//Look for a MeshFilter component attached to this GameObject:
		MeshFilter filter = GetComponent<MeshFilter>();

		//If the MeshFilter exists, attach the new mesh to it.
		//Assuming the GameObject also has a renderer attached, our new mesh will now be visible in the scene.
		if (filter != null)
		{
			filter.sharedMesh = mesh;
		}
	}

	#region "BuildQuad() methods"

	/// <summary>
	/// Builds a single quad in the XZ plane, facing up the Y axis.
	/// </summary>
	/// <param name="meshBuilder">The mesh builder currently being added to.</param>
	/// <param name="offset">A position offset for the quad.</param>
	/// <param name="width">The width of the quad.</param>
	/// <param name="length">The length of the quad.</param>
	protected void BuildQuad(MeshBuilder meshBuilder, Vector3 offset, float width, float length)
	{
		meshBuilder.Vertices.Add(new Vector3(0.0f, 0.0f, 0.0f) + offset);
		meshBuilder.UVs.Add(new Vector2(0.0f, 0.0f));
		meshBuilder.Normals.Add(Vector3.up);

		meshBuilder.Vertices.Add(new Vector3(0.0f, 0.0f, length) + offset);
		meshBuilder.UVs.Add(new Vector2(0.0f, 1.0f));
		meshBuilder.Normals.Add(Vector3.up);

		meshBuilder.Vertices.Add(new Vector3(width, 0.0f, length) + offset);
		meshBuilder.UVs.Add(new Vector2(1.0f, 1.0f));
		meshBuilder.Normals.Add(Vector3.up);

		meshBuilder.Vertices.Add(new Vector3(width, 0.0f, 0.0f) + offset);
		meshBuilder.UVs.Add(new Vector2(1.0f, 0.0f));
		meshBuilder.Normals.Add(Vector3.up);

		//we don't know how many verts the meshBuilder is up to, but we only care about the four we just added:
		int baseIndex = meshBuilder.Vertices.Count - 4;

		meshBuilder.AddTriangle(baseIndex, baseIndex + 1, baseIndex + 2);
		meshBuilder.AddTriangle(baseIndex, baseIndex + 2, baseIndex + 3);
	}

	/// <summary>
	/// Builds a single quad based on a position offset and width and length vectors.
	/// </summary>
	/// <param name="meshBuilder">The mesh builder currently being added to.</param>
	/// <param name="offset">A position offset for the quad.</param>
	/// <param name="widthDir">The width vector of the quad.</param>
	/// <param name="lengthDir">The length vector of the quad.</param>
	protected void BuildQuad(MeshBuilder meshBuilder, Vector3 offset, Vector3 widthDir, Vector3 lengthDir)
	{
		Vector3 normal = Vector3.Cross(lengthDir, widthDir).normalized;

		meshBuilder.Vertices.Add(offset);
		meshBuilder.UVs.Add(new Vector2(0.0f, 0.0f));
		meshBuilder.Normals.Add(normal);

		meshBuilder.Vertices.Add(offset + lengthDir);
		meshBuilder.UVs.Add(new Vector2(0.0f, 1.0f));
		meshBuilder.Normals.Add(normal);

		meshBuilder.Vertices.Add(offset + lengthDir + widthDir);
		meshBuilder.UVs.Add(new Vector2(1.0f, 1.0f));
		meshBuilder.Normals.Add(normal);

		meshBuilder.Vertices.Add(offset + widthDir);
		meshBuilder.UVs.Add(new Vector2(1.0f, 0.0f));
		meshBuilder.Normals.Add(normal);

		//we don't know how many verts the meshBuilder is up to, but we only care about the four we just added:
		int baseIndex = meshBuilder.Vertices.Count - 4;

		meshBuilder.AddTriangle(baseIndex, baseIndex + 1, baseIndex + 2);
		meshBuilder.AddTriangle(baseIndex, baseIndex + 2, baseIndex + 3);
	}

	#endregion

	#region "BuildQuadForGrid() methods"

	/// <summary>
	/// Builds a single quad as part of a mesh grid.
	/// </summary>
	/// <param name="meshBuilder">The mesh builder currently being added to.</param>
	/// <param name="position">A position offset for the quad. Specifically the position of the corner vertex of the quad.</param>
	/// <param name="uv">The UV coordinates of the quad's corner vertex.</param>
	/// <param name="buildTriangles">Should triangles be built for this quad? This value should be false if this is the first quad in any row or collumn.</param>
	/// <param name="vertsPerRow">The number of vertices per row in this grid.</param>
	protected void BuildQuadForGrid(MeshBuilder meshBuilder, Vector3 position, Vector2 uv, bool buildTriangles, int vertsPerRow)
	{
		meshBuilder.Vertices.Add(position);
		meshBuilder.UVs.Add(uv);

		if (buildTriangles)
		{
			int baseIndex = meshBuilder.Vertices.Count - 1;

			int index0 = baseIndex;
			int index1 = baseIndex - 1;
			int index2 = baseIndex - vertsPerRow;
			int index3 = baseIndex - vertsPerRow - 1;

			meshBuilder.AddTriangle(index0, index2, index1);
			meshBuilder.AddTriangle(index2, index3, index1);
		}
	}

	/// <summary>
	/// Builds a single quad as part of a mesh grid.
	/// </summary>
	/// <param name="meshBuilder">The mesh builder currently being added to.</param>
	/// <param name="position">A position offset for the quad. Specifically the position of the corner vertex of the quad.</param>
	/// <param name="uv">The UV coordinates of the quad's corner vertex.</param>
	/// <param name="buildTriangles">Should triangles be built for this quad? This value should be false if this is the first quad in any row or collumn.</param>
	/// <param name="vertsPerRow">The number of vertices per row in this grid.</param>
	/// <param name="normal">The normal of the quad's corner vertex.</param>
	protected void BuildQuadForGrid(MeshBuilder meshBuilder, Vector3 position, Vector2 uv, bool buildTriangles, int vertsPerRow, Vector3 normal)
	{
		meshBuilder.Vertices.Add(position);
		meshBuilder.UVs.Add(uv);
		meshBuilder.Normals.Add(normal);

		if (buildTriangles)
		{
			int baseIndex = meshBuilder.Vertices.Count - 1;

			int index0 = baseIndex;
			int index1 = baseIndex - 1;
			int index2 = baseIndex - vertsPerRow;
			int index3 = baseIndex - vertsPerRow - 1;

			meshBuilder.AddTriangle(index0, index2, index1);
			meshBuilder.AddTriangle(index2, index3, index1);
		}
	}

	#endregion

	#region "BuildRing() methods"

	/// <summary>
	/// Builds a ring as part of a cylinder.
	/// </summary>
	/// <param name="meshBuilder">The mesh builder currently being added to.</param>
	/// <param name="segmentCount">The number of segments in this ring.</param>
	/// <param name="centre">The position at the centre of the ring.</param>
	/// <param name="radius">The radius of the ring.</param>
	/// <param name="v">The V coordinate for this ring.</param>
	/// <param name="buildTriangles">Should triangles be built for this ring? This value should be false if this is the first ring in the cylinder.</param>
	protected void BuildRing(MeshBuilder meshBuilder, int segmentCount, Vector3 centre, float radius, float v, bool buildTriangles)
	{
		float angleInc = (Mathf.PI * 2.0f) / segmentCount;

		for (int i = 0; i <= segmentCount; i++)
		{
			float angle = angleInc * i;

			Vector3 unitPosition = Vector3.zero;
			unitPosition.x = Mathf.Cos(angle);
			unitPosition.z = Mathf.Sin(angle);

			meshBuilder.Vertices.Add(centre + unitPosition * radius);
			meshBuilder.Normals.Add(unitPosition);
			meshBuilder.UVs.Add(new Vector2((float)i / segmentCount, v));

			if (i > 0 && buildTriangles)
			{
				int baseIndex = meshBuilder.Vertices.Count - 1;

				int vertsPerRow = segmentCount + 1;

				int index0 = baseIndex;
				int index1 = baseIndex - 1;
				int index2 = baseIndex - vertsPerRow;
				int index3 = baseIndex - vertsPerRow - 1;

				meshBuilder.AddTriangle(index0, index2, index1);
				meshBuilder.AddTriangle(index2, index3, index1);
			}
		}
	}

	/// <summary>
	/// Builds a ring as part of a cylinder.
	/// </summary>
	/// <param name="meshBuilder">The mesh builder currently being added to.</param>
	/// <param name="segmentCount">The number of segments in this ring.</param>
	/// <param name="centre">The position at the centre of the ring.</param>
	/// <param name="radius">The radius of the ring.</param>
	/// <param name="v">The V coordinate for this ring.</param>
	/// <param name="buildTriangles">Should triangles be built for this ring? This value should be false if this is the first ring in the cylinder.</param>
	/// <param name="rotation">A rotation value to be applied to the whole ring.</param>
	protected void BuildRing(MeshBuilder meshBuilder, int segmentCount, Vector3 centre, float radius, float v, bool buildTriangles, Quaternion rotation)
	{
		float angleInc = (Mathf.PI * 2.0f) / segmentCount;

		for (int i = 0; i <= segmentCount; i++)
		{
			float angle = angleInc * i;

			Vector3 unitPosition = Vector3.zero;
			unitPosition.x = Mathf.Cos(angle);
			unitPosition.z = Mathf.Sin(angle);

			unitPosition = rotation * unitPosition;

			meshBuilder.Vertices.Add(centre + unitPosition * radius);
			meshBuilder.Normals.Add(unitPosition);
			meshBuilder.UVs.Add(new Vector2((float)i / segmentCount, v));

			if (i > 0 && buildTriangles)
			{
				int baseIndex = meshBuilder.Vertices.Count - 1;

				int vertsPerRow = segmentCount + 1;

				int index0 = baseIndex;
				int index1 = baseIndex - 1;
				int index2 = baseIndex - vertsPerRow;
				int index3 = baseIndex - vertsPerRow - 1;

				meshBuilder.AddTriangle(index0, index2, index1);
				meshBuilder.AddTriangle(index2, index3, index1);
			}
		}
	}

	/// <summary>
	/// Builds a ring as part of a cylinder.
	/// </summary>
	/// <param name="meshBuilder">The mesh builder currently being added to.</param>
	/// <param name="segmentCount">The number of segments in this ring.</param>
	/// <param name="centre">The position at the centre of the ring.</param>
	/// <param name="radius">The radius of the ring.</param>
	/// <param name="v">The V coordinate for this ring.</param>
	/// <param name="buildTriangles">Should triangles be built for this ring? This value should be false if this is the first ring in the cylinder.</param>
	/// <param name="rotation">A rotation value to be applied to the whole ring.</param>
	/// <param name="slope">The normalised slope (rise and run) of the cylinder at this height.</param>
	protected void BuildRing(MeshBuilder meshBuilder, int segmentCount, Vector3 centre, float radius, float v, bool buildTriangles, Quaternion rotation, Vector2 slope)
	{
		float angleInc = (Mathf.PI * 2.0f) / segmentCount;

		for (int i = 0; i <= segmentCount; i++)
		{
			float angle = angleInc * i;

			Vector3 unitPosition = Vector3.zero;
			unitPosition.x = Mathf.Cos(angle);
			unitPosition.z = Mathf.Sin(angle);

			float normalVertical = -slope.x;
			float normalHorizontal = slope.y;

			Vector3 normal = unitPosition * normalHorizontal;
			normal.y = normalVertical;

			normal = rotation * normal;

			unitPosition = rotation * unitPosition;

			meshBuilder.Vertices.Add(centre + unitPosition * radius);
			meshBuilder.Normals.Add(normal);
			meshBuilder.UVs.Add(new Vector2((float)i / segmentCount, v));

			if (i > 0 && buildTriangles)
			{
				int baseIndex = meshBuilder.Vertices.Count - 1;

				int vertsPerRow = segmentCount + 1;

				int index0 = baseIndex;
				int index1 = baseIndex - 1;
				int index2 = baseIndex - vertsPerRow;
				int index3 = baseIndex - vertsPerRow - 1;

				meshBuilder.AddTriangle(index0, index2, index1);
				meshBuilder.AddTriangle(index2, index3, index1);
			}
		}
	}

	#endregion

	/// <summary>
	/// Builds a single triangle.
	/// </summary>
	/// <param name="meshBuilder">The mesh builder currently being added to.</param>
	/// <param name="corner0">The vertex position at index 0 of the triangle.</param>
	/// <param name="corner1">The vertex position at index 1 of the triangle.</param>
	/// <param name="corner2">The vertex position at index 2 of the triangle.</param>
	protected void BuildTriangle(MeshBuilder meshBuilder, Vector3 corner0, Vector3 corner1, Vector3 corner2)
	{
		Vector3 normal = Vector3.Cross((corner1 - corner0), (corner2 - corner0)).normalized;

		meshBuilder.Vertices.Add(corner0);
		meshBuilder.UVs.Add(new Vector2(0.0f, 0.0f));
		meshBuilder.Normals.Add(normal);

		meshBuilder.Vertices.Add(corner1);
		meshBuilder.UVs.Add(new Vector2(0.0f, 1.0f));
		meshBuilder.Normals.Add(normal);

		meshBuilder.Vertices.Add(corner2);
		meshBuilder.UVs.Add(new Vector2(1.0f, 1.0f));
		meshBuilder.Normals.Add(normal);

		int baseIndex = meshBuilder.Vertices.Count - 3;

		meshBuilder.AddTriangle(baseIndex, baseIndex + 1, baseIndex + 2);
	}

	/// <summary>
	/// Builds a ring as part of a sphere. Normals are calculated as directions from the sphere's centre.
	/// </summary>
	/// <param name="meshBuilder">The mesh builder currently being added to.</param>
	/// <param name="segmentCount">The number of segments in this ring.</param>
	/// <param name="centre">The position at the centre of the ring.</param>
	/// <param name="radius">The radius of the ring.</param>
	/// <param name="v">The V coordinate for this ring.</param>
	/// <param name="buildTriangles">Should triangles be built for this ring? This value should be false if this is the first ring in the cylinder.</param>
	protected void BuildRingForSphere(MeshBuilder meshBuilder, int segmentCount, Vector3 centre, float radius, float v, bool buildTriangles)
	{
		float angleInc = (Mathf.PI * 2.0f) / segmentCount;

		for (int i = 0; i <= segmentCount; i++)
		{
			float angle = angleInc * i;

			Vector3 unitPosition = Vector3.zero;
			unitPosition.x = Mathf.Cos(angle);
			unitPosition.z = Mathf.Sin(angle);

			Vector3 vertexPosition = centre + unitPosition * radius;

			meshBuilder.Vertices.Add(vertexPosition);
			meshBuilder.Normals.Add(vertexPosition.normalized);
			meshBuilder.UVs.Add(new Vector2((float)i / segmentCount, v));

			if (i > 0 && buildTriangles)
			{
				int baseIndex = meshBuilder.Vertices.Count - 1;

				int vertsPerRow = segmentCount + 1;

				int index0 = baseIndex;
				int index1 = baseIndex - 1;
				int index2 = baseIndex - vertsPerRow;
				int index3 = baseIndex - vertsPerRow - 1;

				meshBuilder.AddTriangle(index0, index2, index1);
				meshBuilder.AddTriangle(index2, index3, index1);
			}
		}
	}

	//private void OnDrawGizmos()
	//{
	//    Gizmos.DrawCube(transform.position, Vector3.one);
	//}
}
