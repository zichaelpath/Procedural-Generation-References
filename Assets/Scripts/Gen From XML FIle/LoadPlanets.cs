using UnityEngine;
using System.Xml;
public class LoadPlanets : MonoBehaviour
{
    public GameObject planetTemplate;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadAllPlanets();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LoadAllPlanets()
    {
        TextAsset textAsset = (TextAsset)Resources.Load("planets");
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(textAsset.text);
        foreach (XmlNode planet in doc.SelectNodes("planets/planet"))
        {
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
            Debug.Log("Planet" + name + ": Diameter" + diameter2 + ";Distance" + distancetoSun2);
            GameObject g = Instantiate(planetTemplate);
            g.GetComponent<Planet>().SetDistanceToSun(distancetoSun2);
            g.GetComponent<Planet>().setOrbitSpeed(orbitalVelocity2);
            g.GetComponent<Planet>().SetRotationalSpeed(rotationPeriod2);
            g.GetComponent<Planet>().SetName(name);
            g.GetComponent<Planet>().SetRadius(diameter2);
        }
    }
}
