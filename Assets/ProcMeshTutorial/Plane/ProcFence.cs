using UnityEngine;
using System.Collections;

/// <summary>
/// A fence mesh.
/// </summary>
public class ProcFence : ProcBase
{
	//post width and height:
	public float m_PostWidth = 0.2f;
	public float m_PostHeight = 1.0f;

	//maximum random variation of the post height (set to 0.0 for no variation):
	public float m_PostHeightVariation = 0.25f;

	//maximum random angle (in degrees) for tilting the posts (set to 0.0 for no variation):
	public float m_PostTiltAngle = 10.0f;

	//crosspiece width and height:
	public float m_CrossPieceHeight = 0.2f;
	public float m_CrossPieceWidth = 0.1f;

	//base Y offset for the crosspieces (a value of 0.0 will sit the crosspieces on the ground):
	public float m_CrossPieceY = 0.5f;

	//maximum random variation of the crosspiece Y offset (set to 0.0 for no variation):
	public float m_CrossPieceYVariation = 0.25f;

	//number of sections in the fence:
	public int m_SectionCount = 10;

	//distance between posts:
	public float m_DistBetweenPosts = 1.0f;

	//Build the mesh:
	public override Mesh BuildMesh()
	{
		//Create a new mesh builder:
		MeshBuilder meshBuilder = new MeshBuilder();

		//some variables to hold values needed by the loop:
		Vector3 prevCrossPosition = Vector3.zero;
		Quaternion prevRotation = Quaternion.identity;

		//interate over all the posts in this fence:
		for (int i = 0; i <= m_SectionCount; i++)
		{
			//calculate the position and rotation of this post:
			Vector3 offset = Vector3.right * m_DistBetweenPosts * i;

			float xAngle = Random.Range(-m_PostTiltAngle, m_PostTiltAngle);
			float zAngle = Random.Range(-m_PostTiltAngle, m_PostTiltAngle);
			Quaternion rotation = Quaternion.Euler(xAngle, 0.0f, zAngle);

			//Any level-specific position offsets (eg. the height of the terrain at this position) should be applied here.

			//build the post:
			BuildPost(meshBuilder, offset, rotation);


			//build the crosspiece:

			//start with the post position:
			Vector3 crossPosition = offset;

			//offset to the back of the post:
			crossPosition += rotation * (Vector3.back * m_PostWidth * 0.5f);

			//calculate 2 random Y offsets (one for each end of the crosspiece):
			float randomYStart = Random.Range(-m_CrossPieceYVariation, m_CrossPieceYVariation);
			float randomYEnd = Random.Range(-m_CrossPieceYVariation, m_CrossPieceYVariation);

			//calculate Y offsets for the start and end positions:
			Vector3 crossYOffsetStart = prevRotation * Vector3.up * (m_CrossPieceY + randomYStart);
			Vector3 crossYOffsetEnd = rotation * Vector3.up * (m_CrossPieceY + randomYEnd);

			//if this is not the first section (ie. if there is a previous post to join to), build the crosspiece:
			if (i != 0)
				BuildCrossPiece(meshBuilder, prevCrossPosition + crossYOffsetStart, crossPosition + crossYOffsetEnd);

			//store the position and rotation for use by the next section:
			prevCrossPosition = crossPosition;
			prevRotation = rotation;
		}

		//initialise the Unity mesh and return it:
		return meshBuilder.CreateMesh();
	}

	/// <summary>
	/// Builds a post extending upward from the specified position:
	/// </summary>
	/// <param name="meshBuilder">The mesh builder currently being added to.</param>
	/// <param name="position">The position of the post.</param>
	/// <param name="rotation">The rotation or the post.</param>
	private void BuildPost(MeshBuilder meshBuilder, Vector3 position, Quaternion rotation)
	{
		//get the post height, including a random offset:
		float postHeight = m_PostHeight + Random.Range(-m_PostHeightVariation, m_PostHeightVariation);

		//calculate directional vectors for all 3 dimensions of the cube:
		Vector3 upDir = rotation * Vector3.up * postHeight;
		Vector3 rightDir = rotation * Vector3.right * m_PostWidth;
		Vector3 forwardDir = rotation * Vector3.forward * m_PostWidth;

		//calculate the positions of two corners opposite each other on the cube:
		Vector3 farCorner = upDir + rightDir + forwardDir + position;
		Vector3 nearCorner = position;

		//shift pivot to centre base:
		Vector3 pivotOffset = (rightDir + forwardDir) * 0.5f;
		farCorner -= pivotOffset;
		nearCorner -= pivotOffset;

		//build the quads that originate from nearCorner (minus the bottom quad):
		BuildQuad(meshBuilder, nearCorner, rightDir, upDir);
		BuildQuad(meshBuilder, nearCorner, upDir, forwardDir);

		//build the 3 quads that originate from farCorner:
		BuildQuad(meshBuilder, farCorner, -rightDir, -forwardDir);
		BuildQuad(meshBuilder, farCorner, -upDir, -rightDir);
		BuildQuad(meshBuilder, farCorner, -forwardDir, -upDir);
	}

	/// <summary>
	/// Builds a crosspiece extending from a starting position to an end position.
	/// </summary>
	/// <param name="meshBuilder">The mesh builder currently being added to.</param>
	/// <param name="start">The start position.</param>
	/// <param name="end">The end position.</param>
	private void BuildCrossPiece(MeshBuilder meshBuilder, Vector3 start, Vector3 end)
	{
		//calculate a directional vector from start to end:
		Vector3 dir = end - start;

		//get a look-at roation based on this direction:
		Quaternion rotation = Quaternion.LookRotation(dir);

		//calculate directional vectors for all 3 dimensions of the cube:
		Vector3 upDir = rotation * Vector3.up * m_CrossPieceHeight;
		Vector3 rightDir = rotation * Vector3.right * m_CrossPieceWidth;
		Vector3 forwardDir = rotation * Vector3.forward * dir.magnitude;

		//calculate the positions of two corners opposite each other on the cube:
		Vector3 farCorner = upDir + rightDir + forwardDir + start;
		Vector3 nearCorner = start;

		//build the 3 quads that originate from nearCorner:
		BuildQuad(meshBuilder, nearCorner, forwardDir, rightDir);
		BuildQuad(meshBuilder, nearCorner, rightDir, upDir);
		BuildQuad(meshBuilder, nearCorner, upDir, forwardDir);

		//build the 3 quads that originate from farCorner:
		BuildQuad(meshBuilder, farCorner, -rightDir, -forwardDir);
		BuildQuad(meshBuilder, farCorner, -upDir, -rightDir);
		BuildQuad(meshBuilder, farCorner, -forwardDir, -upDir);
	}

	/// <summary>
	/// Builds a crosspiece extending from a starting position over a distance of m_DistBetweenPosts.
	/// </summary>
	/// <param name="meshBuilder">The mesh builder currently being added to.</param>
	/// <param name="start">The position of the crosspiece.</param>
	private void BuildCrossPiece(MeshBuilder meshBuilder, Vector3 start)
	{
		//calculate directional vectors for all 3 dimensions of the cube:
		Vector3 upDir = Vector3.up * m_CrossPieceHeight;
		Vector3 rightDir = Vector3.right * m_CrossPieceWidth;
		Vector3 forwardDir = Vector3.forward * m_DistBetweenPosts;

		//calculate the positions of two corners opposite each other on the cube:
		Vector3 farCorner = upDir + rightDir + forwardDir + start;
		Vector3 nearCorner = start;

		//build the 3 quads that originate from nearCorner:
		BuildQuad(meshBuilder, nearCorner, forwardDir, rightDir);
		BuildQuad(meshBuilder, nearCorner, rightDir, upDir);
		BuildQuad(meshBuilder, nearCorner, upDir, forwardDir);

		//build the 3 quads that originate from farCorner:
		BuildQuad(meshBuilder, farCorner, -rightDir, -forwardDir);
		BuildQuad(meshBuilder, farCorner, -upDir, -rightDir);
		BuildQuad(meshBuilder, farCorner, -forwardDir, -upDir);
	}
}
