using UnityEngine;

public class Physics2DLayerEnforcer : MonoBehaviour
{
    void Awake()
    {
        int PL = LayerMask.NameToLayer("Player-light");
        int PS = LayerMask.NameToLayer("Player-shadow");
        int OL = LayerMask.NameToLayer("Obstacle-light");
        int OS = LayerMask.NameToLayer("Obstacle-shadow");

        if (PL==-1||PS==-1||OL==-1||OS==-1) { Debug.LogError("[LayerEnforcer] Missing layer names"); return; }

        // Reversed logic: same color passes through, different color collides
        Physics2D.IgnoreLayerCollision(PS, OS, true);  // Player-shadow passes through Obstacle-shadow
        Physics2D.IgnoreLayerCollision(PL, OL, true);  // Player-light passes through Obstacle-light

        Debug.Log($"[LayerEnforcer] ignore(PS,OS)={Physics2D.GetIgnoreLayerCollision(PS,OS)}  ignore(PL,OL)={Physics2D.GetIgnoreLayerCollision(PL,OL)}");
    }
}
