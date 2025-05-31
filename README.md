# Unity Procedural Generation Examples

Welcome! This project demonstrates several approaches to procedural generation in Unity, with practical scripts and documentation for each method. Explore how to generate content dynamically using arrays, text files, images, and XML data.

---

## Project Overview

Procedural generation is a powerful technique for creating dynamic, varied, and scalable content in games and simulations. This repository provides hands-on examples for different data-driven and algorithmic approaches, each with clear documentation and ready-to-use Unity scripts.

---

## Project Structure & Topics

### 1. Maze Generation from Arrays
- **What you'll learn:**  
  How to use a hardcoded 2D array to define a maze layout, and instantiate wall GameObjects based on array values.
- **Key script:**  
  `GenerateMaze.cs` in `Assets/Scripts/Gen From Array/`
- **How it works:**  
  The script loops through a 2D array (`worldMap`), placing a wall prefab wherever a `1` appears. This is ideal for prototyping or fixed layouts.
- **More details:**  
  See [Gen From Array/README.md](Assets/Scripts/Gen%20From%20Array/README.md)

### 2. Maze Generation from Text Files
- **What you'll learn:**  
  How to load a maze layout from a text file, allowing designers to edit levels without touching code.
- **Key script:**  
  `GenerateMazeFromTextFile.cs` in `Assets/Scripts/Gen From Text File/`
- **How it works:**  
  The script reads a text file (`maze.txt`) from the `Resources` folder, where each line represents a row and each character a cell. `'1'` means wall, `'0'` means empty.
- **More details:**  
  See [Gen From Text File/README.md](Assets/Scripts/Gen%20From%20Text%20File/README.md)

### 3. Level Generation from Images
- **What you'll learn:**  
  How to use an image as a blueprint for level generation, translating pixel data into 3D objects.
- **Key script:**  
  `GenerateFromImage.cs` in `Assets/Scripts/Gen From Image FIle/`
- **How it works:**  
  The script reads a `Texture2D` image, and for each non-white pixel, instantiates a wall prefab at the corresponding position. This enables visual level design using any image editor.
- **More details:**  
  See [Gen From Image File/README.md](Assets/Scripts/Gen%20From%20Image%20FIle/README.md)

### 4. Solar System Generation from XML
- **What you'll learn:**  
  How to procedurally generate a solar system by reading planet data from an XML file, and how to use component scripts to animate and display planets.
- **Key scripts:**  
  - `LoadPlanets.cs` (loads and instantiates planets from XML)
  - `Planet.cs` (controls planet behavior and appearance)
  - Both in `Assets/Scripts/Gen From XML FIle/`
- **How it works:**  
  The XML file (`planets.xml`) defines each planet's properties (name, diameter, distance to sun, rotation period, orbital velocity). The loader script reads these and creates planet GameObjects, while the `Planet` script animates their orbits and rotations.
- **More details:**  
  See [Gen From XML File/README.md](Assets/Scripts/Gen%20From%20XML%20FIle/README.md)

---

## Educational Goals & Applications

- **Understand core procedural generation techniques** for Unity, including data-driven and algorithmic approaches.
- **Learn how to separate data from code** for flexible, scalable content creation.
- **Gain practical experience** with Unity scripting, prefab instantiation, and working with external data sources (arrays, text, images, XML).
- **Apply these patterns** to your own games, simulations, or creative projects.

---

## How to Use

- Each folder in `Assets/Scripts` contains a Unity script and a README with setup instructions, code explanations, and sample data formats.
- Prefabs and data files (such as images, text files, or XML) should be placed in the appropriate `Resources` folder as described in each topic's README.
- Open the Unity project, assign the required prefabs, and run the scene to see procedural generation in action.

---

## Extending This Project

- All examples are designed to be extensible. You can modify the data sources or algorithms to create more complex or randomized content.
- Try combining techniques (e.g., generate a maze from an image, then export it as text or XML).
- For more advanced procedural generation, consider integrating maze/level generation algorithms, noise functions, or runtime data editing.

---

For detailed instructions, code samples, and further reading, refer to the README in each topic folder.
