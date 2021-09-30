using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    GameObject chunkGameObject;
     MeshFilter meshFilter;
     MeshRenderer meshRenderer;
    public ChunkCoord coord;
    int verticesIndex = 0;
    List<Vector3> voxelVertices = new List<Vector3>();
    List<int> voxelTriangels = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    public byte[,,] voxelMap = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];
    World world;

    public Chunk(ChunkCoord _coord ,World _worl)
    {
        coord = _coord;
        world = _worl;

        chunkGameObject = new GameObject();
        meshFilter = chunkGameObject.AddComponent<MeshFilter>();
        meshRenderer = chunkGameObject.AddComponent<MeshRenderer>();

        meshRenderer.material = world.material;
        chunkGameObject.transform.SetParent(world.transform);
        chunkGameObject.transform.position = new Vector3(coord.x * VoxelData.ChunkWidth, 0f, coord.z * VoxelData.ChunkWidth);
        chunkGameObject.name = "Chunk" + coord.x + "," + coord.z;
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
                    
                    voxelMap[x, y, z] = world.GetVoxel(new Vector3(x, y, z)+position);
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
                    if (world.blockstype[voxelMap[x, y, z]].isSolid)
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


    public bool isActive
    {
        get { return chunkGameObject.activeSelf; }
        set { chunkGameObject.SetActive(value); }
    }

    public Vector3 position
    {
        get { return chunkGameObject.transform.position; }
        set { }
    }
    bool IsVoxelInChunk(int x,int y, int z)
    {
        if (x < 0 || x > VoxelData.ChunkWidth - 1 || y < 0 || y > VoxelData.ChunkHeight - 1 || z < 0 || z > VoxelData.ChunkWidth - 1)
            return false;
        else
            return true;
    }
    bool CheckVoxel(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (!IsVoxelInChunk(x, y, z))
            return world.blockstype[world.GetVoxel(pos+position)].isSolid;

        
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

public class ChunkCoord
{
    public int x;
    public int z;

    public ChunkCoord(int _x, int _z)
    {
        x = _x;
        z = _z;
    }
    public bool Equals(ChunkCoord other)
    {
        if (other == null)
            return false;
        else if (other.x == x && other.z == z)
            return true;
        else
            return false;
    }
}
