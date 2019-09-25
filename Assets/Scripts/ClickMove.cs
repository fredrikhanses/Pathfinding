using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Search;

namespace Control
{
    public class ClickMove : MonoBehaviour
    {
        public GameObject player;
        public static Vector3 playerPosition;
        Vector3 worldPoint;
        Vector3 shootPoint;
        Camera mainCamera;
        public float speed = 5.0f;
        private float normalSpeed = 5.0f;
        private float slowdownSpeed = 2.0f;
        public static int mapSizeX = 20;
        public static int mapSizeY = 20;
        public static int obstacleCount = 100;
        public int bulletPickUps = 10;
        public int startNumber = 21;
        public int goalNumber = 5;
        public static SquareGrid grid = new SquareGrid(mapSizeX, mapSizeY);
        Vector3 winPosition = new Vector3();
        LinkedList<Location> path = new LinkedList<Location>();
        public Sprite obstacleSprite;
        public Sprite floorSprite;
        public Sprite forestSprite;
        public Sprite winSprite;
        public Sprite pathingSprite;
        public Sprite bulletSprite;
        public float ballForce = 500f;
        public static bool bulletSpawned = false;
        public static Location obstaclePosition = new Location(-1, -1);
        public static Location pickUpPosition = new Location(-1, -1);
        Location oldLocation;

        private void Awake()
        {
            Initialization();
        }
        private void Start()
        {
            GameRules.instance.OnAmmoAdded();
            GameRules.instance.OnAmmoAdded();
        }

        private void Update()
        {
            InputHandler();
        }

        private void FixedUpdate()
        {
            ShowPath();
            NextStep();
            MovementHandler();  
        }

        private void LateUpdate()
        {
            playerPosition = player.transform.position;
        }

        private void Initialization()
        {
            ResetPath();
            ResetGrid();
            mainCamera = Camera.main;
            CreateObstacles();
            CreateBoard();
            CreateWinPoint();
            CreatePickUps();
            SetPlayerStartPosition();
            worldPoint = player.transform.position;
        }

        private void InputHandler()
        {
            if (Input.GetMouseButton(1) && !bulletSpawned && GameRules.instance.ammoRemaining > 0)
            {
                RemoveObstacle(obstaclePosition);
                CreateBullet();
            }
            if (Input.GetMouseButtonDown(0)) // && worldPoint == player.transform.position)
            {
                ResetPath();
                UpdateWorldPoint();
                SearchShortestPath();
                ResetWorldPoint();
            }
            if (Input.GetMouseButtonDown(2)) //&& worldPoint == player.transform.position)
            {
                ResetPath();
                worldPoint = winPosition;
                SearchShortestPath();
                ResetWorldPoint();
            }
        }

        private void SetPlayerStartPosition()
        {
            int randomX = Random.Range(0, mapSizeX);
            int randomY = Random.Range(0, mapSizeY);
            while (grid.obstacles.Contains(new Location(randomX, randomY)))
            {
                randomX = Random.Range(0, mapSizeX);
                randomY = Random.Range(0, mapSizeY);
            }
            player.transform.position = new Vector2(randomX, randomY);
        }

        private void ResetGrid()
        {
            grid.obstacles.Clear();
            grid.slowDowns.Clear();
            grid.pickUps.Clear();
            grid.pathing.Clear();
            grid.AIPathing.Clear();
        }

        private void ResetPath()
        {
            RemoveObstacle(obstaclePosition);
            if (path.Any())
            {
                path.Clear();
            }
        }

        private void UpdateWorldPoint()
        {
            worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            worldPoint.z = 0;
            RoundToInt(ref worldPoint);
        }

        private void SearchShortestPath()
        {
            Location start = new Location((int)player.transform.position.x, (int)player.transform.position.y);
            Location goal = new Location((int)worldPoint.x, (int)worldPoint.y);
            AStarSearch astar = new AStarSearch();
            path = astar.AStar(grid, start, goal);      
        }

        private void ResetWorldPoint()
        {
            worldPoint = player.transform.position;
        }

        private void NextStep()
        {
            if (worldPoint == player.transform.position && path.Any())
            {
                float pathX = path.Last.Value.x;
                float pathY = path.Last.Value.y;
                worldPoint = new Vector2(pathX, pathY);
                path.RemoveLast();
            }
        }

        private void MovementHandler()
        {
            if(grid.slowDowns.Contains(new Location((int)worldPoint.x, (int)worldPoint.y)))
            {
                speed = slowdownSpeed;
            }
            else
            {
                speed = normalSpeed;
            }
            if (worldPoint != player.transform.position)
            {
                float step = speed * Time.deltaTime;
                player.transform.position = Vector3.MoveTowards(player.transform.position, worldPoint, step);
                if (Vector3.Distance(player.transform.position, worldPoint) < 0.001f)
                {
                    worldPoint = player.transform.position;
                    RemovePathing(oldLocation);
                }
            }
            else
            {
                if (grid.pickUps.Contains(pickUpPosition))
                {
                    RemovePickup(pickUpPosition);
                    GameRules.instance.OnAmmoAdded();
                }
            }
            if (player.transform.position == winPosition)
            {
                Debug.Log("WIN");
                GameRules.instance.GameWin();
            }
        }

        #region CreateObjects
        private void CreateBoard()
        {
            for (int j = 0; j < mapSizeY; j++)
            {
                for (int i = 0; i < mapSizeX; i++)
                {
                    GameObject floor = new GameObject("Floor", typeof(SpriteRenderer));
                    floor.transform.position = new Vector2(i, j);
                    floor.GetComponent<SpriteRenderer>().sprite = floorSprite;
                    floor.GetComponent<SpriteRenderer>().color = Color.white;
                    
                    if (CreateSlowdown())
                    {
                        int randomX = Random.Range(0, mapSizeX);
                        int randomY = Random.Range(0, mapSizeY);
                        if (randomX == 0 && randomY == 0)
                        {
                            randomX += 1;
                            randomY += 1;
                        }
                        grid.slowDowns.Add(new Location(randomX, randomY));
                        GameObject forest = new GameObject("Forest", typeof(SpriteRenderer));
                        forest.transform.position = new Vector2(randomX, randomY);
                        forest.GetComponent<SpriteRenderer>().sprite = forestSprite;
                        forest.GetComponent<SpriteRenderer>().color = Color.green;
                        forest.GetComponent<SpriteRenderer>().sortingLayerName = "Obstacle";
                    }
                }
            }
        }
        private void CreateObstacles()
        {
            for (int i = 0; i < obstacleCount; i++)
            {
                int randomX = Random.Range(0, mapSizeX);
                int randomY = Random.Range(0, mapSizeY);
                if (randomX == 0 && randomY == 0)
                {
                    randomX += 1;
                    randomY += 1;
                }
                grid.obstacles.Add(new Location(randomX, randomY));
                GameObject obstacle = new GameObject("Obstacle", typeof(SpriteRenderer), typeof(BoxCollider2D));
                obstacle.transform.position = new Vector2(randomX, randomY);
                obstacle.GetComponent<SpriteRenderer>().sprite = obstacleSprite;
                obstacle.GetComponent<SpriteRenderer>().color = Color.red;
                obstacle.GetComponent<SpriteRenderer>().sortingLayerName = "Obstacle";
                obstacle.GetComponent<BoxCollider2D>().size = new Vector2(1f, 1f);
            }
        }
        private void CreatePickUps()
        {
            for (int i = 0; i < bulletPickUps; i++)
            {
                int randomX = Random.Range(0, mapSizeX);
                int randomY = Random.Range(0, mapSizeY);
                if (randomX == 0 && randomY == 0)
                {
                    randomX += 1;
                    randomY += 1;
                }
                grid.pickUps.Add(new Location(randomX, randomY));
                GameObject pickUp = new GameObject("PickUp", typeof(SpriteRenderer), typeof(CircleCollider2D));
                pickUp.transform.position = new Vector2(randomX, randomY);
                pickUp.GetComponent<SpriteRenderer>().sprite = bulletSprite;
                pickUp.GetComponent<SpriteRenderer>().color = Color.magenta;
                pickUp.GetComponent<SpriteRenderer>().sortingLayerName = "Obstacle";
                pickUp.GetComponent<SpriteRenderer>().transform.localScale = new Vector2(0.25f, 0.25f);
                pickUp.GetComponent<CircleCollider2D>().isTrigger = true;
                pickUp.AddComponent<Rigidbody2D>();
                pickUp.GetComponent<Rigidbody2D>().gravityScale = 0;
                pickUp.GetComponent<Rigidbody2D>().mass = 0;
                pickUp.AddComponent<AmmoPickUp>();
            }
        }

        private void CreateWinPoint()
        {
            int randomX = Random.Range(0, mapSizeX);
            int randomY = Random.Range(0, mapSizeY);
            if (grid.obstacles.Contains(new Location(randomX, randomY)))
            {
                CreateWinPoint();
            }
            else if (randomX == 0 && randomY == 0)
            {
                randomX += 1;
                randomY += 1;
                winPosition = new Vector2(randomX, randomY);
                GameObject win = new GameObject("Win", typeof(SpriteRenderer));
                win.transform.position = winPosition;
                win.GetComponent<SpriteRenderer>().sprite = winSprite;
                win.GetComponent<SpriteRenderer>().color = Color.yellow;
                win.GetComponent<SpriteRenderer>().sortingLayerName = "Obstacle";
            }
            else
            {
                winPosition = new Vector2(randomX, randomY);
                GameObject win = new GameObject("Win", typeof(SpriteRenderer));
                win.transform.position = winPosition;
                win.GetComponent<SpriteRenderer>().sprite = winSprite;
                win.GetComponent<SpriteRenderer>().color = Color.yellow;
                win.GetComponent<SpriteRenderer>().sortingLayerName = "Obstacle";
            }
        }

        private void ShowPath()
        {
            oldLocation = new Location((int)worldPoint.x, (int)worldPoint.y);
            if (path.Any() && !grid.pathing.Contains(oldLocation))
            {
                grid.pathing.Add(oldLocation);
                GameObject pathing = new GameObject("Pathing", typeof(SpriteRenderer));
                pathing.transform.position = new Vector2(worldPoint.x, worldPoint.y);
                pathing.GetComponent<SpriteRenderer>().sprite = pathingSprite;
                pathing.GetComponent<SpriteRenderer>().color = Color.blue;
                pathing.GetComponent<SpriteRenderer>().transform.localScale = new Vector2(0.5f, 0.5f);
                pathing.GetComponent<SpriteRenderer>().sortingLayerName = "Obstacle";
                pathing.GetComponent<SpriteRenderer>().sortingOrder = 1;
                pathing.AddComponent<TimedDelete>();
            }
        }

        private void CreateBullet()
        {
            shootPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            shootPoint.z = 0;
            RoundToInt(ref shootPoint);
            GameObject bullet = new GameObject("Bullet", typeof(SpriteRenderer), typeof(CircleCollider2D), typeof(Rigidbody2D));
            bullet.transform.position = new Vector2(player.transform.position.x, player.transform.position.y);
            bullet.tag = "Bullet";
            bullet.GetComponent<SpriteRenderer>().sprite = bulletSprite;
            bullet.GetComponent<SpriteRenderer>().color = Color.magenta;
            bullet.GetComponent<SpriteRenderer>().transform.localScale = new Vector2(0.25f, 0.25f);
            bullet.GetComponent<SpriteRenderer>().sortingLayerName = "Obstacle";
            bullet.GetComponent<SpriteRenderer>().sortingOrder = 1;
            bullet.GetComponent<Rigidbody2D>().velocity = transform.TransformDirection(Vector3.right) * ballForce * Time.deltaTime;
            bullet.GetComponent<Rigidbody2D>().gravityScale = 0;
            bullet.GetComponent<Rigidbody2D>().mass = 0;
            bullet.AddComponent<Bullet>();
            bulletSpawned = true;
            GameRules.instance.OnAmmoRemoved();
        }
        #endregion CreateObjects

        public void RemoveObstacle(Location location)
        {
            grid.obstacles.Remove(location);
        }

        public void RemovePathing(Location location)
        {
            grid.pathing.Remove(location);
        }

        public void RemovePickup(Location location)
        {
            grid.pickUps.Remove(location);
        }

        private void RoundToInt(ref Vector3 worldPoint)
        {
            worldPoint.x = Mathf.RoundToInt(worldPoint.x);
            worldPoint.y = Mathf.RoundToInt(worldPoint.y);
        }

        public bool CreateSlowdown()
        {
            int random = Random.Range(0, 5);
            if (random == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}