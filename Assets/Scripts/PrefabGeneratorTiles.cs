using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabGeneratorTile", menuName = "Tiles", order = 1)]
public class PrefabGeneratorTiles : ScriptableObject {

    public GameObject prefab;
    public float probability;
    public int width;
    public int height;
    public int depth;
}
