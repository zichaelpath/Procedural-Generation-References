using UnityEngine;
using TMPro;
public class Planet : MonoBehaviour
{
    float rotationSpeed = 10f;
    float orbitSpeed = 0.2f;
    float orbitAngle = 0.0f;
    float angle = 0.0f;
    float orbitalRotationalSpeed = 20f;
    float distanceToSun = 150f;

    private Color c1 = Color.blue;
    private int lengthOfLineRenderer = 100;
    GameObject sun;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sun = GameObject.Find("Sun");
        transform.position = new Vector3(distanceToSun, 0, distanceToSun);
        DrawOrbit();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        float tempx, tempy, tempz;
        orbitAngle += Time.deltaTime * orbitSpeed;
        tempx = sun.transform.position.x + distanceToSun * Mathf.Cos(orbitAngle);
        tempz = sun.transform.position.z + distanceToSun * Mathf.Sin(orbitAngle);
        tempy = sun.transform.position.y;
        transform.position = new Vector3(tempx, transform.position.y, tempz);

    }

    void DrawOrbit()
    {
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Additive"));
        lineRenderer.startColor = c1;
        lineRenderer.startWidth = 1.0f;
        lineRenderer.positionCount = lengthOfLineRenderer + 1;

        int i = 0;
        while (i <= lengthOfLineRenderer)
        {
            float unitAngle = (float)(2 * 3.14) / lengthOfLineRenderer;
            float currentAngle = (float)unitAngle * i;
            Vector3 pos = new Vector3(distanceToSun * Mathf.Cos(currentAngle), 0, distanceToSun * Mathf.Sin(currentAngle));
            lineRenderer.SetPosition(i, pos);
            i++;
        }
    }

    public void SetRotationalSpeed(float s)
    {
        rotationSpeed = s * rotationSpeed;
    }
    public void setOrbitSpeed(float os)
    {
        orbitSpeed = os * orbitSpeed;
    }
    public void SetDistanceToSun(float d)
    {
        distanceToSun = distanceToSun * d;
    }
    public void SetName(string name)
    {
        this.name = name;
        transform.Find("label").GetComponent<TextMeshPro>().text = name;
    }
    public void SetRadius(float radius)
    {
        transform.localScale = new Vector3(radius, radius, radius);
    }
}
