using UnityEngine;
using System.Collections;

/// <summary>
/// A cylinder with a taper deformation.
/// </summary>
public class ProcCylinderTaper : ProcBase
{
	//the radii at the start and end of the cylinder:
	public float m_RadiusStart = 0.5f;
	public float m_RadiusEnd = 0.0f;

	//the height of the cylinder:
	public float m_Height = 2.0f;

	//the number of radial segments:
	public int m_RadialSegmentCount = 10;

	//the number of height segments:
	public int m_HeightSegmentCount = 4;

	public override Mesh BuildMesh()
	{
		MeshBuilder meshBuilder = new MeshBuilder();

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


		return meshBuilder.CreateMesh();
	}
}