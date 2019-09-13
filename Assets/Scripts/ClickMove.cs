using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Search;
using UnityEngine.SceneManagement;

public class ClickMove : MonoBehaviour
{
    public GameObject player;
    Vector3 worldPoint;
    Camera mainCamera;
    public float speed = 5.0f;
    public static int mapSizeX = 20;
    public static int mapSizeY = 20;
    public static int obstacleCount = 150;
    public int startNumber = 21;
    public int goalNumber = 5;
    public Graph<int> graphNumbers = new Graph<int>();
    Vector3[] obstacleArray = new Vector3[obstacleCount];
    Vector3[] walkableArray = new Vector3[mapSizeX * mapSizeY];
    Vector3 winPosition = new Vector3();
    LinkedList<int> pathNumbers = new LinkedList<int>();
    public Sprite obstacleSprite;
    public Sprite floorSprite;
    public Sprite winSprite;
    int[] obstacleArrayIndex = new int[obstacleCount];
    public int winSceneIndex = 1;

    private void Start()
    {
        Initialization();
    }

    private void Update()
    {
        InputHandler();
    }

    private void FixedUpdate()
    {
        NextStep();
        MovementHandler();
    }

    private void CreateWinPoint()
    {
        winPosition.x = Random.Range(mapSizeX / 2, mapSizeX);
        winPosition.y = Random.Range(mapSizeY / 2, mapSizeY);
        winPosition.z = 0;
        RoundToInt(ref winPosition);
        if (obstacleArray.Contains(winPosition))
        {
            CreateWinPoint();
        }
        GameObject win = new GameObject("Win", typeof(SpriteRenderer));
        win.transform.position = winPosition;
        win.GetComponent<SpriteRenderer>().sprite = winSprite;
        win.GetComponent<SpriteRenderer>().color = Color.yellow;
        win.GetComponent<SpriteRenderer>().sortingLayerName = "Obstacle";
    }
    
    private void CreateObstacles()
    {
        for(int i = 0; i < obstacleCount; i++)
        {
            obstacleArray[i].x = Random.Range(0, mapSizeX);
            obstacleArray[i].y = Random.Range(0, mapSizeY);
            obstacleArray[i].z = 0;
            RoundToInt(ref obstacleArray[i]);
            if (obstacleArray[i] == Vector3.zero)
            {
                obstacleArray[i].x += 1;
                obstacleArray[i].y += 1;
            }
            GameObject obstacle = new GameObject("Obstacle", typeof(SpriteRenderer));
            obstacle.transform.position = obstacleArray[i];
            obstacle.GetComponent<SpriteRenderer>().sprite = obstacleSprite;
            obstacle.GetComponent<SpriteRenderer>().color = Color.red;
            obstacle.GetComponent<SpriteRenderer>().sortingLayerName = "Obstacle";
        }
    }

    private void CreateBoard()
    {
        int index = 0;
        int obstacleIndex = 0;
        for (int j = 0; j < mapSizeY; j++)
        {
            for (int i = 0; i < mapSizeX; i++)
            {
                walkableArray[index] = new Vector3(i, j, 0);
                if (obstacleArray.Contains(walkableArray[index]))
                {
                    obstacleArrayIndex[obstacleIndex] = index;
                    obstacleIndex++;
                }
                GameObject floor = new GameObject("Floor", typeof(SpriteRenderer));
                floor.transform.position = walkableArray[index];
                floor.GetComponent<SpriteRenderer>().sprite = floorSprite;
                floor.GetComponent<SpriteRenderer>().color = Color.white;
                //Debug.Log($"{index}: {walkableArray[index]}");
                index++;
            }
        }
    }

    private void Initialization()
    {
        worldPoint = player.transform.position;
        mainCamera = Camera.main;

        #region Old Hardcoded Values
        /*
        obstacleArray[0] = new Vector3(1f, 4f, 0f);
        obstacleArray[1] = new Vector3(2f, 4f, 0f);
        obstacleArray[2] = new Vector3(4f, 4f, 0f);
        obstacleArray[3] = new Vector3(1f, 3f, 0f);
        obstacleArray[4] = new Vector3(4f, 3f, 0f);
        obstacleArray[5] = new Vector3(2f, 2f, 0f);
        obstacleArray[6] = new Vector3(3f, 2f, 0f);
        obstacleArray[7] = new Vector3(4f, 2f, 0f);
        obstacleArray[8] = new Vector3(2f, 1f, 0f);

        walkableArray[0] = new Vector3(0f, 5f, 0f);
        walkableArray[1] = new Vector3(1f, 5f, 0f);
        walkableArray[2] = new Vector3(2f, 5f, 0f);
        walkableArray[3] = new Vector3(3f, 5f, 0f);
        walkableArray[4] = new Vector3(4f, 5f, 0f);
        walkableArray[5] = new Vector3(5f, 5f, 0f);
        walkableArray[6] = new Vector3(0f, 4f, 0f);
        walkableArray[7] = new Vector3(3f, 4f, 0f);
        walkableArray[8] = new Vector3(5f, 4f, 0f);
        walkableArray[9] = new Vector3(0f, 3f, 0f);
        walkableArray[10] = new Vector3(2f, 3f, 0f);
        walkableArray[11] = new Vector3(3f, 3f, 0f);
        walkableArray[12] = new Vector3(5f, 3f, 0f);
        walkableArray[13] = new Vector3(0f, 2f, 0f);
        walkableArray[14] = new Vector3(1f, 2f, 0f);
        walkableArray[15] = new Vector3(5f, 2f, 0f);
        walkableArray[16] = new Vector3(0f, 1f, 0f);
        walkableArray[17] = new Vector3(1f, 1f, 0f);
        walkableArray[18] = new Vector3(3f, 1f, 0f);
        walkableArray[19] = new Vector3(4f, 1f, 0f);
        walkableArray[20] = new Vector3(5f, 1f, 0f);
        walkableArray[21] = new Vector3(0f, 0f, 0f);
        walkableArray[22] = new Vector3(1f, 0f, 0f);
        walkableArray[23] = new Vector3(2f, 0f, 0f);
        walkableArray[24] = new Vector3(3f, 0f, 0f);
        walkableArray[25] = new Vector3(4f, 0f, 0f);
        walkableArray[26] = new Vector3(5f, 0f, 0f);

        {
                { 0, new [] { 1, 6 } },
                { 1, new [] { 0, 2, 7 } },
                { 2, new [] { 1, 3, 8 } },
                { 3, new [] { 2, 4, 9 } },
                { 4, new [] { 3, 5, 10 } },
                { 5, new [] { 4, 11 } },
                { 6, new [] { 0, 7, 12 } },
                { 7, new [] { 1, 6, 8, 13 } },
                { 8, new [] { 2, 7, 9, 14 } },
                { 9, new [] { 3, 8, 10, 15 } },
                { 10, new [] { 4, 9, 11, 16 } },
                { 11, new [] { 5, 10, 17 } },
                { 12, new [] { 6, 13, 18 } },
                { 13, new [] { 7, 12, 14, 19 } },
                { 14, new [] { 8, 13, 15, 20 } },
                { 15, new [] { 9, 14, 16, 21 } },
                { 16, new [] { 10, 15, 17, 22 } },
                { 17, new [] { 11, 16, 23 } },
                { 18, new [] { 12, 19, 24 } },
                { 19, new [] { 13, 18, 20, 25 } },
                { 20, new [] { 14, 19, 21, 26 } },
                { 21, new [] { 15, 20, 22, 27 } },
                { 22, new [] { 16, 21, 23, 28 } },
                { 23, new [] { 17, 22, 29 } },
                { 24, new [] { 18, 25, 30 } },
                { 25, new [] { 19, 24, 26, 31 } },
                { 26, new [] { 20, 25, 27, 32 } },
                { 27, new [] { 21, 26, 28, 33 } },
                { 28, new [] { 22, 27, 29, 34 } },
                { 29, new [] { 23, 28, 35 } },
                { 30, new [] { 24, 31} },
                { 31, new [] { 25, 30, 32 } },
                { 32, new [] { 26, 31, 33 } },
                { 33, new [] { 27, 32, 34 } },
                { 34, new [] { 28, 33, 35 } },
                { 35, new [] { 29, 34 } }
        };
        */
        #endregion Old Hardcoded Values

        CreateObstacles();
        CreateBoard();
        CreateWinPoint();
        CreateNodes();
    }

    private void CreateNodes()
    {
        graphNumbers.edges = new Dictionary<int, int[]>();

        for (int i = 0; i < mapSizeX * mapSizeY; i++)
        {
            if (i == 0)
            {
                graphNumbers.edges.Add(i, new[] { i + 1, i + mapSizeX });                      // Left Bottom Corner 0
            }
            else if (i == (mapSizeX - 1))
            {
                graphNumbers.edges.Add(i, new[] { i - 1, i + mapSizeX });                      // Right Bottom Corner 5
            }
            else if (i == (mapSizeX * mapSizeY - mapSizeX))
            {
                graphNumbers.edges.Add(i, new[] { i - mapSizeX, i + 1 });                      // Left Upper Corner 30
            }
            else if (i == (mapSizeX * mapSizeY - 1))
            {
                graphNumbers.edges.Add(i, new[] { i - mapSizeX, i - 1 });                      // Right Upper Corner 35
            }
            else if (i < mapSizeX)
            {
                graphNumbers.edges.Add(i, new[] { i - 1, i + 1, i + mapSizeX });               // Bottom Row 1, 2, 3, 4
            }
            else if (i == (i / mapSizeX) * mapSizeX)
            {
                graphNumbers.edges.Add(i, new[] { i - mapSizeX, i + 1, i + mapSizeX });        // Left Column 6, 12, 18, 24
            }
            else if (i == (i / mapSizeX) * mapSizeX + mapSizeX - 1)
            {
                graphNumbers.edges.Add(i, new[] { i - mapSizeX, i - 1, i + mapSizeX });        // Right Column 11, 17, 23, 29
            }
            else if (i > (mapSizeX * mapSizeY - mapSizeX))
            {
                graphNumbers.edges.Add(i, new[] { i - mapSizeX, i - 1, i + 1 });               // Upper Row 31, 32, 33, 34
            }
            else
            {
                graphNumbers.edges.Add(i, new[] { i - mapSizeX, i - 1, i + 1, i + mapSizeX }); // The Rest | 7, 8, 9, 10 | 13, 14, 15, 16 | 19, 20, 21, 22 | 25, 26, 27, 28 |
            }
        }
    }

    private void InputHandler()
    {
        if (Input.GetMouseButton(1))
        {
            UpdateWorldPoint();
        }
        if (Input.GetMouseButtonDown(0) && worldPoint == player.transform.position)
        {
            //Debug.Log("click");
            UpdateWorldPoint();
            SearchShortestPath();
            ResetWorldPoint();
        }
    }

    private void ResetWorldPoint()
    {
        worldPoint = player.transform.position;
    }

    private void SearchShortestPath()
    {
        startNumber = LinearSearch.LinearSearchSimple(ref walkableArray, player.transform.position);
        goalNumber = LinearSearch.LinearSearchSimple(ref walkableArray, worldPoint);
        pathNumbers = BreadthFirstSearch.BreadthSearchNumbers(ref graphNumbers, ref obstacleArrayIndex, startNumber, goalNumber, mapSizeX * mapSizeY);
    }

    private void UpdateWorldPoint()
    {
        worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        worldPoint.z = 0;
        //Debug.Log($"Clicked: {worldPoint}");
        RoundToInt(ref worldPoint);
        //Debug.Log($"Want: {worldPoint}");
    }

    private void NextStep()
    {
        if(worldPoint == player.transform.position && pathNumbers.Any())
        {
            worldPoint = walkableArray[pathNumbers.Last.Value];
            pathNumbers.RemoveLast();
        }
    }

    private void MovementHandler()
    {
        if (worldPoint != player.transform.position && worldPoint.x >= 0f && worldPoint.x < mapSizeX && worldPoint.y < mapSizeY && worldPoint.y >= 0f)
        {
            float step = speed * Time.deltaTime;
            player.transform.position = Vector3.MoveTowards(player.transform.position, worldPoint, step);
            if (Vector3.Distance(player.transform.position, worldPoint) < 0.001f)
            {
                worldPoint = player.transform.position;
            }
        }
        if(player.transform.position == winPosition)
        {
            GameWin();
        }
    }

    private void RoundToInt(ref Vector3 worldPoint)
    {
        worldPoint.x = Mathf.RoundToInt(worldPoint.x);
        worldPoint.y = Mathf.RoundToInt(worldPoint.y);
    }

    private void GameWin()
    {
        SceneManager.LoadScene(winSceneIndex);
    }
}
