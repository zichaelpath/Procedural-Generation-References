using System;
using Unity.VisualScripting;
using UnityEngine;

public class GenerateHeightMap : MonoBehaviour
{
    float[,] map;
    [SerializeField][Range(10, 100)] int mapHeight, mapWidth;
    [SerializeField][Range(0, 100)] float blockSize, blockHeight, frequency, scale;
    public GameObject minecraftBlock;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        map = new float[mapWidth, mapHeight];
        minecraftBlock.transform.localScale = new Vector3(blockSize, blockHeight, blockSize);
        initArray();
        displayArray();
    }

    private void displayArray()
    {
        for (int j = 0; j < mapHeight; j++)
        {
            for (int i = 0; i < mapWidth; i++)
            {
                GameObject t = (GameObject)(Instantiate(minecraftBlock, new Vector3(i * blockSize, Mathf.Round(map[i, j] * blockHeight * scale), j * blockSize), Quaternion.identity));
            }
        }
    }

    void initArray()
    {
        map = new float[mapWidth, mapHeight];
        for (int j = 0; j < mapHeight; j++)
        {
            for (int i = 0; i < mapWidth; i++)
            {
                float nx = i / mapWidth;
                float ny = j / mapHeight;
                map[i, j] = Mathf.PerlinNoise(i * 1.0f / frequency + 0.1f, j * 1.0f / frequency + 0.1f);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
