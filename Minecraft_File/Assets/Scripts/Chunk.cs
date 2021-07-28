using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    int verticesIndex = 0;
    List<Vector3> voxelVertices = new List<Vector3>();
    List<int> voxelTriangels = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    bool[,,] voxelMap = new bool[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];
    void Start()
    {
        PopulateVoxelMap();
        CreateChunkMesh();  
        CreateMesh();
    }

    public void CreateDataMesh(Vector3 pos)
    {
        for (int p = 0; p < 6; p++)
        {
            //Eğer Çizdirilen Voxel'in arka yüzünde Bir Voxel Varsa Bunu Çizdirmesin.
            //It's gonna check the voxel behind it if it returns true that means as a voxel there because of this
            //we don't need to draw draw this face so skip over it if it returns false it means there's nothing there 
            if (!CheckVoxel(pos + VoxelData.faceChek[p]))
            {
                voxelVertices.Add(pos + VoxelData.voxelVertices[VoxelData.voxelTris[p, 0]]);
                voxelVertices.Add(pos + VoxelData.voxelVertices[VoxelData.voxelTris[p, 1]]);
                voxelVertices.Add(pos + VoxelData.voxelVertices[VoxelData.voxelTris[p, 2]]);
                voxelVertices.Add(pos + VoxelData.voxelVertices[VoxelData.voxelTris[p, 3]]);
                uvs.Add(VoxelData.uvVertices[0]);
                uvs.Add(VoxelData.uvVertices[1]);
                uvs.Add(VoxelData.uvVertices[2]);
                uvs.Add(VoxelData.uvVertices[3]);
                voxelTriangels.Add(verticesIndex);
                voxelTriangels.Add(verticesIndex+1);
                voxelTriangels.Add(verticesIndex+2);
                voxelTriangels.Add(verticesIndex+2);
                voxelTriangels.Add(verticesIndex+1);
                voxelTriangels.Add(verticesIndex+3);
                verticesIndex += 4;
            }
           
        }
    }
    public void PopulateVoxelMap()
    {
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    voxelMap[x, y, z] = true;
                }
            }
        }
    }
    public void CreateChunkMesh()
    {
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    CreateDataMesh(new Vector3(x, y, z));
                }
            }
        }
    }
    public void CreateMesh()
    {

        Mesh mesh = new Mesh();
        mesh.vertices = voxelVertices.ToArray();
        mesh.triangles = voxelTriangels.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }

    bool CheckVoxel(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (x < 0 || x > VoxelData.ChunkWidth - 1 || y < 0 || y > VoxelData.ChunkHeight - 1 || z < 0 || z > VoxelData.ChunkWidth - 1)
         return false;

        return voxelMap[x,y,z];
    }
}
