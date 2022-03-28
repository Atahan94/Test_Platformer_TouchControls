using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    delegate void OnReleased(Vector2Int direction);
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

    private Vector2Int dir = Vector2Int.right;

    public Vector2Int Direction
    {
        get { return dir; }
        private set { dir = value; }
    }

    public LayerMask[] layers;

    public float feetSize;
    public Transform feet; 
    public float handSize;
    public Transform hand;

    [SerializeField]
    bool isground, doubleJump;

   
    public float time;


    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        
        currentHealth = health;       
    }
    private void OnRelease(int time, Vector2Int direction, OnReleased rel)
    {
        if (time < 1)
            rel(direction);
    }
    internal void AttackController(Vector2Int direction, int time, bool onHold)
    {
        if (onHold)
            AttackOnHold(direction, time);
        else
            OnRelease(time, Direction, BasicAttack);
    }


    private void AttackOnHold(Vector2Int direction, int time)
    {
        if (time == 5) {PowerAttack(); return;}
                  
        if (direction.y < 0)
            Defend();
        else if(!direction.Equals(Vector2Int.zero))
            DashAttack(direction);// Adjust startpos on direciton axes
    }

    private void Defend()
    {
        Debug.Log("Defending");
    }

    private void DashAttack(Vector2Int direction)
    {
        Debug.Log("DashAttack" + "X:"+ direction.x + "Y:" + direction.y);
    }

    private void PowerAttack()
    {
        Debug.Log("PoweAttack");
    }

    private void BasicAttack(Vector2Int direction)
    {
        Debug.Log("BasicAttack");
    }

    internal void MoveController(Vector2Int direction, int time, bool onHold)
    {
        if (onHold)
            MoveOnHold(direction);
        else
            OnRelease(time, direction, Jump);
    
    }

    private void MoveOnHold(Vector2Int direction)
    {
        if(direction.Equals(Vector2Int.zero))
            return;

        if (Mathf.Abs(direction.x) > direction.y)
            Move(speed, direction);
        else 
        {
            if (direction.y > 0)
                Jump(direction);
            //else move down to platform below
        }
        
            
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
        StartCoroutine(ChangeColor(Color.red));
        if (currentHealth <= 0)
            this.Death();
    }

    private IEnumerator ChangeColor(Color red)
    {
        Color Cur = sr.color;
        sr.color = Color.red;
        yield return new WaitForSeconds(2f);
        sr.color = Cur;

    }

    private void Death()
    {
        StartCoroutine(ChangeColor(Color.black));
        Move(0, Vector2Int.zero);
        rb.isKinematic = true;
        this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }

    private void AddForceToPlayer(float damage, Vector2 direction, ForceMode2D mode)
    {
        rb.AddForce(damage * direction, mode);
    }

    private void Jump(Vector2Int direction)
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
    }
}
