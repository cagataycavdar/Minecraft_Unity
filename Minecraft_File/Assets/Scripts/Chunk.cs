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

    byte[,,] voxelMap = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];
    World world;
    void Start()
    {
        world = GameObject.Find("World").GetComponent<World>();
        PopulateVoxelMap();
        CreateChunkMesh();  
        CreateMesh();
    }

    public void CreateDataMesh(Vector3 pos)
    {
        for (int p = 0; p < 6; p++)
        {

            byte blockID = voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];

            //Eğer Çizdirilen Voxel'in arka yüzünde Bir Voxel Varsa Bunu Çizdirmesin.
            //It's gonna check the voxel behind it if it returns true that means as a voxel there because of this
            //we don't need to draw draw this face so skip over it if it returns false it means there's nothing there 
            if (!CheckVoxel(pos + VoxelData.faceChek[p]))
            {
                voxelVertices.Add(pos + VoxelData.voxelVertices[VoxelData.voxelTris[p, 0]]);
                voxelVertices.Add(pos + VoxelData.voxelVertices[VoxelData.voxelTris[p, 1]]);
                voxelVertices.Add(pos + VoxelData.voxelVertices[VoxelData.voxelTris[p, 2]]);
                voxelVertices.Add(pos + VoxelData.voxelVertices[VoxelData.voxelTris[p, 3]]);

                AddTexture(world.blockstype[blockID].getFaceId(p));

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
                    if(y<1)
                    voxelMap[x, y, z] = 0;
                   else if (y == VoxelData.ChunkHeight - 1)
                        voxelMap[x, y, z] = 2;
                    else
                        voxelMap[x, y, z] = 1;
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
        Debug.Log(world.blockstype[voxelMap[x, y, z]].isSolid);
        return world.blockstype[voxelMap[x,y,z]].isSolid;
    }

    void AddTexture(int textureID)
    {
        float y = textureID / VoxelData.TextureAtlasSizeInBlocks;
        float x = textureID - (y * VoxelData.TextureAtlasSizeInBlocks);
        x *= VoxelData.NormalizedBlockTextureSize;
        y *= VoxelData.NormalizedBlockTextureSize;
        y = 1f - y - VoxelData.NormalizedBlockTextureSize;

        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y+ VoxelData.NormalizedBlockTextureSize));
        uvs.Add(new Vector2(x+ VoxelData.NormalizedBlockTextureSize, y));
        uvs.Add(new Vector2(x+ VoxelData.NormalizedBlockTextureSize, y+ VoxelData.NormalizedBlockTextureSize));

    }
}
