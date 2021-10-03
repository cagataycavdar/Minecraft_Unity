using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugScreen : MonoBehaviour
{
    World world;
    Text text;

    float frameRate;
    float timer;

    int halfWorldSizeInVoxel;
    int halfWorldSizeInChunk;

    private void Start()
    {
        world = GameObject.Find("World").GetComponent<World>();
        text = GetComponent<Text>();

        halfWorldSizeInChunk = VoxelData.WorldSızeChunk / 2;
        halfWorldSizeInVoxel = VoxelData.WorldSizeVoxels / 2;
    }

    private void Update()
    {
        string debugText = "Sukomastik kir plastik kömür plastik";

        debugText += "\n";
        debugText += frameRate + " fps";
        debugText += "\n\n";
        debugText += "XYZ" + (Mathf.FloorToInt(world.player.transform.position.x)-halfWorldSizeInVoxel) + "/" + Mathf.FloorToInt(world.player.transform.position.y)+"/"+ (Mathf.FloorToInt(world.player.transform.position.z)- halfWorldSizeInVoxel);
        debugText += "\n";
        debugText += "Chunk" + (world.playerChunkCoord.x - halfWorldSizeInChunk ) + "/" + (world.playerChunkCoord.z - halfWorldSizeInChunk);
        text.text = debugText;

        if (timer > 1f)
        {
            frameRate = (int)(1f / Time.unscaledTime);
            timer = 0;
        }
        else
            timer += Time.deltaTime;
    }
}
