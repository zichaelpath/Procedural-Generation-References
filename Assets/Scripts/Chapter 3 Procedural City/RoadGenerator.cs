using UnityEngine;
using UnityEngine.Animations;

public class RoadGenerator : MonoBehaviour
{
    #region City Parameters
    public int cityWidth = 10; //Number of blocks horizontally
    public int cityLength = 10; //Number of blocks vertically
    public float roadWidth = 1f; //Width of each road segment
    public float blockSpacing = 10f; //Spacing between blocks (determines road length)
    public float blockSize = 15f; //Size of each block between roads
    private bool[,] reservedBlocks;
    #endregion

    #region Building Parameters
    public int minBuildingHeight = 3; //Minimum building height
    public int maxBuildingHeight = 10; //Maximum building height
    public float minBuildingSize = 3f; //Minimum building footprint
    public float maxBuildingSize = 7f; //Maximum building footprint
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateRoadGrid();
        //GenerateBuildings();
        reservedBlocks = new bool[cityWidth, cityLength];
        GenerateOpenSpaces();
        GenerateVariedBuildings();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateRoadGrid()
    {
        //Parent object to organize roads in the hierarchy
        GameObject roadParent = new GameObject("Roads");

        //Loop to generate horizontal roads
        for (int x = 0; x <= cityWidth; x++)
        {
            Vector3 startPosition = new Vector3(x * blockSpacing, 0, 0);
            Vector3 endPosition = new Vector3(x * blockSpacing, 0, cityLength * blockSpacing);
            CreateRoadSegment(startPosition, endPosition, roadParent);
        }

        //Loop to generate vertical roads
        for (int z = 0; z <= cityLength; z++)
        {
            Vector3 startPosition = new Vector3(0, 0, z * blockSpacing);
            Vector3 endPosition = new Vector3(cityWidth * blockSpacing, 0, z * blockSpacing);
            CreateRoadSegment(startPosition, endPosition, roadParent);
        }
    }

    private void CreateRoadSegment(Vector3 start, Vector3 end, GameObject roadParent)
    {
        //Calculate the road segment's position and rotation
        Vector3 position = (start + end) / 2;
        Quaternion rotation = Quaternion.LookRotation(end - start);

        //Create the road segment
        GameObject roadSegment = GameObject.CreatePrimitive(PrimitiveType.Cube);
        roadSegment.transform.position = position;
        roadSegment.transform.rotation = rotation;

        //Adjust the size based on road widht and the distance between start and end
        float roadLength = Vector3.Distance(start, end);
        roadSegment.transform.localScale = new Vector3(roadWidth, 0.1f, roadLength);

        //Assign a parent for organization in the hierarchy
        roadSegment.transform.parent = roadParent.transform;

        //traffic System
        roadSegment.isStatic = true;
    }

    void GenerateBuildings()
    {
        for (int x = 0; x < cityWidth; x++)
        {
            for (int z = 0; z < cityLength; z++)
            {
                //Center position of the current block, adjusted for road width
                float blockCenterX = x * blockSpacing + blockSpacing / 2;
                float blockCenterZ = z * blockSpacing + blockSpacing / 2;

                //Calculate building area by accounting for road width on all sides
                float buildingAreaWidth = blockSpacing - roadWidth * 2;
                float buildingAreaDepth = blockSpacing - roadWidth * 2;

                //Only create a building if there is enough space on the block
                if (buildingAreaWidth > minBuildingSize && buildingAreaDepth > minBuildingSize)
                {
                    //Set random dimensions fo the building within allowed limits
                    float buildingWidth = Random.Range(minBuildingSize, Mathf.Min(maxBuildingSize, buildingAreaWidth));
                    float buildingDepth = Random.Range(minBuildingSize, Mathf.Min(maxBuildingSize, buildingAreaDepth));
                    float buildingHeight = Random.Range(minBuildingHeight, maxBuildingHeight);

                    //Randomly position the building within the center of the block
                    float buildingX = Random.Range(blockCenterX - buildingAreaWidth / 2 + buildingWidth / 2, blockCenterX + buildingAreaWidth / 2 - buildingWidth / 2);
                    float buildingZ = Random.Range(blockCenterZ - buildingAreaDepth / 2 + buildingDepth / 2, blockCenterZ + buildingAreaDepth / 2 - buildingDepth / 2);

                    //Create and position the building GameObject
                    GameObject building = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    building.transform.position = new Vector3(buildingX, buildingHeight / 2, buildingZ);
                    building.transform.localScale = new Vector3(buildingWidth, buildingHeight, buildingDepth);
                    building.name = $"Building_{x}_{z}";

                    //Apply a random color to distinguish each building
                    Renderer renderer = building.GetComponent<Renderer>();
                    renderer.material.color = new Color(Random.value, Random.value, Random.value);
                }
            }
        }
    }

    void GenerateVariedBuildings()
    {
        GameObject buildingParent = new GameObject("Buildings");
        for (int x = 0; x < cityWidth; x++)
        {
            for (int z = 0; z < cityLength; z++)
            {
                if (reservedBlocks[x, z]) continue;

                //Center of the block adjusted for road width on both sides
                float blockCenterX = x * blockSpacing + roadWidth + (blockSpacing - roadWidth) / 2;
                float blockCenterZ = z * blockSpacing + roadWidth + (blockSpacing - roadWidth) / 2;

                //Generate random dimensions for each building
                float buildingWidth = Random.Range(minBuildingSize, Mathf.Min(maxBuildingSize, blockSpacing - roadWidth * 2));
                float buildingDepth = Random.Range(minBuildingSize, Mathf.Min(maxBuildingSize, blockSpacing - roadWidth * 2));
                float buildingHeight = Random.Range(minBuildingHeight, maxBuildingHeight);

                //Generate a single color for this building
                Color buildingColor = new Color(Random.value, Random.value, Random.value);

                if (Random.value < 0.7f)
                {
                    //Standard building
                    CreateBuilding(blockCenterX, blockCenterZ, buildingWidth, buildingDepth, buildingHeight, buildingParent, buildingColor);
                }
                else
                {
                    //L-shaped building with two parts
                    CreateBuilding(blockCenterX - buildingWidth / 4, blockCenterZ, buildingWidth / 2, buildingDepth, buildingHeight, buildingParent, buildingColor);
                    CreateBuilding(blockCenterX, blockCenterZ - buildingDepth / 4, buildingWidth, buildingDepth / 2, buildingHeight, buildingParent, buildingColor);
                }
            }
        }
    }

    void CreateBuilding(float x, float z, float width, float depth, float height, GameObject parent, Color color)
    {
        GameObject building = GameObject.CreatePrimitive(PrimitiveType.Cube);
        building.transform.position = new Vector3(x, height / 2, z);
        building.transform.localScale = new Vector3(width, height, depth);
        building.transform.parent = parent.transform;

        //Apply the provided color
        Renderer renderer = building.GetComponent<Renderer>();
        renderer.material.color = color;
    }

    void GenerateOpenSpaces()
    {
        int reservedCount = 0;
        GameObject openSpaceParent = new GameObject("OpenSpaces");
        for (int x = 0; x < cityWidth; x++)
        {
            for (int z = 0; z < cityLength; z++)
            {
                //Decide whether to place an open space based on a probability
                if (Random.value < 0.2f) //20% chance to create an open space
                {
                    //Mark this block as reserved for an open space
                    reservedBlocks[x, z] = true;
                    reservedCount++;
                    //Center of the block, adjusted for road width
                    float blockCenterX = x * blockSpacing + roadWidth + (blockSpacing - roadWidth) / 2;
                    float blockCenterZ = z * blockSpacing + roadWidth + (blockSpacing - roadWidth) / 2;

                    GameObject greenSpace = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    greenSpace.transform.position = new Vector3(blockCenterX, 0.05f, blockCenterZ);
                    greenSpace.transform.localScale = new Vector3(blockSpacing - roadWidth, 0.1f, blockSpacing - roadWidth);
                    greenSpace.transform.parent = openSpaceParent.transform;

                    //Apply green color to represent a part of open space
                    Renderer renderer = greenSpace.GetComponent<Renderer>();
                    renderer.material.color = new Color(0.1f, 0.8f, 0.1f);
                }
            }
        }
        Debug.Log($"Reserved Count is {reservedCount} out of {cityWidth * cityLength} possible places");
    }
}
