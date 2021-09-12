using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelData
{
    public static readonly int ChunkWidth = 5;
    public static readonly int ChunkHeight = 15;
    public static readonly int TextureAtlasSizeInBlocks = 4;
    public static readonly int WorldSızeChunk =100;
    public static int WorldSizeVoxels
    {
        get { return WorldSızeChunk * ChunkWidth; }
    }

    public static readonly int ViewDistanceInChunks=5;
    public static float NormalizedBlockTextureSize { 
        //it's return 0.25 value
        get { return 1f / (float)TextureAtlasSizeInBlocks;}
    }

    public static readonly Vector3[] voxelVertices = new Vector3[8] {
            new Vector3(0.0f,0.0f,0.0f),
            new Vector3(1.0f,0.0f,0.0f),
            new Vector3(1.0f,1.0f,0.0f),
            new Vector3(0.0f,1.0f,0.0f),
            new Vector3(0.0f,0.0f,1.0f),
            new Vector3(1.0f,0.0f,1.0f),
            new Vector3(1.0f,1.0f,1.0f),
            new Vector3(0.0f,1.0f,1.0f),
    };

    public static readonly Vector3[] faceChek = new Vector3[6] {
    new Vector3(0,0,-1),//Back Face
    new Vector3(0,0,1),//Front Face
    new Vector3(0,1,0),//Top Face
    new Vector3(0,-1,0),//Bottom Face
    new Vector3(-1,0,0),//Left Face
    new Vector3(1,0,0)//Right Face
    };

    public static readonly int[,] voxelTris = new int[6, 4] {

        //Back, Front, Top, Bottom, Left, Right

              {0,3,1,2},    //Back Face
              {5,6,4,7},    //Front face
              {3,7,2,6},    //Top Face -- 
              {1,5,0,4},    //Bottom Face
              {4,7,0,3},    //Left Face
              {1,2,5,6}     //Right Face
    };

    public static readonly Vector2[] uvVertices = new Vector2[4]
        {
        new Vector2(0f,0f),
        new Vector2(0f,1f),
        new Vector2(1f,0f),
        new Vector2(1f,1f)
        };
}
