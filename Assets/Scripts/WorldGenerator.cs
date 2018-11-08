using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GeneratorType {RandomNumbers, Prefabs, PerlinNoise};

public class WorldGenerator : MonoBehaviour {
    public GeneratorType generatorType;
    public GameObject world;
    [Range(0,100)]
    public int mapWidth;
    [Range(0, 25)]
    public int mapHeight;
    [Range(0,100)]
    public int mapDepth;
    public bool isRevealingTheMapSlowly;
    public int timeToRevealMap;

    int[,,] worldData;

    [Header("Tile Prefabs")]
    [Tooltip("The tilePrefab[i] has a probability of tileProbabilies[i] to appear.")]
    public float[] tileProbabilities;
    public GameObject[] tilePrefabs;


    [Header("From Prefab Generator Variables")]
    public PrefabGeneratorTiles[] prefabGeneratorTiles;

    [Header("Perlin Noise Generator Variables")]
    public bool isUsingRandomPerlinNoise;
    public bool isUsingHeightMap;
    public bool isShowingHeightMapTexture;
    public GameObject heightMap;
    [Tooltip("Lower value leads to broader Mountains")]
    [Range(0, 10)]
    public float spikyness;
    public bool isUsingSnow;
    public GameObject snowPrefab;
    [Range(0, 25)]
    public float snowLine;
    public bool isUsingBioms;
    public bool isShowingBiomTexture;
    public GameObject biomMap;
    [Tooltip("Bigger biom scale leads to bigger bioms")]
    [Range(0,15)]
    public float biomScale;
    public bool isUsingThreedimensionalBioms;
    public bool isShowingThreedimensionalBiomTexture;
    public GameObject threeDimensionalBiomMap;
    /*
    public bool isUsingClouds;
    public bool isUsingForest;
    */

    Texture2D heightTexture;
    Texture2D biomTexture;
    Texture2D threeDimensionalBiomTexture;

    float randomX;
    float randomY;
    float randomZ;
    
	// Use this for initialization
	void Start () {
        CreateNewWorld();
	}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            CreateNewWorld();
        }
    }

    public void CreateNewWorld()
    {
        DeleteOldWorld();
        world = new GameObject("World");
        worldData = new int[mapWidth, mapHeight, mapDepth];
        switch (generatorType)
        {
            case GeneratorType.RandomNumbers:
                CreateWorldFromRandomNumbers();
                break;
            case GeneratorType.Prefabs:
                CreateWorldFromPrefabs();
                break;
            case GeneratorType.PerlinNoise:
                CreateWorldFromPerlinNoise();
                break;
            default:
                break;
        }        
        
    }

    public void DeleteOldWorld()
    {
        if(world != null)
        {
            Destroy(world);
        }
    }

    private void CreateWorldFromRandomNumbers()
    {        
        float fullProbability = 0;
        float[] combinedTileProbabilities = new float[tileProbabilities.Length];
        for (int i = 0; i < tilePrefabs.Length; i++)
        {
            fullProbability += tileProbabilities[i];
            combinedTileProbabilities[i] = fullProbability;
        }
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                for (int z = 0; z < mapDepth; z++)
                {
                    float randomTileValue = Random.Range(0.0f, fullProbability);
                    for (int i = 0; i < combinedTileProbabilities.Length; i++)
                    {
                        if(randomTileValue <= combinedTileProbabilities[i])
                        {
                            worldData[x, y, z] = i;
                            break;
                        }
                    }
                }
            }
        }
        StartCoroutine(populateWorld());
    }

    private void CreateWorldFromPrefabs()
    {
        float fullProbability = 0;
        float[] combinedTileProbabilities = new float[tileProbabilities.Length];
        for (int i = 0; i < prefabGeneratorTiles.Length; i++)
        {
            fullProbability += prefabGeneratorTiles[i].probability;
            combinedTileProbabilities[i] = fullProbability;
        }
        for (int x = 0; x < mapWidth; x += 10)
        {
            for (int y = 0; y < mapHeight; y += 5)
            {
                for (int z = 0; z < mapDepth; z += 10)
                {
                    float randomTileValue = Random.Range(0.0f, fullProbability);
                    for (int i = 0; i < combinedTileProbabilities.Length; i++)
                    {
                        if (randomTileValue <= combinedTileProbabilities[i])
                        {
                            Instantiate(prefabGeneratorTiles[i].prefab, new Vector3(x,y,z), Quaternion.identity, world.transform);
                            break;
                        }
                    }
                }
            }
        }
    }

    private void CreateWorldFromPerlinNoise()
    {
        if (isUsingRandomPerlinNoise)
        {
            randomX = Random.Range(0.0f, 1);
            randomY = Random.Range(0.0f, 1);
            randomZ = Random.Range(0.0f, 1);
        }
        if (isShowingHeightMapTexture)
        {
            heightTexture = new Texture2D(mapWidth, mapDepth);
            heightMap.SetActive(true);
            heightMap.GetComponent<Renderer>().material.mainTexture = heightTexture;
            heightMap.transform.localScale = new Vector3(mapWidth, mapDepth, 1);
            heightMap.transform.position = new Vector3(mapWidth - 1, -2, mapDepth - 1) / 2;
        }
        else
        {
            heightMap.SetActive(false);
        }
        if (isShowingBiomTexture)
        {
            biomTexture = new Texture2D(mapWidth, mapDepth);
            biomMap.SetActive(true);
            biomMap.GetComponent<Renderer>().material.mainTexture = biomTexture;
            biomMap.transform.localScale = new Vector3(mapWidth, mapDepth, 1);
            biomMap.transform.position = new Vector3(mapWidth - 1, -2, mapDepth - 1) / 2;
        }
        else
        {
            biomMap.SetActive(false);
        }
        if (isShowingThreedimensionalBiomTexture)
        {
            threeDimensionalBiomTexture = new Texture2D(mapWidth, mapDepth);
            threeDimensionalBiomMap.SetActive(true);
            threeDimensionalBiomMap.GetComponent<Renderer>().material.mainTexture = biomTexture;
            threeDimensionalBiomMap.transform.localScale = new Vector3(mapWidth, mapHeight, 1);
            threeDimensionalBiomMap.transform.position = new Vector3(mapWidth - 1, mapHeight - 2, -2) / 2;
        }
        else
        {
            threeDimensionalBiomMap.SetActive(false);
        }
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                for (int z = 0; z < mapDepth; z++)
                {
                    worldData[x, y, z] = CalculateWorldDataFromPerlinNoise(x, y, z);
                }
            }
        }
        
        StartCoroutine(populateWorld());
    }

    public int CalculateWorldDataFromPerlinNoise(float tileX, float tileY, float tileZ)
    {
        
        int tileNumber = 0;
        if (isUsingHeightMap)
        {
            float heightX = (randomX + tileX / mapWidth) / (1/spikyness) * mapWidth/25;
            float heightZ = (randomZ + tileZ / mapDepth) / (1/spikyness) * mapDepth/25;
            float heightValue = Mathf.PerlinNoise(heightX, heightZ) * mapHeight;
            if (isShowingHeightMapTexture)
            {
                float sample = heightValue / mapHeight;
                Color color = new Color(sample, sample, sample);
                heightTexture.SetPixel((int)tileX, (int)tileZ, color);
                heightTexture.Apply();
            }            
            if (heightValue < tileY)
            {
                return -1;
            }
        }
        if (isUsingSnow)
        {
            if(tileY > snowLine)
            {
                return -2;
            }
        }
        if (isUsingBioms)
        {
            float fullProbability = 0;
            float[] combinedTileProbabilities = new float[tileProbabilities.Length];
            for (int i = 0; i < tilePrefabs.Length; i++)
            {
                fullProbability += tileProbabilities[i];
                combinedTileProbabilities[i] = fullProbability;
            }

            float biomX = (randomX + tileX / mapWidth) / biomScale * mapWidth / 25;
            float biomY = (randomY + tileY / mapHeight) / biomScale * mapWidth / 25;
            float biomZ = (randomZ + tileZ / mapDepth) / biomScale * mapDepth / 25;
            for (int i = 0; i < combinedTileProbabilities.Length; i++)
            {
                if (isShowingBiomTexture)
                {
                    float sample = Mathf.PerlinNoise(biomX, biomZ) / combinedTileProbabilities.Length;
                    Color color = new Color(sample, sample, sample);
                    biomTexture.SetPixel((int)tileX, (int)tileZ, color);
                    biomTexture.Apply();
                }
                if (isUsingThreedimensionalBioms)
                {
                    if (((Mathf.PerlinNoise(biomX, biomZ) + (Mathf.PerlinNoise(biomX, biomY)) / 2) * fullProbability) <= combinedTileProbabilities[i])
                    {
                        if (isShowingThreedimensionalBiomTexture)
                        {
                            float sample = Mathf.PerlinNoise(biomX, biomY) / combinedTileProbabilities.Length;
                            Color color = new Color(sample, sample, sample);
                            threeDimensionalBiomTexture.SetPixel((int)tileX, (int)tileZ, color);
                            threeDimensionalBiomTexture.Apply();
                        }
                        return tileNumber = i;
                    }
                }
                else if (Mathf.PerlinNoise(biomX, biomZ) * fullProbability <= combinedTileProbabilities[i])
                {
                    
                    return tileNumber = i;
                }
                
            }
            
        }
        return tileNumber;
    }

    private IEnumerator populateWorld()
    {
        int numberOfCreatedTiles = 0;
        float tilesPerFrame = (mapWidth * mapHeight * mapDepth) / (timeToRevealMap * 600);
        if (isRevealingTheMapSlowly)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    for (int z = 0; z < mapDepth; z++)
                    {
                        if (worldData[x, y, z] >= 0)
                        {
                            Instantiate(tilePrefabs[worldData[x, y, z]], new Vector3(x, y, z), Quaternion.identity, world.transform);
                        }
                        else if(worldData[x, y, z] == -2)
                        {
                            Instantiate(snowPrefab, new Vector3(x, y, z), Quaternion.identity, world.transform);
                        }
                    }
                    if (isRevealingTheMapSlowly)
                    {
                        numberOfCreatedTiles++;
                        if (numberOfCreatedTiles >= tilesPerFrame)
                        {
                            yield return new WaitForEndOfFrame();
                            numberOfCreatedTiles = 0;
                        }
                    }
                    
                }
            }
        }
        else
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    for (int z = 0; z < mapDepth; z++)
                    {
                        if (worldData[x, y, z] >= 0)
                        {
                            Instantiate(tilePrefabs[worldData[x, y, z]], new Vector3(x, y, z), Quaternion.identity, world.transform);
                        }
                    }                    
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
