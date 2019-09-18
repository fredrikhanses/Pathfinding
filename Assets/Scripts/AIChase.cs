using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Search;
using control;

namespace AIcontrol
{
    public class AIChase : MonoBehaviour
    {
        public GameObject enemy;
        Vector3 enemyPosition;
        public int numberOfEnemies = 1;
        Vector3 worldPoint;
        public float speed = 3.0f;
        public int startNumber = 21;
        public int goalNumber = 5;
        LinkedList<Location> path = new LinkedList<Location>();
        public Sprite enemySprite;
        public Sprite pathingSprite;
        Location oldLocation;

        private void Start()
        {
            Initialization();
        }

        private void Update()
        {
            ResetPath();
            worldPoint = ClickMove.playerPosition;
            RoundToInt(ref worldPoint);
            SearchShortestPath();
            ResetWorldPoint();
        }

        private void FixedUpdate()
        {
            ShowPath();
            NextStep();
            MovementHandler();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.gameObject.tag == "Player")
            {
                ClickMove.GameLose();
            }
        }

        private void Initialization()
        {
            worldPoint = ClickMove.playerPosition;
            RoundToInt(ref worldPoint);
            //CreateEnemy();
        }

        private void ResetPath()
        {
            if (path.Any())
            {
                path.Clear();
            }
        }

        private void SearchShortestPath()
        {
            Location start = new Location((int)enemy.transform.position.x, (int)enemy.transform.position.y);
            Location goal = new Location((int)worldPoint.x, (int)worldPoint.y);
            AStarSearch astar = new AStarSearch();
            path = astar.AStar(ClickMove.grid, start, goal);
        }

        private void ResetWorldPoint()
        {
            worldPoint = enemy.transform.position;
        }

        private void NextStep()
        {
            if (worldPoint == enemy.transform.position && path.Any())
            {
                float pathX = path.Last.Value.x;
                float pathY = path.Last.Value.y;
                worldPoint = new Vector2(pathX, pathY);
                path.RemoveLast();
            }
        }

        private void MovementHandler()
        {
            if (worldPoint != enemy.transform.position)
            {
                float step = speed * Time.deltaTime;
                enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, worldPoint, step);
                if (Vector3.Distance(enemy.transform.position, worldPoint) < 0.001f)
                {
                    worldPoint = enemy.transform.position;
                    RemovePathing(oldLocation);
                }
            }
        }

        #region CreateObjects
        private void CreateEnemy()
        {
            for(int i = 0; i < numberOfEnemies; i++)
            {
                int randomX = Random.Range(1, ClickMove.mapSizeX);
                int randomY = Random.Range(1, ClickMove.mapSizeY);
                if (ClickMove.grid.obstacles.Contains(new Location(randomX, randomY)))
                {
                    CreateEnemy();
                }
                else
                {
                    enemyPosition = new Vector2(randomX, randomY);
                    GameObject enemy = new GameObject("Enemy", typeof(SpriteRenderer), typeof(CircleCollider2D));
                    enemy.transform.position = enemyPosition;
                    enemy.GetComponent<SpriteRenderer>().sprite = enemySprite;
                    enemy.GetComponent<SpriteRenderer>().color = Color.red;
                    enemy.GetComponent<SpriteRenderer>().sortingLayerName = "Obstacle";
                    enemy.GetComponent<CircleCollider2D>().isTrigger = true;
                    enemy.AddComponent<Rigidbody2D>();
                    enemy.GetComponent<Rigidbody2D>().gravityScale = 0;
                    enemy.GetComponent<Rigidbody2D>().mass = 0;
                    enemy.AddComponent<AIChase>();
                }
            }
        }

        private void ShowPath()
        {
            oldLocation = new Location((int)worldPoint.x, (int)worldPoint.y);
            if (path.Any() && !ClickMove.grid.pathing.Contains(oldLocation))
            {
                //ClickMove.grid.AIPathing.Add(oldLocation);
                GameObject pathing = new GameObject("Pathing", typeof(SpriteRenderer));
                pathing.transform.position = new Vector2(worldPoint.x, worldPoint.y);
                pathing.GetComponent<SpriteRenderer>().sprite = pathingSprite;
                pathing.GetComponent<SpriteRenderer>().color = Color.red;
                pathing.GetComponent<SpriteRenderer>().transform.localScale = new Vector2(0.5f, 0.5f);
                pathing.GetComponent<SpriteRenderer>().sortingLayerName = "Obstacle";
                pathing.GetComponent<SpriteRenderer>().sortingOrder = 1;
                pathing.AddComponent<TimedDelete>();
            }
        }
        #endregion CreateObjects

        public void RemovePathing(Location location)
        {
            ClickMove.grid.pathing.Remove(location);
        }

        private void RoundToInt(ref Vector3 worldPoint)
        {
            worldPoint.x = Mathf.RoundToInt(worldPoint.x);
            worldPoint.y = Mathf.RoundToInt(worldPoint.y);
        }
    }
}