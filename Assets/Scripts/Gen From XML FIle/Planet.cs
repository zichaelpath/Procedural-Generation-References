using UnityEngine;

public class Planet : MonoBehaviour
{
    float rotationSpeed = 10f;
    float orbitSpeed = 0.2f;
    float orbitAngle = 0.0f;
    float angle = 0.0f;
    float orbitalRotationalSpeed = 20f;
    float distanceToSun = 150f;
    GameObject sun;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sun = GameObject.Find("Sun");
        transform.position = new Vector3(distanceToSun, 0, distanceToSun);
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
}
