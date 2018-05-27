using UnityEngine;
using System.Collections;

/// <summary>
/// A flower mesh.
/// </summary>
public class ProcFlower : ProcBase
{
	//base class for the data for each part of the flower:
	[System.Serializable]
	public class PartData
	{
		//should this part be generated?
		public bool m_Build = true;
	}

	//leafy parts (petals, sepals etc.) of the flower:
	[System.Serializable]
	public class LeafPartData : PartData
	{
		//width and length:
		public float m_Width = 0.2f;
		public float m_Length = 0.3f;

		//the the angle to bend:
		public float m_BendAngle = 90.0f;

		//the starting angle:
		public float m_StartAngle = 0.0f;

		//the maximum amount of variation in the bend angle:
		public float m_BendAngleVariation = 10.0f;

		//the maximum amount of variation in the starting angle:
		public float m_StartAngleVariation = 0.0f;

		//the number of petals in a ring:
		public int m_Count = 6;

		//the number of width segments:
		public int m_WidthSegmentCount = 8;

		//the number of length segments:
		public int m_LengthSegmentCount = 8;

		//should the backfaces be built?
		public bool m_BuildBackfaces = true;
	}

	//contains data for building a cylinder:
	[System.Serializable]
	public class CylinderData : PartData
	{
		//height and radius:
		public float m_Radius = 0.05f;
		public float m_Height = 1.0f;

		//the angle to bend the cylinder:
		public float m_BendAngle = 10.0f;

		//the number of radial segments:
		public int m_RadialSegmentCount = 10;

		//the number of height segments:
		public int m_HeightSegmentCount = 10;
	}

	//contains data for building a sphere:
	[System.Serializable]
	public class SphereData : PartData
	{
		//radius of the sphere:
		public float m_Radius = 0.05f;

		//vertical scale value to apply to the sphere:
		public float m_VerticalScale = 1.0f;

		//the number of radial segments:
		public int m_RadialSegmentCount = 10;

		//the number of height segments:
		public int m_HeightSegmentCount = 10;
	}

	//the parts of the flower:
	public CylinderData m_StemData;
	public SphereData m_HeadData;
	public LeafPartData m_SepalData;
	public LeafPartData m_PetalData;
	
		
	public override Mesh BuildMesh()
	{
		MeshBuilder meshBuilder = new MeshBuilder();

		//store the current position and rotaion of the stem:
		Vector3 currentPosition;
		Quaternion currentRotation;

		//build the main stem:
		BuildStem(meshBuilder, out currentPosition, out currentRotation, m_StemData);
		BuildHead(meshBuilder, currentPosition, currentRotation, m_HeadData);

		//build the sepals:
		BuildLeafRing(meshBuilder, currentPosition, currentRotation, m_StemData.m_Radius, m_SepalData);

		//build the petals:
		BuildLeafRing(meshBuilder, currentPosition, currentRotation, m_StemData.m_Radius, m_PetalData);

		return meshBuilder.CreateMesh();
	}

	/// <summary>
	/// Builds the stem.
	/// </summary>
	/// <param name="meshBuilder">The mesh builder currently being added to.</param>
	/// <param name="currentOffset">Vector3 to store the position at the end of the stem.</param>
	/// <param name="currentRotation">Quaternion to store the rotation at the end of the stem.</param>
	/// <param name="partData">The parameters describing the cylinder to be built.</param>
	private void BuildStem(MeshBuilder meshBuilder, out Vector3 currentOffset, out Quaternion currentRotation, CylinderData partData)
	{
		currentOffset = Vector3.zero;
		currentRotation = Quaternion.identity;

		//bail if this part has been disabled:
		if (!partData.m_Build)
			return;

		//build a straight stem if partData.m_BendAngle is zero:
		if (partData.m_BendAngle == 0.0f)
		{
			//straight cylinder:
			float heightInc = partData.m_Height / partData.m_HeightSegmentCount;

			for (int i = 0; i <= partData.m_HeightSegmentCount; i++)
			{
				currentOffset = Vector3.up * heightInc * i;

				BuildRing(meshBuilder, partData.m_RadialSegmentCount, currentOffset, partData.m_Radius, (float)i / partData.m_HeightSegmentCount, i > 0);
			}
		}
		else
		{
			//get the bend angle in radians:
			float stemBendRadians = partData.m_BendAngle * Mathf.Deg2Rad;

			//the radius of our bend (vertical) circle:
			float stemBendRadius = partData.m_Height / stemBendRadians;

			//the angle increment per height segment (based on arc length):
			float angleInc = stemBendRadians / partData.m_HeightSegmentCount;

			//calculate a start offset that will place the centre of the first ring (angle 0.0f) on the mesh origin:
			//(x = cos(0.0f) * stemBendRadius, y = sin(0.0f) * stemBendRadius)
			Vector3 startOffset = new Vector3(stemBendRadius, 0.0f, 0.0f);

			//build the rings:
			for (int i = 0; i <= partData.m_HeightSegmentCount; i++)
			{
				//current normalised height value:
				float heightNormalised = (float)i / partData.m_HeightSegmentCount;

				//unit position along the edge of the vertical circle:
				currentOffset = Vector3.zero;
				currentOffset.x = Mathf.Cos(angleInc * i);
				currentOffset.y = Mathf.Sin(angleInc * i);

				//rotation at that position on the circle:
				float zAngleDegrees = angleInc * i * Mathf.Rad2Deg;
				currentRotation = Quaternion.Euler(0.0f, 0.0f, zAngleDegrees);

				//multiply the unit postion by the bend radius:
				currentOffset *= stemBendRadius;

				//offset the position so that the base ring (at angle zero) centres around zero:
				currentOffset -= startOffset;

				//build the ring:
				BuildRing(meshBuilder, partData.m_RadialSegmentCount, currentOffset, partData.m_Radius, heightNormalised, i > 0, currentRotation);
			}
		}
	}

	/// <summary>
	/// Builds the "head" of the flower (a sphere that sits on top of the stem).
	/// </summary>
	/// <param name="meshBuilder">The mesh builder currently being added to.</param>
	/// <param name="offset">The position offset to apply to the head (position at the top of the stem).</param>
	/// <param name="rotation">The rotation offset to apply to the head (rotation at the top of the stem).</param>
	/// <param name="partData">The parameters describing the sphere to be built.</param>
	private void BuildHead(MeshBuilder meshBuilder, Vector3 offset, Quaternion rotation, SphereData partData)
	{
		//bail if this part has been disabled:
		if (!partData.m_Build)
			return;

		//the angle increment per height segment:
		float angleInc = Mathf.PI / partData.m_HeightSegmentCount;

		//the vertical (scaled) radius of the sphere:
		float verticalRadius = partData.m_Radius * partData.m_VerticalScale;

		//build the rings:
		for (int i = 0; i <= partData.m_HeightSegmentCount; i++)
		{
			Vector3 centrePos = Vector3.zero;

			//calculate a height offset and radius based on a vertical circle calculation:
			centrePos.y = -Mathf.Cos(angleInc * i);
			float radius = Mathf.Sin(angleInc * i);

			//calculate the slope of the shpere at this ring based on the height and radius:
			Vector2 slope = new Vector3(-centrePos.y / partData.m_VerticalScale, radius);
			slope.Normalize();

			//multiply the unit height by the vertical radius, and then add the radius to the height to make this sphere originate from its base rather than its centre:
			centrePos.y = centrePos.y * verticalRadius + verticalRadius;

			//scale the radius by the one stored in the partData:
			radius *= partData.m_Radius;

			//calculate the final position of the ring centre:
			Vector3 finalRingCentre = rotation * centrePos + offset;

			//V coordinate:
			float v = (float)i / partData.m_HeightSegmentCount;

			//build the ring:
			BuildRing(meshBuilder, partData.m_RadialSegmentCount, finalRingCentre, radius, v, i > 0, rotation, slope);
		}
	}

	/// <summary>
	/// Builds a ring of leafy parts (petals, sepals, etc.).
	/// </summary>
	/// <param name="meshBuilder">The mesh builder currently being added to.</param>
	/// <param name="offset">The position offset to apply (position at the top of the stem).</param>
	/// <param name="rotation">The rotation offset to apply(rotation at the top of the stem).</param>
	/// <param name="radius">The radius at the top of the stem.</param>
	/// <param name="partData">The parameters describing the part to be built.</param>
	private void BuildLeafRing(MeshBuilder meshBuilder, Vector3 offset, Quaternion rotation, float radius, LeafPartData partData)
	{
		//bail if this part has been disabled:
		if (!partData.m_Build)
			return;

		for (int i = 0; i < partData.m_Count; i++)
		{
			//calculate the rotation of this part:
			float yAngle = 360.0f * i / partData.m_Count;
			Quaternion radialRotation = rotation * Quaternion.Euler(0.0f, yAngle, 0.0f);

			//set the postion at the top of the stem, away from the middle:
			Vector3 position = offset + radialRotation * Vector3.forward * radius;

			//calculate a bend angle with random variation:
			float bendAngleRandom = Random.Range(-partData.m_BendAngleVariation, partData.m_BendAngleVariation);
			float bendAngle = partData.m_BendAngle + bendAngleRandom;

			//calculate a starting angle with random variation:
			float startAngleRandom = Random.Range(-partData.m_StartAngleVariation, partData.m_StartAngleVariation);
			float startAngle = partData.m_StartAngle + startAngleRandom;

			//build the leaf part:
			BuildLeafPart(meshBuilder, position, radialRotation, partData, false, bendAngle, startAngle);
		}
	}

	/// <summary>
	/// Build a single leaf part (petals, sepals, etc.).
	/// </summary>
	/// <param name="meshBuilder">The mesh builder currently being added to.</param>
	/// <param name="offset">The position offset to apply (position at the base of the leaf part).</param>
	/// <param name="rotation">The rotation offset to apply.</param>
	/// <param name="partData">The parameters describing the part to be built.</param>
	/// <param name="isBackFace">Is this the back side of the part?</param>
	/// <param name="bendAngle">The bend angle</param>
	/// <param name="startAngle">The starting angle.</param>
	private void BuildLeafPart(MeshBuilder meshBuilder, Vector3 offset, Quaternion rotation, LeafPartData partData, bool isBackFace, float bendAngle, float startAngle)
	{
		//get the angle in radians:
		float bendAngleRadians = bendAngle * Mathf.Deg2Rad;

		//the radius of our bend (vertical) circle:
		float bendRadius = partData.m_Length / bendAngleRadians;

		//the angle increment per height segment (based on arc length):
		float angleInc = bendAngleRadians / partData.m_LengthSegmentCount;

		//get the starting angle in radians:
		float startAngleRadians = startAngle * Mathf.Deg2Rad;

		//calculate a startOffset based on the starting angle:
		Vector3 startOffset = Vector3.zero;
		startOffset.y = Mathf.Cos(startAngleRadians) * bendRadius;
		startOffset.z = Mathf.Sin(startAngleRadians) * bendRadius;

		//a multiplier to reverse some values for the back of the leaf part:
		float backFaceMultiplier = isBackFace ? -1.0f : 1.0f;

		//build the rows:
		for (int i = 0; i <= partData.m_LengthSegmentCount; i++)
		{
			//V coordinate:
			float v = (1.0f / partData.m_LengthSegmentCount) * i;

			//width of the current row, scaled to shape the leaf part:
			float localWidth = partData.m_Width * Mathf.Sin(v * Mathf.PI) * backFaceMultiplier;
			////use this instead for rectangular leaves:
			//float localWidth = partData.m_Width * backFaceMultiplier;

			//offset the x value to put the origin of the leaf part at bottom-centre:
			float xOffset = -localWidth * 0.5f;

			//unit position along the edge of the vertical circle:
			Vector3 centrePos = Vector3.zero;
			centrePos.y = Mathf.Cos(angleInc * i + startAngleRadians);
			centrePos.z = Mathf.Sin(angleInc * i + startAngleRadians);

			//rotation at that position on the circle:
			float bendAngleDegrees = (angleInc * i + startAngleRadians) * Mathf.Rad2Deg;
			Quaternion bendRotation = Quaternion.Euler(bendAngleDegrees, 0.0f, 0.0f);

			//multiply the unit postion by the radius:
			centrePos *= bendRadius;

			//offset the position so that the base row (at the starting angle) sits on zero:
			centrePos -= startOffset;

			//calculate the normal for this row:
			Vector3 normal = rotation * (bendRotation * Vector3.up) * backFaceMultiplier;

			//build the row:
			for (int j = 0; j <= partData.m_WidthSegmentCount; j++)
			{
				//X position:
				float x = (localWidth / partData.m_WidthSegmentCount) * j;

				//U coordinate:
				float u = (1.0f / partData.m_WidthSegmentCount) * j;

				//calculate the final position of this quad:
				Vector3 position = offset + rotation * new Vector3(x + xOffset, centrePos.y, centrePos.z);

				Vector2 uv = new Vector2(u, v);
				bool buildTriangles = i > 0 && j > 0;

				//build the quad:
				BuildQuadForGrid(meshBuilder, position, uv, buildTriangles, partData.m_WidthSegmentCount + 1, normal);
			}
		}

		//if not building the back side of the leaf parts, and the part data does not have backfaces disabled, 
		//rebuild this part, facing in the other direction:
		if (!isBackFace && partData.m_BuildBackfaces)
			BuildLeafPart(meshBuilder, offset, rotation, partData, true, bendAngle, startAngle);
	}
}
