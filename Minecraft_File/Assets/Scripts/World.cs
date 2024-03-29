﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class World : MonoBehaviour
{
    public int seed;
    public Transform player;
    public Vector3 spawnPos;
    public Material material;
    public BlockType[] blockstype;
    Chunk[,] chunks = new Chunk[VoxelData.WorldSızeChunk,VoxelData.WorldSızeChunk];
    List<ChunkCoord> activeChunks = new List<ChunkCoord>();
    List<ChunkCoord> chunksToCreate = new List<ChunkCoord>();
    public ChunkCoord playerChunkCoord;
    ChunkCoord playerLastChunkCoord;
    public BiomeAttirbute biome;
    bool isCreatingChunk;
    public GameObject debugScreen;
    private void Start()
    {
        Random.InitState(seed);
        spawnPos = new Vector3((VoxelData.WorldSızeChunk * VoxelData.ChunkWidth) / 2f, VoxelData.ChunkHeight-50, (VoxelData.WorldSızeChunk * VoxelData.ChunkWidth) / 2f);
        GenerateWorld();
        playerLastChunkCoord = GetChunkCoordFromVector3(player.position);
    }

    private void Update()
    {
        playerChunkCoord = GetChunkCoordFromVector3(player.position);
        if(!playerChunkCoord.Equals(playerLastChunkCoord))
            CheckViewDistance();

        if (chunksToCreate.Count > 0 && !isCreatingChunk)
            StartCoroutine(CreateChunks());

        if (Input.GetKeyDown(KeyCode.F3))
            debugScreen.SetActive(!debugScreen.activeSelf);
    }
    void GenerateWorld()
    {
        for (int x = (VoxelData.WorldSızeChunk/2)-VoxelData.ViewDistanceInChunks; x < (VoxelData.WorldSızeChunk / 2) + VoxelData.ViewDistanceInChunks; x++)
        {
            for (int z = (VoxelData.WorldSızeChunk / 2) - VoxelData.ViewDistanceInChunks; z < (VoxelData.WorldSızeChunk / 2) + VoxelData.ViewDistanceInChunks; z++)
            {
                chunks[x, z] = new Chunk(new ChunkCoord(x, z), this, true);
                activeChunks.Add(new ChunkCoord(x, z));
            }
        }
        player.position = spawnPos;
    }

    IEnumerator CreateChunks()
    {
        isCreatingChunk = true;
        
        while(chunksToCreate.Count>0)
        {
            chunks[chunksToCreate[0].x, chunksToCreate[0].z].Init();
            chunksToCreate.RemoveAt(0);
            yield return null;
        }

            isCreatingChunk = false;
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
        playerLastChunkCoord = playerChunkCoord;
        List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeChunks);
        for (int x = coord.x-VoxelData.ViewDistanceInChunks; x < coord.x+VoxelData.ViewDistanceInChunks; x++)
        {
            for (int z = coord.z - VoxelData.ViewDistanceInChunks; z < coord.z + VoxelData.ViewDistanceInChunks; z++)
            {
                if(isChunkInWorld(new ChunkCoord(x,z)))
                {
                    if(chunks[x,z]==null)
                    {
                        chunks[x, z] = new Chunk(new ChunkCoord(x, z), this, false);
                        chunksToCreate.Add(new ChunkCoord(x, z));
                    }
                    else if(!chunks[x,z].isActive)
                    {
                        chunks[x, z].isActive = true;
                        
                    }
                    activeChunks.Add(new ChunkCoord(x, z));
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

    public bool CheckForVoxel(Vector3 pos)
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        if (!isChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.ChunkHeight)
            return false;

        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulate)
            return blockstype[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].isSolid;

        return blockstype[GetVoxel(pos)].isSolid;
    }

    public byte GetVoxel(Vector3 pos)
    {
        int yPos = Mathf.FloorToInt(pos.y);

        //eğer map dışında ise air texture döndür
        if (!IsVoxelInWorld(pos))
            return 0;

        //Eğer tabana ulaştıysa bedrock block döndür
        if(yPos==0)
            return 1;

        /* Terrain Pass */

        int terrainHeight = Mathf.FloorToInt(biome.terrainHeight*PerlinNoise.Get2DPerlinNoise(new Vector2(pos.x,pos.z),500,biome.terrainScale))+biome.solidGroundHeight;
        byte voxelValue = 0;
        if (yPos == terrainHeight)
            voxelValue = 3;

        else if (yPos < terrainHeight && yPos > terrainHeight - 4)
            voxelValue = 5;
        else if (yPos > terrainHeight)
            return 0;

        else
            voxelValue = 2;

        /* ikinci geçiş */

        if (voxelValue == 2)
        {
            foreach(Lode lode in biome.lodes)
            {
                if (yPos > lode.minHeight && yPos < lode.maxHeight)
                    if (PerlinNoise.Get3DPerlinNoise(pos, lode.noiseOffset, lode.scale, lode.threshold))
                        voxelValue = lode.blockID;
            }
        }
            return voxelValue;
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
