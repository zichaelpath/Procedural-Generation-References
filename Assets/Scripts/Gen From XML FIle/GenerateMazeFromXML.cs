using UnityEngine;
using System.Xml;
public class GenerateMazeFromXML : MonoBehaviour
{
    public GameObject wall;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TextAsset textAsset = (TextAsset)Resources.Load("scene");
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(textAsset.text);

        foreach (XmlNode level in doc.SelectNodes("game/level"))
        {
            if (level.Attributes.GetNamedItem("number").Value == "1")
            {
                foreach (XmlNode gameObject in level.SelectNodes(".//object"))
                {
                    string name, location;
                    name = gameObject.Attributes.GetNamedItem("name").Value;
                    location = gameObject.Attributes.GetNamedItem("location").Value;
                    Vector3 v = ConvertStringToVector(location);
                    GameObject g = Instantiate(wall, v, Quaternion.identity);
                }
            }
        }
    }

    Vector3 ConvertStringToVector(string s)
    {
        string[] newString;
        newString = s.Split(new char[] {','});
        float x, y, z;
        x = float.Parse(newString[0]);
        y = float.Parse(newString[1]);
        z = float.Parse(newString[2]);
        return new Vector3(x, y, z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
