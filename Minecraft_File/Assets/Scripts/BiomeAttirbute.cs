﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="BiomeAttirbutes",menuName ="Minecraft/Biome Attirbute")]
public class BiomeAttirbute : ScriptableObject
{

    public string biomName;
    public int solidGroundHeight;
    public int terrainHeight;
    public float terrainScale;
    public Lode[] lodes;
}

[System.Serializable]
public class Lode
{
    public string nodeName;
    public byte blockID;
    public int minHeight;
    public int maxHeight;
    public float scale;
    public float threshold;
    public float noiseOffset;
}
