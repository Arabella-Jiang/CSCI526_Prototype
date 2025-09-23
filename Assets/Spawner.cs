using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject[] obstaclePrefabs;
    public float obstacleSpawnTime = 2f;
    public float obstacleSpeed = 1f;
    private float[] spawnHeights = new float[] { -4f, -3.1f };

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
        float chosenY = IsType1(prefab) ? -4f : spawnHeights[Random.Range(0, spawnHeights.Length)];

        Vector3 spawnPosition = new Vector3(transform.position.x, chosenY, 0f);
        GameObject spawned = Instantiate(prefab, spawnPosition, Quaternion.identity);

        //Add active obstacle ob into list
        activeObstacles.Add(spawned);

        Physics2D.SyncTransforms();

        var cols = spawned.GetComponentsInChildren<Collider2D>(true);
        if (cols.Length > 0)
        {
            float bottom = float.MaxValue;
            foreach (var c in cols) if (c.enabled) bottom = Mathf.Min(bottom, c.bounds.min.y);
            if (bottom < float.MaxValue)
                spawned.transform.position += new Vector3(0f, chosenY - bottom, 0f);
        }

        PostSpawnFixups(spawned);

        var rb = spawned.GetComponentInChildren<Rigidbody2D>();
        if (rb) rb.linearVelocity = Vector2.left * obstacleSpeed; 
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
        ForceChildrenLayersToRoot(root);
        MakeAllCollidersTrigger(root);
        EnsureHasKinematicRB(root);
    }

    private static void ForceChildrenLayersToRoot(GameObject root)
    {
        int layer = root.layer; 
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
            t.gameObject.layer = layer;
    }

    private static void MakeAllCollidersTrigger(GameObject root)
    {
        var cols = root.GetComponentsInChildren<Collider2D>(true);
        foreach (var c in cols) c.isTrigger = true;
    }

    private static void EnsureHasKinematicRB(GameObject root)
    {
        var rb = root.GetComponentInChildren<Rigidbody2D>();
        if (!rb) rb = root.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private bool IsType1(GameObject prefab)
    {
        string n = prefab.name.ToLowerInvariant();
        return n.Contains("type1") || n.Replace(" ", "").Contains("type1");
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
                var rb = obstacle.GetComponentInChildren<Rigidbody2D>();
                if (rb) rb.linearVelocity = Vector2.left * obstacleSpeed;
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





