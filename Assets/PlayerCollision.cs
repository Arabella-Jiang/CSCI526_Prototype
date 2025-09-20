using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerCollision : MonoBehaviour
{
    int L_PL, L_PS, L_OL, L_OS;

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
        if ((me == L_PL && ot == L_OL) || (me == L_PS && ot == L_OS))
        {
            Destroy(gameObject); // TODO: GM/HP
        }
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        int me = gameObject.layer;
        int ot = c.collider.gameObject.layer;

        bool isObstacle = (ot == L_OL || ot == L_OS);
        bool isMismatchedPair =
            (me == L_PL && ot == L_OS) ||   
            (me == L_PS && ot == L_OL);    

        if (isObstacle && isMismatchedPair)
        {
            foreach (var myCol in GetComponentsInChildren<Collider2D>())
                Physics2D.IgnoreCollision(myCol, c.collider, true);
        }
    }
}
