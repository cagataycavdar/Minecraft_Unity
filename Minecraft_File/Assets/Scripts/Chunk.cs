using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    
    void Start()
    {
        int verticesIndex = 0;
        List<Vector3> voxelVertices = new List<Vector3>();
        List<int> voxelTriangels = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int p = 0; p < 6; p++)
        {
            for (int i = 0; i < 6; i++)
            {
                int triangelsIndex = VoxelData.voxelTris[p, i];
                voxelVertices.Add(VoxelData.voxelVertices[triangelsIndex]);
                voxelTriangels.Add(verticesIndex);
                uvs.Add(VoxelData.uvVertices[i]);
                verticesIndex++;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = voxelVertices.ToArray();
        mesh.triangles = voxelTriangels.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }


}
