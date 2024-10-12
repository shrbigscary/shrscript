using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using System.IO;

public class LabGenerator : MonoBehaviour
{
    [Header("This script was made by Keo.cs")]
    public int labWidth = 10;
    public int labHeight = 10;
    public float roomSize = 5f;
    public float wallHeight = 2.5f;
    public float wallThickness = 0.2f;
    public Material floorMaterial;
    public Material wallMaterial;
    public Material altWallMaterial;
    public string altWallLayer;
    public Material roofMaterial;
    public bool randomSeed = true;
    public int specifiedSeed = 0;
    public Transform floorParent;
    public Transform wallParent;
    public Transform roofParent;
    public Transform propsParent;
    public Transform lampParent;
    public GameObject lampPrefab;
    public int lampFrequency = 1;
    public bool useAltWalls = true;
    public bool generateEmptyRooms = false;
    public int emptyRoomProbability = 25;
    public List<RandomItem> randomItems;

    public float lampOffset = 0.5f; //Lamp offset cause it was absolute garbige for me ;)

    [System.Serializable]
    public class RandomItem
    {
        public GameObject itemPrefab;
        public Material itemMaterial;
    }

    private int currentSeed;

    private void Start()
    {
        if (Application.isPlaying)
        {
            CreateMaze();
        }
    }

    public void CreateMaze()
    {
        DeleteOldMaze();
        GenerateMaze();
        ScatterRandomItems();
    }

    public void DeleteOldMaze()
    {
        DestroyOldLab();
    }

    public void AutoSetup()
    {
        GameObject lab = new GameObject("Lab");

        GameObject floorParentObj = new GameObject("Floor");
        floorParentObj.transform.parent = lab.transform;
        floorParent = floorParentObj.transform;

        GameObject wallParentObj = new GameObject("Walls");
        wallParentObj.transform.parent = lab.transform;
        wallParent = wallParentObj.transform;

        GameObject roofParentObj = new GameObject("Roof");
        roofParentObj.transform.parent = lab.transform;
        roofParent = roofParentObj.transform;

        GameObject propsParentObj = new GameObject("Props");
        propsParentObj.transform.parent = lab.transform;
        propsParent = propsParentObj.transform;

        GameObject lampParentObj = new GameObject("Lamps");
        lampParentObj.transform.parent = lab.transform;
        lampParent = lampParentObj.transform;
    }

    private void GenerateMaze()
    {
        if (randomSeed)
        {
            currentSeed = Random.Range(0, int.MaxValue);
        }
        else
        {
            currentSeed = specifiedSeed;
        }

        Random.InitState(currentSeed);

        for (int x = 0; x < labWidth; x++)
        {
            for (int y = 0; y < labHeight; y++)
            {
                if (generateEmptyRooms && Random.Range(0, 100) < emptyRoomProbability)
                {
                    CreateFloor(x, y);
                    CreateRoof(x, y, (x * labHeight + y) % lampFrequency == 0);
                }
                else
                {
                    CreateFloor(x, y);
                    CreateWalls(x, y);
                    CreateRoof(x, y, (x * labHeight + y) % lampFrequency == 0);
                }
            }
        }
        CreateBoundaryWalls();
    }

    private void CreateFloor(int x, int y)
    {
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.transform.position = new Vector3(x * roomSize, -wallThickness / 2, y * roomSize);
        floor.transform.localScale = new Vector3(roomSize, wallThickness, roomSize);
        floor.GetComponent<Renderer>().material = floorMaterial;
        floor.transform.parent = floorParent;
        floor.tag = "LabObject";
    }

    private void CreateWalls(int x, int y)
    {
        if (x == 0 || x == labWidth - 1 || y == 0 || y == labHeight - 1)
        {
            return;
        }

        if (Random.Range(0, 2) == 0)
        {
            CreateWall(new Vector3(x * roomSize, wallHeight / 2, y * roomSize + roomSize / 2), new Vector3(roomSize, wallHeight, wallThickness), x, y);
        }

        if (Random.Range(0, 2) == 0)
        {
            CreateWall(new Vector3(x * roomSize + roomSize / 2, wallHeight / 2, y * roomSize), new Vector3(wallThickness, wallHeight, roomSize), x, y);
        }
    }

    private void CreateWall(Vector3 position, Vector3 scale, int x, int y)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.position = position;
        wall.transform.localScale = scale;

        if (useAltWalls && Random.Range(0, 4) == 0)
        {
            wall.GetComponent<Renderer>().material = altWallMaterial;
            wall.layer = LayerMask.NameToLayer(altWallLayer);
        }
        else
        {
            wall.GetComponent<Renderer>().material = wallMaterial;
        }

        wall.transform.parent = wallParent;
        wall.tag = "LabObject";
    }

    private void CreateRoof(int x, int y, bool addLamp)
    {
        GameObject roof = GameObject.CreatePrimitive(PrimitiveType.Cube);
        roof.transform.position = new Vector3(x * roomSize, wallHeight, y * roomSize);
        roof.transform.localScale = new Vector3(roomSize, wallThickness, roomSize);
        roof.GetComponent<Renderer>().material = roofMaterial;
        roof.transform.parent = roofParent;
        roof.tag = "LabObject";

        if (addLamp && lampPrefab != null)
        {
            GameObject lamp = Instantiate(lampPrefab, roof.transform.position + Vector3.up * lampOffset, Quaternion.identity);
            lamp.transform.parent = lampParent;
            lamp.tag = "LabObject";
        }
    }

    private void CreateBoundaryWalls()
    {
        for (int x = -1; x <= labWidth; x++)
        {
            CreateWall(new Vector3(x * roomSize, wallHeight / 2, -roomSize / 2), new Vector3(roomSize, wallHeight, wallThickness), x, -1);
            CreateWall(new Vector3(x * roomSize, wallHeight / 2, labHeight * roomSize - roomSize / 2), new Vector3(roomSize, wallHeight, wallThickness), x, labHeight);
        }

        for (int y = 0; y < labHeight; y++)
        {
            CreateWall(new Vector3(-roomSize / 2, wallHeight / 2, y * roomSize), new Vector3(wallThickness, wallHeight, roomSize), -1, y);
            CreateWall(new Vector3(labWidth * roomSize - roomSize / 2, wallHeight / 2, y * roomSize), new Vector3(wallThickness, wallHeight, roomSize), labWidth, y);
        }
    }

    private void ScatterRandomItems()
    {
        foreach (RandomItem randomItem in randomItems)
        {
            int itemCount = Random.Range(1, 4);
            for (int i = 0; i < itemCount; i++)
            {
                Vector3 randomPosition = new Vector3(
                    Random.Range(0, labWidth) * roomSize,
                    0,
                    Random.Range(0, labHeight) * roomSize
                );

                GameObject item = Instantiate(randomItem.itemPrefab, randomPosition, Quaternion.identity);
                item.GetComponent<Renderer>().material = randomItem.itemMaterial;
                item.transform.parent = propsParent;
                item.tag = "LabObject";
            }
        }
    }

    private void DestroyOldLab()
    {
        GameObject[] labObjects = GameObject.FindGameObjectsWithTag("LabObject");

        foreach (GameObject obj in labObjects)
        {
            if (Application.isPlaying)
            {
                Destroy(obj);
            }
            else
            {
                DestroyImmediate(obj);
            }
        }
    }

    public void ExportMazeMap(string filePath)
    {
        int textureWidth = labWidth * (int)roomSize;
        int textureHeight = labHeight * (int)roomSize;
        Texture2D mazeTexture = new Texture2D(textureWidth, textureHeight);

        for (int x = 0; x < textureWidth; x++)
        {
            for (int y = 0; y < textureHeight; y++)
            {
                mazeTexture.SetPixel(x, y, Color.white);
            }
        }

        foreach (Transform wall in wallParent)
        {
            int startX = Mathf.FloorToInt(wall.position.x - wall.localScale.x / 2);
            int endX = Mathf.CeilToInt(wall.position.x + wall.localScale.x / 2);
            int startY = Mathf.FloorToInt(wall.position.z - wall.localScale.z / 2);
            int endY = Mathf.CeilToInt(wall.position.z + wall.localScale.z / 2);

            for (int x = startX; x < endX; x++)
            {
                for (int y = startY; y < endY; y++)
                {
                    mazeTexture.SetPixel(x, y, Color.black);
                }
            }
        }

        mazeTexture.Apply();
        byte[] bytes = mazeTexture.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

    #region Editor

#if UNITY_EDITOR
    [CustomEditor(typeof(LabGenerator))]
    public class LabGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();

            LabGenerator labGenerator = (LabGenerator)target;

            EditorGUILayout.Space();

            if (GUILayout.Button("Create Maze"))
            {
                labGenerator.CreateMaze();
            }

            if (GUILayout.Button("Auto Setup"))
            {
                labGenerator.AutoSetup();
            }

            if (GUILayout.Button("Delete Old Maze"))
            {
                labGenerator.DeleteOldMaze();
            }

            if (GUILayout.Button("Export Maze Map"))
            {
                string path = EditorUtility.SaveFilePanel("Save Maze Map", "", "MazeMap", "png");
                if (!string.IsNullOrEmpty(path))
                {
                    labGenerator.ExportMazeMap(path);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif

    #endregion
}
//IDK even know what you are dooing in here but i have something for you look
/*
I was supposed to be sent away
But they forgot to come and get me
I was a functioning alcoholic
'Til nobody noticed my new aesthetic
All of this to say I hope you're okay
But you're the reason
And no one here's to blame
But what about your quiet treason?
Dont you just like taylor swift huh buddy idk if thats even the right text idk know
which song that is idk even know if thats talylor swift i never ever listend to here
i mean i live in germany we listen to this https://www.youtube.com/watch?v=dQw4w9WgXcQ
i know pretty chill even though basicly everyone says "German is so agressive"
NEIN IST ES VERDAMMT NOCHMAL NICHT DU HUHRENSOHN
Ok i do not want to write any more so just go with it
Made By Keo.CS (not the song or the yutube video only this script and also not the lyrics of talyorswift [Hopefully])
*/