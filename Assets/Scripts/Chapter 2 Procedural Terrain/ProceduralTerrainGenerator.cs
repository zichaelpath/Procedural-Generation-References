using Unity.VisualScripting;
using UnityEngine;

public class Procedural : MonoBehaviour
{
    public GameObject treePrefab;
    public float maxSlope = 30f;
    public int terrainWidth = 100;
    public int terrainLength = 100;
    public int terrainHeight = 20; //Max height of the terrain
    public float noiseScale = 0.1f; //Scale of the Perlin Noise for randomness
    


    private Mesh terrainMesh;
    private Vector3[] vertices; //Vertices of the mesh
    private int[] triangles; //Triangles of the mesh
    private Color[] colors; //Colors for each vertex
    private MeshRenderer meshRenderer;
    private Material terrainMaterial;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateTerrain();
        ApplyTextureLayers();
        GenerateTrees();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateTerrain()
    {
        //Create a new GameObject for the terrain
        GameObject terrainObject = new GameObject("Procedural Terrain");
        MeshFilter meshFilter = terrainObject.AddComponent<MeshFilter>();
        meshRenderer = terrainObject.AddComponent<MeshRenderer>();
        terrainObject.AddComponent<MeshCollider>();
        terrainMesh = new Mesh();
        meshFilter.mesh = terrainMesh;

        //Generate the vertices, triangles, and colors of the terrain
        GenerateTerrainMesh();

        //Applying generated data to the mesh
        terrainMesh.vertices = vertices;
        terrainMesh.triangles = triangles;
        terrainMesh.colors = colors;

        //Recalculate normals for lighting
        terrainMesh.RecalculateNormals();

        //Create and assign the material using the custom vertex color shader
        CreateVertexColorMaterial();
        meshRenderer.material = terrainMaterial;
    }

    void GenerateTerrainMesh()
    {
        //Calculate the number of vertices
        vertices = new Vector3[(terrainWidth + 1) * (terrainLength + 1)];
        colors = new Color[vertices.Length];
        triangles = new int[terrainWidth * terrainLength * 6]; //6 vertices per quad (2 triangles)
        int vertexIndex = 0;
        int triangleIndex = 0;
        for (int z = 0; z <= terrainLength; z++)
        {
            for (int x = 0; x <= terrainWidth; x++)
            {
                //Calculate the height point (y) of the vertex
                float y = Mathf.PerlinNoise(x * noiseScale, z * noiseScale) * terrainHeight;

                //Create the vertex at this position
                vertices[vertexIndex] = new Vector3(x, y, z);

                //Color based on height (y)
                colors[vertexIndex] = CalculateColor(y);

                //Generate triangles for every grid square except for the last row/column
                if (x < terrainWidth && z < terrainLength)
                {
                    //First triangle (bottom-left to top-right)
                    triangles[triangleIndex + 0] = vertexIndex;
                    triangles[triangleIndex + 1] = vertexIndex + terrainWidth + 1;
                    triangles[triangleIndex + 2] = vertexIndex + 1;

                    //Second triangle (top-left to bottom-right)
                    triangles[triangleIndex + 3] = vertexIndex + 1;
                    triangles[triangleIndex + 4] = vertexIndex + terrainWidth + 1;
                    triangles[triangleIndex + 5] = vertexIndex + terrainWidth + 2;

                    triangleIndex += 6;
                }
                vertexIndex++;
            }
        }
    }

    void CreateVertexColorMaterial()
    {
        //Create a new material using the custom vertex color shader
        terrainMaterial = new Material(Shader.Find("Custom/VertexColorShader"));
    }

    Color CalculateColor(float height)
    {
        if (height < terrainHeight * 0.25f) //Water Level
        {
            return Color.blue;
        }
        else if (height < terrainHeight * 0.4f) //Grass Level
        {
            return Color.green;
        }
        else if (height < terrainHeight * 0.6f) //Grass Level
        {
            return new Color(0.55f, 0.27f, 0.07f);
        }
        else if (height < terrainHeight * 0.85f) //Rock Level
        {
            return new Color(0.5f, 0.5f, 0.5f);
        }
        else //Snow Level (Mountain Tops)
        {
            return Color.white;
        }
    }

    void GenerateTrees()
    {
        for (int z = 0; z < terrainLength; z++)
        {
            for (int x = 0; x < terrainWidth; x++)
            {
                int currentVertexIndex = x + z * (terrainWidth + 1);
                int nextXVertexIndex = (x + 1) + z * (terrainWidth + 1);
                int nextZVertexIndex = x + (z + 1) * (terrainWidth + 1);

                if (x < terrainWidth && z < terrainLength)
                {
                    //Get the height difference between adjacent vertices
                    float slopeX = Mathf.Abs(vertices[currentVertexIndex].y - vertices[nextXVertexIndex].y);
                    float slopeZ = Mathf.Abs(vertices[currentVertexIndex].y - vertices[nextZVertexIndex].y);

                    //Calculate slope based on the height difference and distance between vertices
                    float slope = Mathf.Max(slopeX, slopeZ);
                    //Convert slope to degrees
                    float slopeDegrees = Mathf.Atan(slope / 1f) * Mathf.Rad2Deg;

                    if (slopeDegrees < maxSlope)
                    {
                        Vector3 treePosition = vertices[currentVertexIndex];
                        GameObject tree = Instantiate(treePrefab, treePosition, Quaternion.identity);
                        tree.transform.position = new Vector3(treePosition.x, treePosition.y, treePosition.z);
                        Debug.Log($"Tree placed at {treePosition}");
                    }
                }
            }
        }
    }

    void ApplyTextureLayers()
    {
        //Ensure we have a material for the terrain
        if (terrainMaterial == null)
        {
            terrainMaterial = new Material(Shader.Find("Custom/TerrainTextureShader"));
            meshRenderer.material = terrainMaterial;
        }

        //Iterate through vertices to apply textures based on height
        for (int i = 0; i < vertices.Length; i++)
        {
            float height = vertices[i].y / terrainHeight;

            //Choose texture layers based on the height of the terrain
            if (height < 0.3f)
            {
                terrainMaterial.SetFloat("_BlendGrass", 0f);
                terrainMaterial.SetFloat("_BlendDirt", 1f); // Only dirt at lower levels
                terrainMaterial.SetFloat("_BlendRock", 0f);
            }
            else if (height < 0.5f)
            {
                terrainMaterial.SetFloat("_BlendGrass", 1f); // Mostly grass
                terrainMaterial.SetFloat("_BlendDirt", 0.2f); // Some dirt
                terrainMaterial.SetFloat("_BlendRock", 0f); // No Rocks
            }
            else if (height < 0.75f)
            {
                terrainMaterial.SetFloat("_BlendGrass", 0.3f); // Some grass
                terrainMaterial.SetFloat("_BlendDirt", 0.4f); // Dirt for mid-height
                terrainMaterial.SetFloat("_BlendRock", 1f); // Rocks start showing
            }
            else
            {
                terrainMaterial.SetFloat("_BlendGrass", 0f); // No grass
                terrainMaterial.SetFloat("_BlendDirt", 0.3f); // Dirt transitions
                terrainMaterial.SetFloat("_BlendRock", 1f); // Rocks Dominate
            }
        }
    }
}
