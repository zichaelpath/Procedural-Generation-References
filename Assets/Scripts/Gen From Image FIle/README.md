# GenerateFromImage.cs Documentation

This script demonstrates how to procedurally generate a level layout in Unity by reading pixel data from an image file. Each non-white pixel in the image is interpreted as a wall, and a wall GameObject is instantiated at the corresponding position in the scene.

---

## Procedural Generation Context

Procedural generation allows for dynamic and flexible content creation. By using an image as a blueprint, designers can quickly create or modify level layouts visually, and the script will automatically generate the corresponding 3D environment at runtime.

---

## How the Script Works

### 1. Reading Pixel Data

The script uses a `Texture2D` image (`outlineImage`) as the source for the level layout. It creates a 2D array to store the color of each pixel:

```csharp
colorOfPixel = new Color[outlineImage.width, outlineImage.height];
for (int x = 0; x < outlineImage.width; x++)
{
    for (int y = 0; y < outlineImage.height; y++)
    {
        colorOfPixel[x, y] = outlineImage.GetPixel(x, y);
        //Check transparency
        if (colorOfPixel[x, y] != Color.white)
        {
            GameObject t = Instantiate(wall, new Vector3((outlineImage.width / 2 * 10) - x * 10, 1.5f, (outlineImage.height / 2 * 10) - y * 10), Quaternion.identity);
        }
    }
}
```

### 2. Instantiating Walls Based on Pixel Color

For each pixel, the script checks if the color is not white (`Color.white`). If so, it instantiates a wall GameObject at a calculated position:

```csharp
if (colorOfPixel[x, y] != Color.white)
{
    GameObject t = Instantiate(
        wall,
        new Vector3((outlineImage.width / 2 * 10) - x * 10, 1.5f, (outlineImage.height / 2 * 10) - y * 10),
        Quaternion.identity
    );
}
```

- The position calculation centers the generated layout in the scene and spaces the walls evenly.
- This approach allows for easy editing: simply change the image to change the level.

---

## How to Use

1. **Assign a Wall Prefab:**  
   In Unity, assign a wall prefab to the `wall` field of the `GenerateFromImage` script.

2. **Assign an Outline Image:**  
   Assign a black-and-white (or color) image to the `outlineImage` field. Non-white pixels will become walls.

3. **Run the Scene:**  
   The script will generate the level layout based on the image at runtime.

---

## Extending Procedural Generation

- You can use different colors to represent different types of objects or terrain.
- This method enables rapid prototyping and visual design of levels, leveraging procedural generation for flexibility and speed.

---