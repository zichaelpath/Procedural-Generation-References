using UnityEngine;

public class GenerateFromImage : MonoBehaviour
{
    Color[,] colorOfPixel;
    public GameObject wall;
    public Texture2D outlineImage;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        #region GenerateFromImageFile
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
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
