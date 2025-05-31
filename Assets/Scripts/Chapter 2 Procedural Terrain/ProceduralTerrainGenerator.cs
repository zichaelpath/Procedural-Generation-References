using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Rendering;

public class Procedural : MonoBehaviour
{
    public GameObject treePrefab;
    public float maxSlope = 30f;
    public int terrainWidth = 100;
    public int terrainLength = 100;
    public int terrainHeight = 20; //Max height of the terrain
    public float noiseScale = 0.1f; //Scale of the Perlin Noise for randomness
    public int octaves = 6; //Number of noise layers
    public float persistence = 0.7f; //Controls amplitude decrease across octaves
    public float lacunarity = 2.8f; //COntrols frequency increase across octaves
    public float modificationRadius = 20f;
    public float modificationHeight = 1f;


    private Mesh terrainMesh;
    private Vector3[] vertices; //Vertices of the mesh
    private int[] triangles; //Triangles of the mesh
    private Color[] colors; //Colors for each vertex
    private MeshRenderer meshRenderer;
    private Material terrainMaterial;
    private Camera mainCamera; //Reference the main camera


    //Player controls
    private GameObject player;
    public float moveSpeed = 5f;
    public float lookSpeed = 2f;
    private float rotationX = 0;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //GenerateTerrain();
        GenerateTerrainWithOctaves();
        GenerateTrees();
        ApplyTextureLayers();

        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(terrainWidth / 2, terrainHeight + 10, terrainLength / 2);
        }
        SpawnPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();

        if (Input.GetKeyDown(KeyCode.G))
        {
            rb.useGravity = true; //enable gravity
            rb.isKinematic = false; //allow physics interactions
            rb.freezeRotation = true;
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        //player.transform.position = new Vector3(terrainWidth / 2, terrainHeight + 10, terrainLength / 2);

        MovePlayer();
        if (Input.GetKey(KeyCode.M))
        {
            ModifyTerrain(mainCamera.transform.position, modificationRadius, modificationHeight);
        }
        else if (Input.GetKey(KeyCode.N))
        {
            ModifyTerrain(mainCamera.transform.position, modificationRadius, -modificationHeight);
        }
    }

    void SpawnPlayer()
    {
        //Create the player GameObject
        player = new GameObject("Player");
        player.transform.position = new Vector3(terrainWidth / 2, terrainHeight + 10, terrainLength / 2);

        //Add a capsule collider
        CapsuleCollider collider = player.AddComponent<CapsuleCollider>();
        collider.height = 2f;

        //set camera for fps
        mainCamera.transform.parent = player.transform;
        mainCamera.transform.localPosition = new Vector3(0, 1, 0);

        Rigidbody rb = player.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    void MovePlayer()
    {
        //Mouse Controls
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f); //limit vertical rotation
        player.transform.Rotate(0, mouseX, 0);
        mainCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0); //rotate camera vertically

        //Movement
        float moveDirectionX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveDirectionZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        Vector3 move = player.transform.right * moveDirectionX + player.transform.forward * moveDirectionZ;

        player.transform.position += move;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb.isKinematic)
        {
            player.transform.position += move;
        }
        else //Grounded
        {
            rb.MovePosition(player.transform.position + move); //Use rigidbody movement for grounded mode
        }
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

    void GenerateTerrainWithOctaves()
    {
        //Create a new GameObject for the terrain
        //GameObject terrainObject = new GameObject("Procedural Terrain");
        GameObject terrainObject = new GameObject("3DTerrain");
        MeshCollider meshCollider = terrainObject.AddComponent<MeshCollider>();
        MeshFilter meshFilter = terrainObject.AddComponent<MeshFilter>();
        meshRenderer = terrainObject.AddComponent<MeshRenderer>();
        terrainObject.AddComponent<MeshCollider>();

        //Create terrain mesh
        terrainMesh = new Mesh();
        meshFilter.mesh = terrainMesh;

        //Generate vertices, triangles, and colors for terrain
        GenerateTerrainMeshWithOctaves();
        terrainMesh.vertices = vertices;
        terrainMesh.triangles = triangles;
        terrainMesh.colors = colors;

        //Recalculate normals for lighting
        terrainMesh.RecalculateNormals();

        //Create and assign materail using custom vertex color shader
        CreateVertexColorMaterial();
        meshRenderer.material = terrainMaterial; //Apply the created material
        terrainObject.GetComponent<MeshCollider>().sharedMesh = terrainMesh;
    }

    void GenerateTerrainMeshWithOctaves()
    {
        //Calculate number of vertices
        vertices = new Vector3[(terrainWidth + 1) * (terrainLength + 1)];
        colors = new Color[vertices.Length];
        triangles = new int[terrainWidth * terrainLength * 6];
        int vertexIndex = 0;
        int triangleIndex = 0;

        //Loop through the grid
        for (int z = 0; z <= terrainLength; z++)
        {
            for (int x = 0; x <= terrainWidth; x++)
            {
                float y = 0f; //Final height
                float frequency = 1f; //Initial frequency for the first octave
                float amplitude = 2f; //Initial amplitude for the first octave
                //amplitude *= 2.0f; //Increase amplitude for higher peaks
                float maxAmplitude = 1f; //Keep track of total amplitude for normalization

                //Loop through the layers (octaves)
                for (int i = 0; i < octaves; i++)
                {
                    //Generate Perlin noise for current octave
                    float perlinValue = Mathf.PerlinNoise(x * noiseScale * frequency, z * noiseScale * frequency);
                    y += perlinValue * amplitude; //Add the scaled noise to the height

                    //Prepare for next octave
                    frequency *= lacunarity; //Increase the frequency (adds more detail)
                    amplitude *= persistence; // Decrease the amplitude (prevents extreme heights)
                    maxAmplitude += amplitude; //Accumulate total amplitude for normalization
                }
                y = y / maxAmplitude * terrainHeight;

                //Apply custom terrain adjustments
                //Set height to 0 for water areas and color them blue
                if (y < 0)
                {
                    y = 0; //Flatten to 0 for water level
                    colors[vertexIndex] = Color.blue; //blue for water
                }
                else
                {
                    //otherwise calculate the color based on height
                    colors[vertexIndex] = CalculateColor(y);
                }

                //Create the vertex at this position after adjustments
                vertices[vertexIndex] = new Vector3(x, y, z);

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

    void ModifyTerrain(Vector3 center, float radius, float heightAdjustment)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            float distance = Vector3.Distance(vertices[i], new Vector3(center.x, vertices[i].y, center.z));

            if (distance < radius)
            {
                vertices[i].y += heightAdjustment * (1 - (distance / radius));
                colors[i] = CalculateColor(vertices[i].y);
            }
        }
        terrainMesh.vertices = vertices;
        terrainMesh.colors = colors;
        terrainMesh.RecalculateNormals();

        MeshCollider collider = GameObject.Find("3DTerrain").GetComponent<MeshCollider>();
        if (collider != null)
        {
            collider.sharedMesh = terrainMesh;
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
                        if (treePosition.y > (terrainHeight * 0.4) && treePosition.y < (terrainHeight * 0.25f))
                        {
                            continue;
                        }
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
