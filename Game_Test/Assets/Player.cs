using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    public bool isActive;

    SpriteRenderer sr;
    Rigidbody2D rb;
    [SerializeField]
    float speed, jumpSpeed;

    [SerializeField]
    bool Enemy;

    [SerializeField]
    float health;

    float currentHealth;

    

    private Vector2Int direction = Vector2Int.right;

    public Vector2Int Direction
    {
        get { return direction; }
        private set { direction = value; }
    }


    Touch[] t;
    Vector2Int LastPos;
    int touchCount;
    [SerializeField]
    float deadZone;

    public LayerMask[] layers;

    public float feetSize;
    public Transform feet; 
    public float handSize;
    public Transform hand;

    [SerializeField]
    bool isground, doubleJump;

    public Text tex;
    public float time;


    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        

        currentHealth = health;
        
    }

    internal void AttackController(Vector2Int direction, int time)
    {
        if (direction == Vector2Int.down)
        {
            Defend();
            return;
        }
        else if (direction.Equals(Vector2Int.zero))
        {
            DashAttack(direction);  // adjust direction deadzone
            return;
        }

        //Debug.Log("Attack" + "direction" + direction + "Time" + time);
        if (time < 1)
        {
            BasicAttack();
        }
        else if (time == 5)
        {
            PowerAttack();
        }
    }

    private void Defend()
    {
        Debug.Log("Defending");
    }

    private void DashAttack(Vector2Int direction)
    {
        Debug.Log("DashAttack" + direction);
    }

    private void PowerAttack()
    {
        Debug.Log("PoweAttack");
    }

    private void BasicAttack()
    {
        Debug.Log("BasicAttack");
    }

    internal void MoveController(Vector2Int direction, int time)
    {
        Debug.Log("Moved" + "direction" + direction + "Time" + time);
        //if (time < 1 && direction.Equals(Vector2Int.zero))
        //    Jump(Vector2Int.up, 1f);
        //else if(direction.Equals(Vector2Int.Up))
        //    Jump(direction, 1f)
        //else if(!direction.Equals(Vector2Int.down))
        //    Move(speed, direction)

    }
    private bool CheckEnemy(out Player p)
    {
        
       BoxCollider2D col = Physics2D.OverlapCircle(hand.position, handSize, layers[1]) as BoxCollider2D;
        if (col == this.gameObject.GetComponent<BoxCollider2D>() || col == null) 
        {
            p = null;
            return false;
        }
        else
            p = col.GetComponent<Player>();
            return true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(feet.position, feetSize);
        Gizmos.DrawWireSphere(hand.position, handSize);
    }
   
    
    private void Damage(float damage1, float damage2, Vector2 direction)
    {
        float damage = UnityEngine.Random.Range(damage1, damage2);
        currentHealth -= damage;
        AddForceToPlayer(0.5f, direction, ForceMode2D.Impulse);
        sr.color = Color.red;
        if (currentHealth <= 0)
            this.Death();
    }

    private void Death()
    {
        sr.color = Color.black;
        Move(0, Vector2Int.zero);
        rb.isKinematic = true;
        this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }

    private void AddForceToPlayer(float damage, Vector2 direction, ForceMode2D mode)
    {
        rb.AddForce(damage * direction, mode);
    }

    private void Jump(Vector2Int direction ,float jumpSped)
    {
        if (IsJumpAvaible())
            Debug.Log("Jump" + direction);
            //rb.velocity = new Vector2(0, jumpSped);
    }
    bool IsJumpAvaible()
    {  //Check the ground with ovelap circle here!!!!
        if (isground) 
        {
            Debug.Log("firstJ");
            doubleJump = true;
            return true;
        }
        else if (doubleJump)
        {
            Debug.Log("secondJ");
            doubleJump = false;
            return true;  
        }
    return false;      
    }

    private void Move(float speed, Vector2Int direction)
    {
        Debug.Log("Move");
        //speed += speed * Time.deltaTime;
        //rb.velocity = new Vector2(speed * direction.x, rb.velocity.y);
       // if(direction.x != 0)
           // this.gameObject.transform.localScale = new Vector3(this.gameObject.transform.localScale.x * direction.x, this.gameObject.transform.localScale.y, this.gameObject.transform.localScale.z);
    }
}
