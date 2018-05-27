using UnityEngine;
using System.Collections;

/// <summary>
/// A cylinder with both bend and taper deformations.
/// </summary>
public class ProcCylinderBendTaper : ProcBase
{
	//the radii at the start and end of the cylinder:
	public float m_RadiusStart = 0.5f;
	public float m_RadiusEnd = 0.0f;

	//the height of the cylinder:
	public float m_Height = 2.0f;

	//the angle to bend the cylinder:
	public float m_BendAngle = 90.0f;

	//the number of radial segments:
	public int m_RadialSegmentCount = 10;

	//the number of height segments:
	public int m_HeightSegmentCount = 4;

	public override Mesh BuildMesh()
	{
		MeshBuilder meshBuilder = new MeshBuilder();

		//our bend code breaks if m_BendAngle is zero:
		if (m_BendAngle == 0.0f)
		{
			//taper only:
			float heightInc = m_Height / m_HeightSegmentCount;

			//calculate the slope of the cylinder based on the height and difference between radii:
			Vector2 slope = new Vector2(m_RadiusEnd - m_RadiusStart, m_Height);
			slope.Normalize();

			//build the rings:
			for (int i = 0; i <= m_HeightSegmentCount; i++)
			{
				//centre position of this ring:
				Vector3 centrePos = Vector3.up * heightInc * i;

				//V coordinate is based on height:
				float v = (float)i / m_HeightSegmentCount;

				//interpolate between the radii:
				float radius = Mathf.Lerp(m_RadiusStart, m_RadiusEnd, (float)i / m_HeightSegmentCount);

				//build the ring:
				BuildRing(meshBuilder, m_RadialSegmentCount, centrePos, radius, v, i > 0, Quaternion.identity, slope);
			}
		}
		else
		{
			//bend and taper:

			//get the angle in radians:
			float bendAngleRadians = m_BendAngle * Mathf.Deg2Rad;

			//the radius of our bend (vertical) circle:
			float bendRadius = m_Height / bendAngleRadians;

			//the angle increment per height segment (based on arc length):
			float angleInc = bendAngleRadians / m_HeightSegmentCount;

			//calculate a start offset that will place the centre of the first ring (angle 0.0f) on the mesh origin:
			//(x = cos(0.0f) * bendRadius, y = sin(0.0f) * bendRadius)
			Vector3 startOffset = new Vector3(bendRadius, 0.0f, 0.0f);

			//calculate the slope of the cylinder based on the height and difference between radii:
			Vector2 slope = new Vector2(m_RadiusEnd - m_RadiusStart, m_Height);
			slope.Normalize();

			//build the rings:
			for (int i = 0; i <= m_HeightSegmentCount; i++)
			{
				//unit position along the edge of the vertical circle:
				Vector3 centrePos = Vector3.zero;
				centrePos.x = Mathf.Cos(angleInc * i);
				centrePos.y = Mathf.Sin(angleInc * i);

				//rotation at that position on the circle:
				float zAngleDegrees = angleInc * i * Mathf.Rad2Deg;
				Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, zAngleDegrees);

				//multiply the unit postion by the radius:
				centrePos *= bendRadius;

				//offset the position so that the base ring (at angle zero) centres around zero:
				centrePos -= startOffset;

				//interpolate between the radii:
				float radius = Mathf.Lerp(m_RadiusStart, m_RadiusEnd, (float)i / m_HeightSegmentCount);

				//V coordinate is based on height:
				float v = (float)i / m_HeightSegmentCount;

				//build the ring:
				BuildRing(meshBuilder, m_RadialSegmentCount, centrePos, radius, v, i > 0, rotation, slope);
			}
		}

		return meshBuilder.CreateMesh();
	}
}