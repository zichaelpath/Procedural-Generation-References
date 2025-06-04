# ProceduralTerrainGenerator.cs Documentation

This script (`ProceduralTerrainGenerator.cs`) demonstrates how to generate, modify, and interact with procedural terrain in Unity. It covers mesh generation using Perlin noise (with octaves), terrain type customization, real-time terrain modification, player controls, and dynamic coloring/texturing. The script is designed for extensibility and experimentation with procedural environments.

---

## Features Overview

- **Procedural Terrain Generation:**  
  Generates a mesh-based terrain using Perlin noise, with support for multiple octaves and terrain types (Desert, Mountainous, Countryside, Rocky, Lake).
- **Player Controls:**  
  First-person movement, mouse look, and toggling between kinematic and physics-based movement.
- **Real-Time Terrain Modification:**  
  Raise or lower terrain in a radius around the camera using keyboard input.
- **Dynamic Coloring and Texturing:**  
  Vertex colors and material blending based on height and terrain type.
- **Tree Placement (Commented Out):**  
  Example logic for placing trees based on slope and height.
- **Custom Shaders:**  
  Designed to work with custom vertex color and terrain texture shaders.

---

## Script Structure

### Class: `Procedural`

#### Public Fields

- `TerrainType terrainType`  
  Enum to select the terrain style (Desert, Mountainous, Countryside, Rocky, Lake).
- `GameObject treePrefab`  
  Prefab for tree placement (used in `GenerateTrees`).
- `float maxSlope`  
  Maximum slope for tree placement.
- `int terrainWidth, terrainLength, terrainHeight`  
  Dimensions and max height of the terrain mesh.
- `float noiseScale`  
  Scale of Perlin noise for terrain randomness.
- `int octaves`  
  Number of Perlin noise layers for fractal detail.
- `float persistence, lacunarity`  
  Controls amplitude and frequency changes across octaves.
- `float modificationRadius, modificationHeight`  
  Parameters for real-time terrain modification.
- `float moveSpeed, lookSpeed`  
  Player movement and look sensitivity.

#### Private Fields

- Mesh and mesh data (`terrainMesh`, `vertices`, `triangles`, `colors`)
- Rendering and camera references (`meshRenderer`, `terrainMaterial`, `mainCamera`)
- Player GameObject and control state

---

## Main Methods

### `void Start()`
- Generates the terrain mesh with octaves and applies textures.
- Positions the camera above the terrain.
- Spawns the player and sets up first-person controls.

**Sample:**
```csharp
void Start()
{
    //GenerateTerrain();
    GenerateTerrainWithOctaves();
    //GenerateTrees();
    ApplyTextureLayers();

    mainCamera = Camera.main;
    if (mainCamera != null)
    {
        mainCamera.transform.position = new Vector3(terrainWidth / 2, terrainHeight + 10, terrainLength / 2);
    }
    SpawnPlayer();
}
```

### `void Update()`
- Handles player movement and mouse look.
- Toggles player physics with `G` (enable gravity) and `B` (disable gravity).
- Modifies terrain in real-time with `M` (raise) and `N` (lower) keys.

**Sample:**
```csharp
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
```

### `void SpawnPlayer()`
- Instantiates a player GameObject with a capsule collider and Rigidbody.
- Parents the main camera to the player for first-person view.

**Sample:**
```csharp
void SpawnPlayer()
{
    player = new GameObject("Player");
    player.transform.position = new Vector3(terrainWidth / 2, terrainHeight + 10, terrainLength / 2);

    CapsuleCollider collider = player.AddComponent<CapsuleCollider>();
    collider.height = 2f;

    mainCamera.transform.parent = player.transform;
    mainCamera.transform.localPosition = new Vector3(0, 1, 0);

    Rigidbody rb = player.AddComponent<Rigidbody>();
    rb.useGravity = false;
    rb.isKinematic = true;
}
```

### `void MovePlayer()`
- Implements first-person movement and mouse look.
- Switches between kinematic and physics-based movement depending on Rigidbody state.

**Sample:**
```csharp
void MovePlayer()
{
    float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
    float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;
    rotationX -= mouseY;
    rotationX = Mathf.Clamp(rotationX, -90f, 90f);
    player.transform.Rotate(0, mouseX, 0);
    mainCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

    float moveDirectionX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
    float moveDirectionZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
    Vector3 move = player.transform.right * moveDirectionX + player.transform.forward * moveDirectionZ;

    player.transform.position += move;

    Rigidbody rb = player.GetComponent<Rigidbody>();
    if (rb.isKinematic)
    {
        player.transform.position += move;
    }
    else
    {
        rb.MovePosition(player.transform.position + move);
    }
}
```

### `void GenerateTerrain()` *(Commented Out in Start)*
- Generates a simple Perlin noise terrain mesh (single octave).
- Assigns vertex colors based on height.

**Sample:**
```csharp
void GenerateTerrain()
{
    GameObject terrainObject = new GameObject("Procedural Terrain");
    MeshFilter meshFilter = terrainObject.AddComponent<MeshFilter>();
    meshRenderer = terrainObject.AddComponent<MeshRenderer>();
    terrainObject.AddComponent<MeshCollider>();
    terrainMesh = new Mesh();
    meshFilter.mesh = terrainMesh;

    GenerateTerrainMesh();

    terrainMesh.vertices = vertices;
    terrainMesh.triangles = triangles;
    terrainMesh.colors = colors;

    terrainMesh.RecalculateNormals();

    CreateVertexColorMaterial();
    meshRenderer.material = terrainMaterial;
}
```

### `void GenerateTerrainWithOctaves()`
- Generates a terrain mesh using multiple Perlin noise octaves for fractal detail.
- Adjusts height and color based on selected `TerrainType`.
- Handles special cases for lakes, countryside rivers, and deserts.

**Sample:**
```csharp
void GenerateTerrainWithOctaves()
{
    GameObject terrainObject = new GameObject("3DTerrain");
    MeshCollider meshCollider = terrainObject.AddComponent<MeshCollider>();
    MeshFilter meshFilter = terrainObject.AddComponent<MeshFilter>();
    meshRenderer = terrainObject.AddComponent<MeshRenderer>();
    terrainObject.AddComponent<MeshCollider>();

    terrainMesh = new Mesh();
    meshFilter.mesh = terrainMesh;

    GenerateTerrainMeshWithOctaves();
    terrainMesh.vertices = vertices;
    terrainMesh.triangles = triangles;
    terrainMesh.colors = colors;

    terrainMesh.RecalculateNormals();

    CreateVertexColorMaterial();
    meshRenderer.material = terrainMaterial;
    terrainObject.GetComponent<MeshCollider>().sharedMesh = terrainMesh;
}
```

### `void GenerateTerrainMeshWithOctaves()`
- Core mesh generation logic for octaved Perlin noise.
- Assigns vertex positions and colors, and builds triangle indices.

**Sample:**
```csharp
void GenerateTerrainMeshWithOctaves()
{
    // ...existing code...
    for (int z = 0; z <= terrainLength; z++)
    {
        for (int x = 0; x <= terrainWidth; x++)
        {
            float y = 0f;
            float frequency = 1f;
            float amplitude = 2f;
            float maxAmplitude = 1f;

            for (int i = 0; i < octaves; i++)
            {
                float perlinValue = Mathf.PerlinNoise(x * noiseScale * frequency, z * noiseScale * frequency);
                y += perlinValue * amplitude;
                frequency *= lacunarity;
                amplitude *= persistence;
                maxAmplitude += amplitude;
            }
            y = y / maxAmplitude * terrainHeight;

            // Terrain type adjustments
            if (terrainType == TerrainType.Lake && y < 2)
            {
                y = -5f;
            }
            // ...other terrain type logic...

            // Set color
            if (y < 0)
            {
                y = 0;
                colors[vertexIndex] = Color.blue;
            }
            else
            {
                colors[vertexIndex] = CalculateColor(y);
            }

            vertices[vertexIndex] = new Vector3(x, y, z);

            // ...triangle generation...
        }
    }
}
```

### `void ModifyTerrain(Vector3 center, float radius, float heightAdjustment)`
- Raises or lowers terrain vertices within a given radius of a point (typically the camera).
- Updates mesh and collider after modification.

**Sample:**
```csharp
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
```

### `void CreateVertexColorMaterial()`
- Creates a material using a custom vertex color shader.

**Sample:**
```csharp
void CreateVertexColorMaterial()
{
    terrainMaterial = new Material(Shader.Find("Custom/VertexColorShader"));
}
```

### `Color CalculateColor(float height)`
- Returns a color for a vertex based on its height and the selected terrain type.
- Handles special coloring for water, grass, rocks, snow, etc.

**Sample:**
```csharp
Color CalculateColor(float height)
{
    switch (terrainType)
    {
        case TerrainType.Desert:
            return height < 1f ? new Color(1f, 0.9f, 0.5f) : new Color(0.9f, 0.8f, 0.4f);
        case TerrainType.Mountainous:
            if (height < terrainHeight * 0.3f) return Color.green;
            if (height < terrainHeight * 0.7f) return new Color(0.6f, 0.5f, 0.4f);
            return Color.white;
        // ...other cases...
        default:
            return Color.green;
    }
}
```

### `void GenerateTrees()` *(Commented Out in Start)*
- Example logic for placing tree prefabs on the terrain.
- Avoids steep slopes and certain height ranges.

**Sample:**
```csharp
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
                float slopeX = Mathf.Abs(vertices[currentVertexIndex].y - vertices[nextXVertexIndex].y);
                float slopeZ = Mathf.Abs(vertices[currentVertexIndex].y - vertices[nextZVertexIndex].y);
                float slope = Mathf.Max(slopeX, slopeZ);
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
                }
            }
        }
    }
}
```

### `void ApplyTextureLayers()`
- Sets material blend values for grass, dirt, and rock textures based on vertex height.
- Requires a custom terrain texture shader.

**Sample:**
```csharp
void ApplyTextureLayers()
{
    if (terrainMaterial == null)
    {
        terrainMaterial = new Material(Shader.Find("Custom/TerrainTextureShader"));
        meshRenderer.material = terrainMaterial;
    }

    for (int i = 0; i < vertices.Length; i++)
    {
        float height = vertices[i].y / terrainHeight;
        if (height < 0.3f)
        {
            terrainMaterial.SetFloat("_BlendGrass", 0f);
            terrainMaterial.SetFloat("_BlendDirt", 1f);
            terrainMaterial.SetFloat("_BlendRock", 0f);
        }
        else if (height < 0.5f)
        {
            terrainMaterial.SetFloat("_BlendGrass", 1f);
            terrainMaterial.SetFloat("_BlendDirt", 0.2f);
            terrainMaterial.SetFloat("_BlendRock", 0f);
        }
        else if (height < 0.75f)
        {
            terrainMaterial.SetFloat("_BlendGrass", 0.3f);
            terrainMaterial.SetFloat("_BlendDirt", 0.4f);
            terrainMaterial.SetFloat("_BlendRock", 1f);
        }
        else
        {
            terrainMaterial.SetFloat("_BlendGrass", 0f);
            terrainMaterial.SetFloat("_BlendDirt", 0.3f);
            terrainMaterial.SetFloat("_BlendRock", 1f);
        }
    }
}
```

---

## Controls

- **WASD / Arrow Keys:** Move player.
- **Mouse:** Look around (first-person).
- **G:** Enable gravity and physics for the player.
- **B:** Disable gravity (kinematic mode).
- **M:** Raise terrain under the camera.
- **N:** Lower terrain under the camera.

---

## Customization

- **Terrain Types:**  
  Change the `terrainType` enum in the inspector to switch between different terrain styles.
- **Noise Parameters:**  
  Adjust `noiseScale`, `octaves`, `persistence`, and `lacunarity` for different terrain roughness and detail.
- **Tree Placement:**  
  Uncomment `GenerateTrees()` in `Start()` and assign a tree prefab to enable tree generation.
- **Shaders:**  
  Requires custom shaders named `Custom/VertexColorShader` and `Custom/TerrainTextureShader` for full visual effect.

---

## Extending

- Add more terrain types or biome logic in `CalculateColor`.
- Integrate procedural object placement (rocks, lakes, etc.).
- Implement runtime saving/loading of terrain modifications.
- Expand player controls for jumping, crouching, or flying.

---

## Example Usage

1. Attach the script to an empty GameObject in your Unity scene.
2. Assign a tree prefab (optional) and set terrain parameters in the inspector.
3. Ensure required shaders are present in your project.
4. Play the scene and use the controls to explore and modify the terrain.

---

## Notes

- The script is designed for experimentation and learning.  
- Some features (tree placement, single-octave terrain) are included as commented-out code for reference.
- For best results, use with the provided custom shaders and adjust parameters to suit your project.

---
