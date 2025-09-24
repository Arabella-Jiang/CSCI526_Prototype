using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerCollision : MonoBehaviour
{
    int L_PL, L_PS, L_OL, L_OS;
    int score = 0;

    void Awake()
    {
        L_PL = LayerMask.NameToLayer("Player-light");
        L_PS = LayerMask.NameToLayer("Player-shadow");
        L_OL = LayerMask.NameToLayer("Obstacle-light");
        L_OS = LayerMask.NameToLayer("Obstacle-shadow");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        int me = gameObject.layer, ot = other.gameObject.layer;
        // Reversed logic: different colors cause death
        if ((me == L_PL && ot == L_OS) || (me == L_PS && ot == L_OL))
        {
            Destroy(gameObject); //Destroy obstacle
            // TODO: GM/HP
        }
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        int me = gameObject.layer;
        int ot = c.collider.gameObject.layer;

        bool isObstacle = (ot == L_OL || ot == L_OS);
        bool isMatchedPair =
            (me == L_PL && ot == L_OL) ||   
            (me == L_PS && ot == L_OS);    

        // Reversed logic: same colors pass through
        if (isObstacle && isMatchedPair)
        {
            foreach (var myCol in GetComponentsInChildren<Collider2D>())
                Physics2D.IgnoreCollision(myCol, c.collider, true);
        }
    }
}
