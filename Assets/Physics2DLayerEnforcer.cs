using UnityEngine;

public class Physics2DLayerEnforcer : MonoBehaviour
{
    void Awake()
    {
        int PL = LayerMask.NameToLayer("Player-light");
        int PS = LayerMask.NameToLayer("Player-shadow");
        int OL = LayerMask.NameToLayer("Obstacle-light");
        int OS = LayerMask.NameToLayer("Obstacle-shadow");

        if (PL==-1||PS==-1||OL==-1||OS==-1) { Debug.LogError("[LayerEnforcer] 层名不全"); return; }

        Physics2D.IgnoreLayerCollision(PS, OL, true); 
        Physics2D.IgnoreLayerCollision(PL, OS, true);

        Debug.Log($"[LayerEnforcer] ignore(PS,OL)={Physics2D.GetIgnoreLayerCollision(PS,OL)}  ignore(PL,OS)={Physics2D.GetIgnoreLayerCollision(PL,OS)}");
    }
}
