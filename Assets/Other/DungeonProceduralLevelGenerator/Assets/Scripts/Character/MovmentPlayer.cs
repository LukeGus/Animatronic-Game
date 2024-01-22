using UnityEngine;
public class MovmentPlayer : MonoBehaviour
{
    public float moveSpeed = 10f;

    Vector2 movement;

    public Animator anim;
    float MoveSpeedDiaganal;
    float MoveSpeedDef;
    Rigidbody2D rb;
    
    private void Start()
    {
        MoveSpeedDiaganal = moveSpeed / 1.414f;
        MoveSpeedDef = moveSpeed;
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
            
        if (movement.magnitude > 1)
            moveSpeed = MoveSpeedDiaganal;
        else
            moveSpeed = MoveSpeedDef;

        anim.SetFloat("MoveX", movement.x);
        anim.SetFloat("MoveY", movement.y);
        anim.SetFloat("Magnitude", movement.magnitude);
        //anim.SetFloat("MouseX", transform.forward.x);
        //anim.SetFloat("MouseY", transform.forward.y);

    }


    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * (moveSpeed) * Time.fixedDeltaTime);
    }

}
