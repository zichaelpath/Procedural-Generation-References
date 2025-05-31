# LoadPlanets.cs Documentation

This script demonstrates how to procedurally generate a solar system in Unity by reading planet data from an XML file. Each planet's properties (such as name, diameter, distance to the sun, rotation period, and orbital velocity) are defined in the XML, and the script instantiates and configures planet GameObjects at runtime.

---

## Procedural Generation Context

Procedural generation allows for dynamic and flexible content creation. By using an XML file as the data source, you can easily add, remove, or modify planets without changing the code or manually placing objects in the Unity Editor.

---

## How the Script Works

### 1. Loading Planet Data from XML

The script loads an XML file named `planets.xml` from the `Resources` folder:

```csharp
TextAsset textAsset = (TextAsset)Resources.Load("planets");
XmlDocument doc = new XmlDocument();
doc.LoadXml(textAsset.text);
foreach (XmlNode planet in doc.SelectNodes("planets/planet"))
{
    // ...existing code...
}
```

- `planets.xml` should be placed in a `Resources` folder in your Unity project.
- Each `<planet>` node in the XML defines a planet's properties as attributes.

### 2. Reading and Parsing Planet Attributes

For each planet node, the script reads its attributes and parses them into usable values:

```csharp
string name, diameter, distanceToSun, rotationPeriod, orbitalVelocity;
name = planet.Attributes.GetNamedItem("name").Value;
diameter = planet.Attributes.GetNamedItem("diameter").Value;
distanceToSun = planet.Attributes.GetNamedItem("distancetoSun").Value;
rotationPeriod = planet.Attributes.GetNamedItem("rotationPeriod").Value;
orbitalVelocity = planet.Attributes.GetNamedItem("orbitVelocity").Value;
float diameter2, distancetoSun2, rotationPeriod2, orbitalVelocity2;
diameter2 = float.Parse(diameter);
distancetoSun2 = float.Parse(distanceToSun);
rotationPeriod2 = float.Parse(rotationPeriod);
orbitalVelocity2 = float.Parse(orbitalVelocity);
```

### 3. Instantiating and Configuring Planets

The script instantiates a planet GameObject from a template and sets its properties using the parsed values:

```csharp
GameObject g = Instantiate(planetTemplate);
g.GetComponent<Planet>().SetDistanceToSun(distancetoSun2);
g.GetComponent<Planet>().setOrbitSpeed(orbitalVelocity2);
g.GetComponent<Planet>().SetRotationalSpeed(rotationPeriod2);
g.GetComponent<Planet>().SetName(name);
g.GetComponent<Planet>().SetRadius(diameter2);
```

- The `Planet` component handles the planet's movement, rotation, and appearance based on these values.

---

## How to Use

1. **Assign a Planet Template:**  
   In Unity, assign a planet prefab to the `planetTemplate` field of the `LoadPlanets` script.

2. **Create an XML File:**  
   - Place a file named `planets.xml` in a `Resources` folder.
   - Define each planet as an XML node with attributes for name, diameter, distance to sun, rotation period, and orbital velocity.
   - Example:
     ```xml
     <planets>
         <planet name="Earth" diameter="1" distancetoSun="1" rotationPeriod="1" orbitVelocity="1" />
         <planet name="Mars" diameter="0.53" distancetoSun="1.52" rotationPeriod="1.03" orbitVelocity="0.8" />
     </planets>
     ```

3. **Run the Scene:**  
   The script will generate the solar system based on the XML data at runtime.

---

## Planet.cs: How Planet Data Is Used

The `Planet` class is responsible for visualizing and animating each planet. Here are the key features and methods:

- **Orbit and Rotation:**  
  Each planet orbits around a GameObject named "Sun" and rotates on its own axis. The orbit and rotation speeds are set by the loader script.

- **Drawing the Orbit:**  
  The `DrawOrbit()` method uses a `LineRenderer` to visualize the planet's orbital path.

- **Key Methods:**  
  ```csharp
  public void SetRotationalSpeed(float s) {
      rotationSpeed = s * rotationSpeed;
  }
  public void setOrbitSpeed(float os) {
      orbitSpeed = os * orbitSpeed;
  }
  public void SetDistanceToSun(float d) {
      distanceToSun = distanceToSun * d;
  }
  public void SetName(string name) {
      this.name = name;
      transform.Find("label").GetComponent<TextMeshPro>().text = name;
  }
  public void SetRadius(float radius) {
      transform.localScale = new Vector3(radius, radius, radius);
  }
  ```

- **Update Loop:**  
  Each frame, the planet rotates and updates its position along its orbit:
  ```csharp
  void Update()
  {
      transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
      orbitAngle += Time.deltaTime * orbitSpeed;
      float tempx = sun.transform.position.x + distanceToSun * Mathf.Cos(orbitAngle);
      float tempz = sun.transform.position.z + distanceToSun * Mathf.Sin(orbitAngle);
      transform.position = new Vector3(tempx, transform.position.y, tempz);
  }
  ```

- **Labeling:**  
  The planet's name is displayed using a child object with a `TextMeshPro` component.

---

## Extending Procedural Generation

- You can generate the XML file algorithmically for random or custom solar systems.
- This approach separates data from code, making it easy to iterate on system design and add new features.

---