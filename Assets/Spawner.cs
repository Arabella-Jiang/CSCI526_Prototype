using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Spawner : MonoBehaviour
{
    //tutorial
    [SerializeField] private GameObject[] tutorialObSequence;
    //tutorial variables
    public bool enableTutorial = true;
    public float tutorialcompletionDelay = 2f;
    private bool isTutorialMode = true;
    private int currentTutorialStep = 0;
    private bool tutorialCompleted = false;
    private float tutorialObSpawnTime = 5f;

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

    //Tutorial manager
    public System.Action<int> OnTutorialStepStarted; //tutorial begin
    public System.Action OnTutorialCompleted; //tutorial end

    private void Start()
    {
        initialObstacleSpeed = obstacleSpeed;

        //update: whether active tutorial mode
        if (enableTutorial && tutorialObSequence != null && tutorialObSequence.Length > 0)
        {
            StartTutorial();
        } else
        {
            StartNormalMode();
        }

        if (enableSpeedIncrease)
        {
            StartCoroutine(ContinuousSpeedIncrease());
        }
    }

    private void StartTutorial()
    {
        isTutorialMode = true;
        currentTutorialStep = 0;
        tutorialCompleted = false;
        obstacleSpeed = initialObstacleSpeed;
        Debug.Log("Tutorial mode started");
        OnTutorialStepStarted?.Invoke(currentTutorialStep);
    }

    //active normal mode after tutorial completed
    private void StartNormalMode()
    {
        isTutorialMode = false;
        tutorialCompleted = true;
        Debug.Log("Normal game mode started");
        OnTutorialCompleted?.Invoke();
    }


    private void Update()
    {
        timeUntilObstacleSpawn += Time.deltaTime;

        //adjust spawn time based on tutorial / normal game mode
        float currentSpawnTime = isTutorialMode ? tutorialObSpawnTime : obstacleSpawnTime;

        if (timeUntilObstacleSpawn >= currentSpawnTime && CanSpawnSafely())
        {
            Spawn();
            timeUntilObstacleSpawn = 0f;
        }

        //Check whether obstacle ob get out of the screen
        CheckObstaclesOutOfScreen();
    }

    //check whether is safe ot generate new ob to avoid overlap
    private float lastSpawnTime = 0f;
    private GameObject lastSpawnedOb = null;

    private bool CanSpawnSafely()
    {
        if (!isTutorialMode) return true;
        //safe & immediately generte if its the 1st ob
        if (lastSpawnedOb == null) return true;
        //check if last ob still exist
        if (lastSpawnedOb == null) return true;

        float timeSinceLastSpawn = Time.time - lastSpawnTime;
        float lastObstacleWidth = GetObWidth(lastSpawnedOb);
        float minWaitTime = lastObstacleWidth / obstacleSpeed;

        bool canSpawn = timeSinceLastSpawn >= minWaitTime;

        Debug.Log($"Time since last spawn: {timeSinceLastSpawn}, Min wait: {minWaitTime}, CanSpawn: {canSpawn}");

        return canSpawn;
    }

    private void Spawn()
    {
        GameObject prefabToSpawn;

        //update & fixed (only in tutorial mode) : too width ob will casue overlap to next ob
        float currentSpawnTime = isTutorialMode ? tutorialObSpawnTime : obstacleSpawnTime;

        //generate ob based on the mode
        if (isTutorialMode && currentTutorialStep < tutorialObSequence.Length)
        {
            prefabToSpawn = tutorialObSequence[currentTutorialStep];
            currentTutorialStep++;

            Debug.Log($"Tutorial step {currentTutorialStep} / {tutorialObSequence.Length}");

            //update tutorial step
            OnTutorialStepStarted?.Invoke(currentTutorialStep);

            //check if tutorial complete
            if (currentTutorialStep >= tutorialObSequence.Length)
            {
                StartCoroutine(CompleteTutorial());
            } 
        }
        else
        {
            prefabToSpawn = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
        }

        //double check prefabTospawn is assigned
        if (prefabToSpawn == null)
        {
            Debug.LogError("prefabToSpawn is null!");
            return;
        }
        
        // Spawn at Spawner's X position, keep prefab's original Y and Z positions
        Vector3 spawnPosition = new Vector3(transform.position.x, prefabToSpawn.transform.position.y, prefabToSpawn.transform.position.z);
        GameObject spawned = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

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

    //update & fixed (only in tutorial mode) : too width ob will casue overlap to next ob
    private float GetObWidth(GameObject obPrefab)
    {
        var renderer = obPrefab.GetComponentInChildren<Renderer>();
        if(renderer != null)
        {
            return renderer.bounds.size.x;
        }
        //TODO: check other ob types' default width???
        return 2f;
    }



    private IEnumerator CompleteTutorial()
    {
        Debug.Log("Tutorial completed, transitioning to normal mode");

        //wait for all tutorial ob pass = not change mode immedietly
        yield return new WaitForSeconds(tutorialcompletionDelay);

        isTutorialMode = false;
        tutorialCompleted = true;

        OnTutorialCompleted?.Invoke();
        Debug.Log("Now in normal game mode");

    }

    public void SkipTutorial()
    {
        if (isTutorialMode && !tutorialCompleted)
        {
            StopAllCoroutines();
            StartNormalMode();
        }
    }

    public void RestartTutorial()
    {
        StopAllCoroutines();
        StartTutorial();
    }

    //check if in tutorial mode
    public bool IsInTutorialMode()
    {
        return isTutorialMode && !tutorialCompleted;
    }

    public (int currentStep, int totalSteps) GetTutorialProgress()
    {
        return (currentTutorialStep, tutorialObSequence.Length);
    }

    
    private IEnumerator ContinuousSpeedIncrease()
    {
        while (enableSpeedIncrease)
        {
            yield return new WaitForSeconds(speeedIncreaseInterval);

            //update: increase only in normal mode, not in tutorial mode
            if (!isTutorialMode)
            {
                IncreaseSpeed();
            }
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
        //update: increase speed only in normal mode, not for tutorial mode
        if (!isTutorialMode)
        {
            obstacleSpeed += speedIncrement;
            UpdateActiveObstaclesSpeed();
            Debug.Log($"Obstacle speed increased to: {obstacleSpeed}");
        }
       
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
        //update: increase speed only in normal mode, not for tutorial mode
        if (!isTutorialMode)
        {
            obstacleSpeed += amount;
            UpdateActiveObstaclesSpeed();
        }
        
    }


}





