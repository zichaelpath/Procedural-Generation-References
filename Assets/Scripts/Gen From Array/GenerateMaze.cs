using UnityEngine;

public class GenerateMaze : MonoBehaviour
{
    public GameObject wall;
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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        #region GenerateFromArray
        /*
        // This line of code is related to loading a level from a multi-dimensional array (worldMap above)
        int i,j;
        for (i = 0; i < 10; i++)
        {
            for (j = 0; j < 10; j++)
            {
                GameObject t;
                if (worldMap[i,j] == 1)
                {
                    t = Instantiate(wall, new Vector3(50-i*10, 1.5f, 50-j*10), Quaternion.identity);
                }
            }
        }
        */
        #endregion
        #region GenerateFromTextFile
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
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
