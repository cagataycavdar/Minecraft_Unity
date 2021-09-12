using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public Transform player;
    public Vector3 spawnPos;
    public Material material;
    public BlockType[] blockstype;
    Chunk[,] chunks = new Chunk[VoxelData.WorldSızeChunk,VoxelData.WorldSızeChunk];
    List<ChunkCoord> activeChunks = new List<ChunkCoord>();
    ChunkCoord playerChunkCoord;
    ChunkCoord playerLastChunkCoord;

    private void Start()
    {
        spawnPos = new Vector3((VoxelData.WorldSızeChunk * VoxelData.ChunkWidth) / 2f, VoxelData.ChunkHeight+2, (VoxelData.WorldSızeChunk * VoxelData.ChunkWidth) / 2f);
        GenerateWorld();
        playerLastChunkCoord = GetChunkCoordFromVector3(player.position);
    }

    private void Update()
    {
        playerChunkCoord = GetChunkCoordFromVector3(player.position);
        if(!playerChunkCoord.Equals(playerLastChunkCoord))
        CheckViewDistance();
    }
    void GenerateWorld()
    {
        for (int x = (VoxelData.WorldSızeChunk/2)-VoxelData.ViewDistanceInChunks; x < (VoxelData.WorldSızeChunk / 2) + VoxelData.ViewDistanceInChunks; x++)
        {
            for (int z = (VoxelData.WorldSızeChunk / 2) - VoxelData.ViewDistanceInChunks; z < (VoxelData.WorldSızeChunk / 2) + VoxelData.ViewDistanceInChunks; z++)
            {
                CreateNewChunk(x, z);
            }
        }
        player.position = spawnPos;
    }
    ChunkCoord GetChunkCoordFromVector3(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);
        return new ChunkCoord(x, z);
    }
    void CheckViewDistance()
    {
        ChunkCoord coord = GetChunkCoordFromVector3(player.position);
        List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeChunks);
        for (int x = coord.x-VoxelData.ViewDistanceInChunks; x < coord.x+VoxelData.ViewDistanceInChunks; x++)
        {
            for (int z = coord.z - VoxelData.ViewDistanceInChunks; z < coord.z + VoxelData.ViewDistanceInChunks; z++)
            {
                if(isChunkInWorld(new ChunkCoord(x,z)))
                {
                    if(chunks[x,z]==null)
                    {
                        CreateNewChunk(x,z);
                    }
                    else if(!chunks[x,z].isActive)
                    {
                        chunks[x, z].isActive = true;
                        activeChunks.Add(new ChunkCoord(x, z));
                    }
                }
                for (int i = 0; i < previouslyActiveChunks.Count; i++)
                {
                    if (previouslyActiveChunks[i].Equals(new ChunkCoord(x,z)))
                    {
                        previouslyActiveChunks.RemoveAt(i);
                    }
                }
            }
        }
        foreach(ChunkCoord c in previouslyActiveChunks)
        {
            chunks[c.x, c.z].isActive = false;
        }
    }


    public byte GetVoxel(Vector3 pos)
    {
        if (!IsVoxelInWorld(pos))
            return 0;
        if (pos.y < 1)
            return  1;
        else if (pos.y == VoxelData.ChunkHeight - 1)
            return  3;
        else
            return  2;
    }

    void CreateNewChunk(int x, int z)
    {
        chunks[x, z] = new Chunk(new ChunkCoord(x, z), this);
        activeChunks.Add(new ChunkCoord(x,z));
    }

    bool isChunkInWorld(ChunkCoord coord)
    {
        if (coord.x > 0 && coord.x < VoxelData.WorldSızeChunk - 1 && coord.z > 0 && coord.z < VoxelData.WorldSızeChunk - 1)
            return true;
        else
            return false;
    }

    bool IsVoxelInWorld(Vector3 pos)
    {
        if (pos.x >= 0 && pos.x < VoxelData.WorldSizeVoxels  && pos.y >= 0 && pos.y < VoxelData.ChunkHeight  && pos.z >= 0 && pos.z < VoxelData.WorldSizeVoxels )
            return true;
        else
            return false;
    }
}

[System.Serializable]
public class BlockType
{
    public string BlockName;
    public bool isSolid;

    public int backFaceTexture;
    public int frontFaceTexture;
    public int topFaceTexture;
    public int bottomFaceTexture;
    public int leftFaceTexture;
    public int rightFaceTexture;


    //Back, Front, Top, Bottom, Left, Right

    public int getFaceId(int faceIndex)
    {
        switch(faceIndex)
        {
            case 0:
                return backFaceTexture;
            case 1:
                return frontFaceTexture;
            case 2:
                return topFaceTexture;
            case 3:
                return bottomFaceTexture;
            case 4:
                return leftFaceTexture;
            case 5:
                return rightFaceTexture;
            default:
                Debug.Log("Error Face Index Texture");
                return 0;
        }
    }
}
