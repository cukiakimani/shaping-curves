using UnityEngine;
using System.Collections;

/// <summary>
/// A basic house shape.
/// </summary>
public class ProcHouse : ProcBase
{
	//the width, length and height of the walls:
	public float m_Width = 1.0f;
	public float m_Length = 1.0f;
	public float m_Height = 1.0f;

	//the height of the roof:
	public float m_RoofHeight = 0.3f;

	//the width of the roof overhang at the front and back of the house:
	public float m_RoofOverhangFront = 0.2f;

	//the width of the roof overhang at the sides of the house:
	public float m_RoofOverhangSide = 0.2f;

	//an offset to reduce z-fighting between the walls and roof:
	public float m_RoofBias = 0.02f;

	//Build the mesh:
	public override Mesh BuildMesh()
	{
		//Create a new mesh builder:
		MeshBuilder meshBuilder = new MeshBuilder();

		//build the walls:

		//calculate directional vectors for the walls:
		Vector3 upDir = Vector3.up * m_Height;
		Vector3 rightDir = Vector3.right * m_Width;
		Vector3 forwardDir = Vector3.forward * m_Length;

		Vector3 farCorner = upDir + rightDir + forwardDir;
		Vector3 nearCorner = Vector3.zero;

		//shift the pivot to centre bottom:
		Vector3 pivotOffset = (rightDir + forwardDir) * 0.5f;
		farCorner -= pivotOffset;
		nearCorner -= pivotOffset;

		//build the quads for the walls:
		BuildQuad(meshBuilder, nearCorner, rightDir, upDir);
		BuildQuad(meshBuilder, nearCorner, upDir, forwardDir);

		BuildQuad(meshBuilder, farCorner, -upDir, -rightDir);
		BuildQuad(meshBuilder, farCorner, -forwardDir, -upDir);

		//build the roof:

		//calculate the position of the roof peak at the near end of the house:
		Vector3 roofPeak = Vector3.up * (m_Height + m_RoofHeight) + rightDir * 0.5f - pivotOffset;

		//calculate the positions at the tops of the walls at the same end of the house:
		Vector3 wallTopLeft = upDir - pivotOffset;
		Vector3 wallTopRight = upDir + rightDir - pivotOffset;

		//build triangles at the tops of the walls:
		BuildTriangle(meshBuilder, wallTopLeft, roofPeak, wallTopRight);
		BuildTriangle(meshBuilder, wallTopLeft + forwardDir, wallTopRight + forwardDir, roofPeak + forwardDir);

		//calculate the directions from the roof peak to the sides of the house:
		Vector3 dirFromPeakLeft = wallTopLeft - roofPeak;
		Vector3 dirFromPeakRight = wallTopRight - roofPeak;

		//extend the directions by a length of m_RoofOverhangSide:
		dirFromPeakLeft += dirFromPeakLeft.normalized * m_RoofOverhangSide;
		dirFromPeakRight += dirFromPeakRight.normalized * m_RoofOverhangSide;

		//offset the roofpeak position to put it at the beginning of the front overhang:
		roofPeak -= Vector3.forward * m_RoofOverhangFront;

		//extend the forward directional vecter ot make it long enough for and overhang at either end:
		forwardDir += Vector3.forward * m_RoofOverhangFront * 2.0f;

		//shift the roof slightly upward to stop it intersecting the top of the walls:
		roofPeak += Vector3.up * m_RoofBias;

		//build the quads for the roof:
		BuildQuad(meshBuilder, roofPeak, forwardDir, dirFromPeakLeft);
		BuildQuad(meshBuilder, roofPeak, dirFromPeakRight, forwardDir);

		BuildQuad(meshBuilder, roofPeak, dirFromPeakLeft, forwardDir);
		BuildQuad(meshBuilder, roofPeak, forwardDir, dirFromPeakRight);

		//initialise the Unity mesh and return it:
		return meshBuilder.CreateMesh();
	}
}
