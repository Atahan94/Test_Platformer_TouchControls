using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class TouchControls : MonoBehaviour
{
   public class MyTouch
    {
        
        static Player player;
        static float deadZoneOffset;
        public static int myTouchCount = 0;
 

        private Touch myTouch;
        private Vector2Int startPos;

        public int touchId;
        public float touchTimer;

        public MyTouch(float deadZone, Player p)
        {
            deadZoneOffset = deadZone;
            player = p;
            touchId = -1;
        }
        public void SetTouch(Touch t, int index) 
        {
            if (t.tapCount > 0)
            {
                if (touchId == -1)
                    myTouchCount++;
                  
                this.touchId = index;
                this.myTouch = t;
            }
            else
            {
                if (touchId != -1 &&--myTouchCount < 0)
                    myTouchCount = 0;
                touchId = -1;
            }
            
        }
      
        private void SetStartPos(Vector2Int pos)
        {
            startPos = pos;
           
        }

        public Vector2Int GetDirection() 
        {
            Vector2Int dir = Vector2Int.RoundToInt(myTouch.position) - startPos;

            if (dir.Equals(Vector2Int.zero))
                return Vector2Int.zero;
            
            //Debug.Log("FirstPos: " + Vector2Int.RoundToInt(myTouch.position) + "StartPos: " + startPos + "Diference: " + dir);
            if (dir.x != dir.y)
            {
                return Mathf.Abs(dir.x) > Mathf.Abs(dir.y) ? CheckDeadZonepassed(x: dir.x) : CheckDeadZonepassed(y: dir.y);
            }
            else
                return Vector2Int.up;
        }
        private Vector2Int CheckDeadZonepassed(int x = 0, int y = 0)
        {
            if (Mathf.Abs(x) > deadZoneOffset)
            {
                return x > 0 ? Vector2Int.right : Vector2Int.left;
            }
            else if (Mathf.Abs(y) > deadZoneOffset)
            {
                return y > 0 ? Vector2Int.up : Vector2Int.down;
            }
            else
                return Vector2Int.zero;
        }
        private void SetStartPosAxis(bool isX)
        {
            if (isX)
                startPos.x = (int)myTouch.position.x;
            else
                startPos.x = (int)myTouch.position.x;
        }

        public static void InitializeTouch(MyTouch mt)
        {
            mt.SetStartPos(Vector2Int.RoundToInt(mt.myTouch.position));
        }
        public static void ResetMyTouch(MyTouch mt) 
        {
            mt.touchId = -1;
            mt.touchTimer = 0;
            mt.startPos = Vector2Int.zero;
            if (myTouchCount > 0)
                myTouchCount--;
        }
       

        internal TouchPhase GetMyTouchPhase()
        {
            return this.myTouch.phase;
        }

    }

     static MyTouch[] MyInputs;
     static int touchCount;
     
     [SerializeField]
     float deadZone;

     Player player;

    
    static Text tex;
    
    // Start is called before the first frame update
    void Start()
    {
        player = this.gameObject.GetComponent<Player>();
        MyInputs = new MyTouch[2] { new MyTouch(deadZone, player), new MyTouch(deadZone, player) };
        tex = GameObject.Find("Canvas/Text").GetComponent<Text>();
    }

    private void CheckTouchPhase(MyTouch mt)
    {
        mt.touchTimer += Time.deltaTime;

        switch (mt.GetMyTouchPhase())
        {
            case TouchPhase.Began:
                MyTouch.InitializeTouch(mt);
                break;
            case TouchPhase.Stationary:
                OnHold(mt);
                Debug.Log(mt.GetMyTouchPhase());
                break;
            case TouchPhase.Ended:
                OnRelease(mt);
                MyTouch.ResetMyTouch(mt);
                Debug.Log("TouchEnded");
                break;
            case TouchPhase.Canceled:
                break;
        }
    }

    private void OnRelease(MyTouch mt)
    {
        if (mt.touchTimer < 0.5f)
            switch (mt.touchId)
            {
                case 0:
                    player.Jump(Vector2Int.up);
                    break;
                case 1:
                    player.BasicAttack();
                    break;
            }

    }

    private void OnHold(MyTouch mt)
    {
        Vector2Int dir = mt.GetDirection();

       
            switch (mt.touchId)
            {
                case 0:
                    if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y) || dir.Equals(Vector2Int.zero))
                        player.Move(dir);
                    else
                        player.Jump(dir);// Reset Deadzone on the dirction axis
                    break;
                case 1:
                    if (mt.touchTimer == 5)
                        player.PowerAttack();
                    else if(!dir.Equals(Vector2Int.zero))
                        player.DashAttack(dir);// Reset Deadzone on the dirction axis
                    break;
            }
       
    }

    bool GetTouches(Touch[] current, MyTouch[] myTouches, out int count) 
    {   
     
        MyInputs[0].SetTouch(current.FirstOrDefault<Touch>(x => x.position.x < Screen.width / 2), 0); 
        MyInputs[1].SetTouch(current.FirstOrDefault<Touch>(x => x.position.x > Screen.width / 2), 1);

        count = MyTouch.myTouchCount;
       
        return count > 0? true : false;
    }
    private void FixedUpdate()
    {
        if(GetTouches(Input.touches, MyInputs, out touchCount)) 
        {
            if(touchCount == 1)
                CheckTouchPhase(MyInputs.First<MyTouch>(x => x.touchId != -1));
            else
                for (int i = 0; i < touchCount; i++)
                {
                    if (MyInputs[i].touchId != -1)
                        CheckTouchPhase(MyInputs[i]);
                }
        }
        
    }
}
