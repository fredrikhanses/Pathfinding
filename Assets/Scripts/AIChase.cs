using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Search;
using Control;

namespace AIcontrol
{
    public class AIChase : MonoBehaviour
    {
        public GameObject enemy;
        Vector3 enemyPosition;
        public int numberOfEnemies = 3;
        Vector3 targetPosition;
        public float speed = 3.0f;
        private float normalSpeed = 3.0f;
        private float slowdownSpeed = 1.0f;
        public int startNumber = 21;
        public int goalNumber = 5;
        LinkedList<Location> path = new LinkedList<Location>();
        public Sprite enemySprite;
        public Sprite pathingSprite;
        Location oldLocation;
        int steps = 0;

        private void Start()
        {
            Initialization();
        }

        private void FixedUpdate()
        {
            ShowPath();
            NextStep();
            MovementHandler();
        }
        private void Update()
        {
            if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(2))
            {
                ResetPath();
                targetPosition = ClickMove.playerPosition;
                RoundToInt(ref targetPosition);
                SearchShortestPath();
                ResetTargetPosition();
            }
        }

        private void Initialization()
        {
            ResetPath();
            targetPosition = ClickMove.playerPosition;
            RoundToInt(ref targetPosition);
            SetEnemyStartPosition();
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
            Location goal = new Location((int)targetPosition.x, (int)targetPosition.y);
            AStarSearch astar = new AStarSearch();
            path = astar.AStar(ClickMove.grid, start, goal);
        }

        private void ResetTargetPosition()
        {
            targetPosition = enemy.transform.position;
        }

        private void NextStep()
        {
            if (targetPosition == enemy.transform.position && path.Any())
            {
                float pathX = path.Last.Value.x;
                float pathY = path.Last.Value.y;
                targetPosition = new Vector2(pathX, pathY);
                path.RemoveLast();
            }
        }

        private void MovementHandler()
        {
            if (ClickMove.grid.slowDowns.Contains(new Location((int)targetPosition.x, (int)targetPosition.y)))
            {
                speed = slowdownSpeed;
            }
            else
            {
                speed = normalSpeed;
            }
            if (targetPosition != enemy.transform.position)
            {
                if(steps == 0)
                {
                    ResetPath();
                    targetPosition = ClickMove.playerPosition;
                    RoundToInt(ref targetPosition);
                    SearchShortestPath();
                    ResetTargetPosition();
                }
                float step = speed * Time.deltaTime;
                enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, targetPosition, step);
                if (Vector3.Distance(enemy.transform.position, targetPosition) < 0.001f)
                {
                    steps++;
                    if(steps == 5)
                    {
                        steps = 0;
                    }
                    ResetTargetPosition();
                    RemovePathing(oldLocation);
                }
                if (Vector3.Distance(enemy.transform.position, ClickMove.playerPosition) < 1f)
                {
                    Debug.Log("LOSE");
                    GameRules.instance.GameLose();
                }
            }
        }

        private void SetEnemyStartPosition()
        {
            int randomX = Random.Range(1, ClickMove.mapSizeX);
            int randomY = Random.Range(1, ClickMove.mapSizeY);
            while (ClickMove.grid.obstacles.Contains(new Location(randomX, randomY)))
            {
                randomX = Random.Range(1, ClickMove.mapSizeX);
                randomY = Random.Range(1, ClickMove.mapSizeY);
            }
            enemy.transform.position = new Vector2(randomX, randomY); 
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
                    GameObject enemy = new GameObject("Enemy", typeof(SpriteRenderer));
                    enemy.transform.position = enemyPosition;
                    enemy.GetComponent<SpriteRenderer>().sprite = enemySprite;
                    enemy.GetComponent<SpriteRenderer>().color = Color.red;
                    enemy.GetComponent<SpriteRenderer>().sortingLayerName = "Obstacle";
                    enemy.AddComponent<AIChase>();
                    enemy.GetComponent<AIChase>().enemy = GameObject.FindGameObjectWithTag("Enemy");
                    //enemy.GetComponent<AIChase>().pathingSprite = Sprite.
                }
            }
        }

        private void ShowPath()
        {
            oldLocation = new Location((int)targetPosition.x, (int)targetPosition.y);
            if (path.Any() && !(ClickMove.grid.AIPathing.Contains(oldLocation)))
            {
                ClickMove.grid.AIPathing.Add(oldLocation);
                GameObject pathing = new GameObject("AIPathing", typeof(SpriteRenderer));
                pathing.transform.position = new Vector2(targetPosition.x, targetPosition.y);
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
            ClickMove.grid.AIPathing.Remove(location);
        }

        private void RoundToInt(ref Vector3 worldPoint)
        {
            worldPoint.x = Mathf.RoundToInt(worldPoint.x);
            worldPoint.y = Mathf.RoundToInt(worldPoint.y);
        }
    }
}