using UnityEngine;
using System.Collections;

/// <summary>
/// A mushroom mesh.
/// </summary>
public class ProcMushroom : ProcBase
{
	//cap data:

	//height and radius of the cap:
	public float m_CapRadius = 0.5f;
	public float m_CapHeight = 0.5f;

	//data to define the Bézier handles for the cap curve:
	public float m_CapPeakHandleLength = 0.5f;
	public float m_CapRimHandleLength = 1.0f;
	public float m_CapRimHandleAngle = 0.0f;

	//vertical thickness of the cap (at the peak):
	public float m_CapThickness = 0.2f;

	//number of radial segments in the cap:
	public int m_CapRadialSegmentCount = 10;

	//stem data:

	//height and radius of the stem:
	public float m_StemHeight = 1.0f;
	public float m_StemRadius = 0.3f;

	//the angle to bend the stem:
	public float m_StemBendAngle = 45.0f;

	//number of radial segments in the stem:
	public int m_StemRadialSegmentCount = 10;

	//number of height segments in the stem:
	public int m_StemHeightSegmentCount = 10;

	public override Mesh BuildMesh()
	{
		MeshBuilder meshBuilder = new MeshBuilder();

		//store the current position and rotaion of the stem:
		Quaternion currentRotation = Quaternion.identity;
		Vector3 currentOffset = Vector3.zero;

		//build the stem:

		//build a straight stem if m_StemBendAngle is zero:
		if (m_StemBendAngle == 0.0f)
		{
			//straight cylinder:
			float heightInc = m_StemHeight / m_StemHeightSegmentCount;

			for (int i = 0; i <= m_StemHeightSegmentCount; i++)
			{
				currentOffset = Vector3.up * heightInc * i;

				BuildRing(meshBuilder, m_StemRadialSegmentCount, currentOffset, m_StemRadius, (float)i / m_StemHeightSegmentCount, i > 0);
			}
		}
		else
		{
			//get the angle in radians:
			float stemBendRadians = m_StemBendAngle * Mathf.Deg2Rad;

			//the radius of our bend (vertical) circle:
			float stemBendRadius = m_StemHeight / stemBendRadians;

			//the angle increment per height segment (based on arc length):
			float angleInc = stemBendRadians / m_StemHeightSegmentCount;

			//calculate a start offset that will place the centre of the first ring (angle 0.0f) on the mesh origin:
			//(x = cos(0.0f) * bendRadius, y = sin(0.0f) * bendRadius)
			Vector3 startOffset = new Vector3(stemBendRadius, 0.0f, 0.0f);

			//build the rings:
			for (int i = 0; i <= m_StemHeightSegmentCount; i++)
			{
				//current normalised height value:
				float heightNormalised = (float)i / m_StemHeightSegmentCount;

				//unit position along the edge of the vertical circle:
				currentOffset = Vector3.zero;
				currentOffset.x = Mathf.Cos(angleInc * i);
				currentOffset.y = Mathf.Sin(angleInc * i);

				//rotation at that position on the circle:
				float zAngleDegrees = angleInc * i * Mathf.Rad2Deg;
				currentRotation = Quaternion.Euler(0.0f, 0.0f, zAngleDegrees);

				//multiply the unit postion by the radius:
				currentOffset *= stemBendRadius;

				//offset the position so that the base ring (at angle zero) centres around zero:
				currentOffset -= startOffset;

				//build the ring:
				BuildRing(meshBuilder, m_StemRadialSegmentCount, currentOffset, m_StemRadius, heightNormalised, i > 0, currentRotation);
			}
		}

		//build the cap:

		//positions of the cap peak and rim in the XY cross-section:
		Vector3 capPeak = new Vector3(0.0f, m_CapThickness, 0.0f);
		Vector3 capRim = new Vector3(m_CapRadius, -m_CapHeight + m_CapThickness, 0.0f);

		//Bézier handles to define the cap curve:
		Vector3 peakHandle = new Vector3(m_CapPeakHandleLength, 0.0f, 0.0f);

		float rimAngleRadians = m_CapRimHandleAngle * Mathf.Deg2Rad;
		Vector3 rimHandle = new Vector3(Mathf.Cos(rimAngleRadians), Mathf.Sin(rimAngleRadians), 0.0f);
		rimHandle *= m_CapRimHandleLength;

		//build the outer surface of the cap:
		BuildCap(meshBuilder, currentOffset, currentRotation, capRim, capPeak, capRim + rimHandle, capPeak + peakHandle);

		//build the gills:

		//adjust the Bézier handles for our inner curve:
		capPeak.y -= m_CapThickness;
		rimHandle = new Vector3(-rimHandle.y, rimHandle.x, 0.0f);

		//build the gills (inner surface of the cap)
		//note the reversal of the control points to make the mesh face inward instead of outward:
		BuildCap(meshBuilder, currentOffset, currentRotation, capPeak, capRim, capPeak + peakHandle, capRim + rimHandle);

		return meshBuilder.CreateMesh();
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="meshBuilder">The mesh builder currently being added to.</param>
	/// <param name="offset">The position offset to apply to the whole cap (position at the top of the stem).</param>
	/// <param name="rotation">The rotation offset to apply to the whole cap (rotation at the top of the stem).</param>
	/// <param name="capRim">Position at the rim of the cap (in XY cross-section), or the start point of the Bézier curve.</param>
	/// <param name="capPeak">Position at the peak of the cap (in XY cross-section), or the end point of the Bézier curve.</param>
	/// <param name="controlRim">The first of the inner control points of the Bézier curve (the rim position plus the rim handle).</param>
	/// <param name="controlPeak">The second of the inner control points of the Bézier curve (the peak position plus the peak handle).</param>
	private void BuildCap(MeshBuilder meshBuilder, Vector3 offset, Quaternion rotation, Vector3 capRim, Vector3 capPeak, Vector3 controlRim, Vector3 controlPeak)
	{
		//we'll use a quarter of the radial segments for the height (the same segment allocation as a hemisphere):
		int capHeightSegmentCount = m_CapRadialSegmentCount / 4;

		//build the rings:
		for (int i = 0; i <= capHeightSegmentCount; i++)
		{
			//current normalised height value:
			float heightNormalised = (float)i / capHeightSegmentCount;

			//interpolated position along the Bézier curve:
			Vector3 bezier = Bezier(capRim, controlRim, controlPeak, capPeak, heightNormalised);

			//calculate a height offset and radius based on the Bézier curve position:
			Vector3 centrePos = new Vector3(0.0f, bezier.y, 0.0f);
			float radius = bezier.x;

			//interpolated tangent along the Bézier curve:
			Vector3 tangent = BezierTangent(capRim, controlRim, controlPeak, capPeak, heightNormalised);
			Vector2 slope = new Vector2(tangent.x, tangent.y);

			//build the ring:
			BuildRing(meshBuilder, m_CapRadialSegmentCount, offset + rotation * centrePos, radius, heightNormalised, i > 0, rotation, slope);
		}
	}

	/// <summary>
	/// Gets a position along a Bézier curve.
	/// 
	/// For more information:
	/// http://pomax.github.io/bezierinfo/
	/// </summary>
	/// <param name="start">The first of the four control points for the Bézier curve.</param>
	/// <param name="controlMid1">The second of the four control points for the Bézier curve.</param>
	/// <param name="controlMid2">The third of the four control points for the Bézier curve.</param>
	/// <param name="end">The last of the four control points for the Bézier curve.</param>
	/// <param name="t">The interpolation value, should be between 0.0 and 1.0</param>
	/// <returns>The interpolated position along the Bézier curve.</returns>
	Vector3 Bezier(Vector3 start, Vector3 controlMid1, Vector3 controlMid2, Vector3 end, float t)
	{
		float t2 = t * t;
		float t3 = t2 * t;

		float mt = 1 - t;
		float mt2 = mt * mt;
		float mt3 = mt2 * mt;

		return start * mt3 + controlMid1 * mt2 * t * 3.0f + controlMid2 * mt * t2 * 3.0f + end * t3;
	}

	/// <summary>
	/// Gets a tangent along a Bézier curve.
	/// 
	/// For more information:
	/// http://pomax.github.io/bezierinfo/
	/// </summary>
	/// <param name="start">The first of the four control points for the Bézier curve.</param>
	/// <param name="controlMid1">The second of the four control points for the Bézier curve.</param>
	/// <param name="controlMid2">The third of the four control points for the Bézier curve.</param>
	/// <param name="end">The last of the four control points for the Bézier curve.</param>
	/// <param name="t">The interpolation value, should be between 0.0 and 1.0</param>
	/// <returns>The interpolated tangent along the Bézier curve.</returns>
	Vector3 BezierTangent(Vector3 start, Vector3 controlMid1, Vector3 controlMid2, Vector3 end, float t)
	{
		float t2 = t * t;

		float mt = 1 - t;
		float mt2 = mt * mt;

		float mid = 2.0f * t * mt;

		Vector3 tangent = start * -mt2 + controlMid1 * (mt2 - mid) + controlMid2 * (-t2 + mid) + end * t2;

		return tangent.normalized;
	}
}
