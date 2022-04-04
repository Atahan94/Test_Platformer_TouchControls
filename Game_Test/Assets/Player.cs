using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Move,
        Jump,
        BasicAttack,
        PowerAttack,
        DashAttack,
        Defend,
        Hurt,
        Death

    }
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

    PlayerState currentState;
    public float time;


    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        
        currentHealth = health;
        currentState = PlayerState.Idle;
    }

 
    private void Defend()
    {
        Debug.Log("Defending");
        currentState = PlayerState.Defend;
    }
    public void DashAttack(Vector2Int direction)
    {
        Debug.Log("DashAttack" + "X:"+ direction.x + "Y:" + direction.y);
        currentState = PlayerState.DashAttack; //Adjusr Deazone on direction axis
    }
    public void PowerAttack()
    {
        Debug.Log("PoweAttack");
        currentState = PlayerState.PowerAttack;
    }
    public void BasicAttack()
    {
        Debug.Log("BasicAttack");
        currentState = PlayerState.BasicAttack;
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

    public void Jump(Vector2Int direction)
    {
        if (IsJumpAvaible())
            Debug.Log("Jump" + direction);
        currentState = PlayerState.Jump;//Adjusr Deazone on direction axis
        AddVelocity(speed, direction);
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
    public void Move(Vector2Int direction)
    {
        Debug.Log("Move");
        currentState = PlayerState.Move;
        AddVelocity(speed, direction);
    }

    private void AddVelocity(float speed, Vector2Int direction)
    {
        float s = speed * Time.deltaTime;
        rb.velocity = new Vector2(direction.x * s, direction.y * s);
    }
    private void AddForceToPlayer(float damage, Vector2 direction, ForceMode2D mode)
    {
        rb.AddForce(damage * direction, mode);
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
    private void Death()
    {
        StartCoroutine(ChangeColor(Color.black));
        Move(Vector2Int.zero);
        rb.isKinematic = true;
        this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }

    private IEnumerator ChangeColor(Color red)
    {
        Color Cur = sr.color;
        sr.color = Color.red;
        yield return new WaitForSeconds(2f);
        sr.color = Cur;

    }

   

   
}
