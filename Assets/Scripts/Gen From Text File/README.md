# GenerateMazeFromTextFile.cs Documentation

This script demonstrates how to procedurally generate a maze in Unity by reading its layout from a text file. Each line in the file represents a row in the maze, and each character represents a cell (wall or empty space).

---

## Procedural Generation Context

Procedural generation allows for dynamic and flexible content creation. By using a text file as the maze blueprint, designers can quickly modify or create new layouts without changing the code or manually placing objects in the Unity Editor.

---

## How the Script Works

### 1. Loading the Maze Layout from a Text File

The script loads a text file named `maze.txt` from the `Resources` folder:

```csharp
TextAsset t1 = (TextAsset)Resources.Load("maze", typeof(TextAsset));
string[] lines = t1.text.Split(new[]{ '\r', '\n'}, System.StringSplitOptions.RemoveEmptyEntries);
```

- `maze.txt` should be placed in a `Resources` folder in your Unity project.
- Each line in the file corresponds to a row in the maze.

### 2. Instantiating Walls Based on File Data

The script iterates through each line and character. For every `'1'` character, it instantiates a wall GameObject at the corresponding position:

```csharp
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

- The position calculation spaces the walls evenly and centers the maze in the scene.
- `'1'` represents a wall; any other character is treated as empty space.

---

## How to Use

1. **Assign a Wall Prefab:**  
   In Unity, assign a wall prefab to the `wall` field of the `GenerateMazeFromTextFile` script.

2. **Create a Maze Text File:**  
   - Place a text file named `maze.txt` in a `Resources` folder.
   - Use '1' for walls and '0' (or any other character) for empty spaces.
   - Example:
     ```
     11111
     10001
     10101
     10001
     11111
     ```

3. **Run the Scene:**  
   The script will generate the maze layout from the text file at runtime.

---

## Extending Procedural Generation

- You can generate the text file algorithmically for random or infinite mazes.
- This approach separates data from code, making it easy to iterate on level design.

---
