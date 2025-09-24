using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject[] obstaclePrefabs;
    public float obstacleSpawnTime = 2f;
    public float obstacleSpeed = 1f;

    public float speeedIncreaseInterval = 10f;
    public float speedIncrement = 0.5f;
    public bool enableSpeedIncrease = true;

    private float timeUntilObstacleSpawn;
    private float initialObstacleSpeed;

    //track active obstacles
    private List<GameObject> activeObstacles = new List<GameObject>();

    private void Start()
    {
        initialObstacleSpeed = obstacleSpeed;

        if (enableSpeedIncrease)
        {
            StartCoroutine(ContinuousSpeedIncrease());
        }
    }

    private void Update()
    {
        timeUntilObstacleSpawn += Time.deltaTime;

        if (timeUntilObstacleSpawn >= obstacleSpawnTime)
        {
            Spawn();
            timeUntilObstacleSpawn = 0f;
        }

        //Check whether obstacle ob get out of the screen
        CheckObstaclesOutOfScreen();
    }

    private void Spawn()
    {
        GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
        
        // Spawn at Spawner's X position, keep prefab's original Y and Z positions
        Vector3 spawnPosition = new Vector3(transform.position.x, prefab.transform.position.y, prefab.transform.position.z);
        GameObject spawned = Instantiate(prefab, spawnPosition, Quaternion.identity);

        //Add active obstacle ob into list
        activeObstacles.Add(spawned);

        Physics2D.SyncTransforms();

        PostSpawnFixups(spawned);

        // Set velocity for all Rigidbody2D components to ensure all parts of composite obstacles move
        var rbs = spawned.GetComponentsInChildren<Rigidbody2D>();
        foreach (var rb in rbs)
        {
            if (rb) rb.linearVelocity = Vector2.left * obstacleSpeed;
        } 
    }

    private IEnumerator ContinuousSpeedIncrease()
    {
        while (enableSpeedIncrease)
        {
            yield return new WaitForSeconds(speeedIncreaseInterval);
            IncreaseSpeed();
        }
    }

    private void CheckObstaclesOutOfScreen()
    {
        for ( int i = activeObstacles.Count - 1; i >= 0; i--)
        {
            if (activeObstacles[i] == null)
            {
                activeObstacles.RemoveAt(i);
                continue;
            }

            if (IsOutOfScreen(activeObstacles[i]))
            {
                Destroy(activeObstacles[i]);
                activeObstacles.RemoveAt(i);
            }
        }
    }


    private bool IsOutOfScreen(GameObject obstacle)
    {
        var renderer = obstacle.GetComponentInChildren<Renderer>();
        if(renderer != null)
        {
            float obstacleRightEdge = renderer.bounds.max.x;
        }
        return obstacle.transform.position.x < GetScreenLeftEdge() - 2f;
    }

    private float GetScreenLeftEdge()
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
    }

    private static void PostSpawnFixups(GameObject root)
    {
        // Don't modify any layer settings, keep prefab's original layer configuration
        MakeAllCollidersTrigger(root);
        EnsureHasKinematicRB(root);
    }

    private static void MakeAllCollidersTrigger(GameObject root)
    {
        var cols = root.GetComponentsInChildren<Collider2D>(true);
        foreach (var c in cols) c.isTrigger = true;
    }

    private static void EnsureHasKinematicRB(GameObject root)
    {
        // Handle all Rigidbody2D components to ensure all parts of composite obstacles have correct physics settings
        var rbs = root.GetComponentsInChildren<Rigidbody2D>();
        
        if (rbs.Length == 0)
        {
            // If no Rigidbody2D exists, add one to the root object
            var rb = root.AddComponent<Rigidbody2D>();
            SetupRigidbody(rb);
        }
        else
        {
            // Setup all existing Rigidbody2D components
            foreach (var rb in rbs)
            {
                SetupRigidbody(rb);
            }
        }
    }
    
    private static void SetupRigidbody(Rigidbody2D rb)
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }


    public void RemoveObstacle(GameObject obstacle)
    {
        if (activeObstacles.Contains(obstacle))
        {
            activeObstacles.Remove(obstacle);
        }
    }

    // Update speed for all active obstacles
    private void UpdateActiveObstaclesSpeed()
    {
        foreach (GameObject obstacle in activeObstacles)
        {
            if (obstacle != null)
            {
                // Set velocity for all Rigidbody2D components in composite obstacles
                var rbs = obstacle.GetComponentsInChildren<Rigidbody2D>();
                foreach (var rb in rbs)
                {
                    if (rb) rb.linearVelocity = Vector2.left * obstacleSpeed;
                }
            }
        }
    }

    public void ResetSpeed()
    {
        obstacleSpeed = initialObstacleSpeed;
        UpdateActiveObstaclesSpeed();
    }

    // Set specific speed value
    public void SetSpeed(float newSpeed)
    {
        obstacleSpeed = newSpeed;
        UpdateActiveObstaclesSpeed();
    }

    public void EnableGradualSpeedIncrease(float duration, float targetSpeed)
    {
        StartCoroutine(GradualSpeedIncrease(duration, targetSpeed));
    }

    private IEnumerator GradualSpeedIncrease(float duration, float targetSpeed)
    {
        float startSpeed = obstacleSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            obstacleSpeed = Mathf.Lerp(startSpeed, targetSpeed, elapsedTime / duration);
            UpdateActiveObstaclesSpeed();
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        obstacleSpeed = targetSpeed;
        UpdateActiveObstaclesSpeed();
    }

    public void IncreaseSpeed()
    {
        obstacleSpeed += speedIncrement;
        UpdateActiveObstaclesSpeed();

        Debug.Log($"Obstacle speed increased to: {obstacleSpeed}");
    }

    public void ToggleSpeedIncrease(bool enable)
    {
        enableSpeedIncrease = enable;
        if (enable)
        {
            StartCoroutine(ContinuousSpeedIncrease());
        }
    }

    public void IncreaseSpeedBy(float amount)
    {
        obstacleSpeed += amount;
        UpdateActiveObstaclesSpeed();
    }


}





