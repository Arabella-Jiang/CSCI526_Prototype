using UnityEngine;

public class SimpleInputHandler : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (SimpleGameManager.Instance != null)
                SimpleGameManager.Instance.TogglePause();
        }
    }
}
