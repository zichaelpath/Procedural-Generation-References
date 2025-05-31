# GenerateMaze.cs Documentation

This script demonstrates two approaches to procedural maze generation in Unity: generating a maze from a hardcoded 2D array and from a text file. Both methods instantiate wall GameObjects to build the maze layout at runtime.

---

## Procedural Generation Context

Procedural generation is a technique where content is created algorithmically, allowing for dynamic and varied environments. In this script, maze layouts are generated at runtime based on data structures, rather than being manually placed in the Unity Editor.

---

## 1. Maze Generation from a 2D Array

The script defines a 10x10 integer array, where `1` represents a wall and `0` represents empty space:

```csharp
// ...existing code...
private int [,] worldMap = new int[,]
{
    {1,1,1,1,1,1,1,1,1,1},
    {1,0,1,0,0,0,0,0,0,1},
    {1,0,1,0,1,0,1,0,0,1},
    {1,0,1,0,0,0,0,0,0,1},
    {1,0,1,1,1,1,0,0,0,1},
    {1,0,0,0,0,0,0,0,0,1},
    {1,0,1,0,1,0,1,1,1,1},
    {1,0,0,1,0,0,0,0,0,1},
    {1,0,1,0,0,0,0,0,0,1},
    {1,1,1,1,1,1,1,1,1,1}
};
// ...existing code...
```

In the `Start()` method, the script loops through the array and instantiates a wall prefab for each `1`:

```csharp
for (i = 0; i < 10; i++)
{
    for (j = 0; j < 10; j++)
    {
        if (worldMap[i,j] == 1)
        {
            Instantiate(wall, new Vector3(50-i*10, 1.5f, 50-j*10), Quaternion.identity);
        }
    }
}
```

**Explanation:**  
This approach is useful for prototyping or fixed layouts. The maze structure is hardcoded, so it remains the same unless the array is changed.

---

## 2. Maze Generation from a Text File

The script can also load a maze layout from a text file placed in the `Resources` folder:

```csharp
TextAsset t1 = (TextAsset)Resources.Load("maze", typeof(TextAsset));
string[] lines = t1.text.Split(new[]{ '\r', '\n'}, System.StringSplitOptions.RemoveEmptyEntries);
for (int row = 0; row < lines.Length; row++)
{
    string line = lines[row].Trim();
    for (int col = 0; col < line.Length; col++)
    {
        if (line[col] == '1')
        {
            Vector3 pos = new Vector3(50 - col * 10, 1.5f, 50 - row * 10);
            Instantiate(wall, pos, Quaternion.identity);
        }
    }
}
```

**Explanation:**  
This method allows designers to easily modify the maze by editing a text file, making it more flexible and data-driven. Each line in the file represents a row, and each character represents a cell (`'1'` for wall, `'0'` for empty).

---

## How to Use

1. **Assign a Wall Prefab:**  
   In Unity, assign a wall prefab to the `wall` field of the `GenerateMaze` script.

2. **Using the Array:**  
   The maze defined in `worldMap` will be generated automatically at runtime.

3. **Using a Text File:**  
   - Place a text file named `maze.txt` in a `Resources` folder.
   - Use '1' for walls and '0' for empty spaces.
   - The script will read and generate the maze from this file.

---

## Extending Procedural Generation

While this script uses static data, you can extend it by generating the array or text file algorithmically (e.g., using maze generation algorithms) for infinite or randomized mazes.

---