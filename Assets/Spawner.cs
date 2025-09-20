using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject[] obstaclePrefabs;
    public float obstacleSpawnTime = 2f;
    public float obstacleSpeed = 1f;
    private float[] spawnHeights = new float[] { -4f, -3.1f };

    private float timeUntilObstacleSpawn;

    private void Update()
    {
        timeUntilObstacleSpawn += Time.deltaTime;
        if (timeUntilObstacleSpawn >= obstacleSpawnTime)
        {
            Spawn();
            timeUntilObstacleSpawn = 0f;
        }
    }

    private void Spawn()
    {
        GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
        float chosenY = IsType1(prefab) ? -4f : spawnHeights[Random.Range(0, spawnHeights.Length)];

        Vector3 spawnPosition = new Vector3(transform.position.x, chosenY, 0f);
        GameObject spawned = Instantiate(prefab, spawnPosition, Quaternion.identity);

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
}
