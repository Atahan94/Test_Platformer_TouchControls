using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class TouchControls : MonoBehaviour
{
   public class MyTouch
    {
        Action<Vector2Int, int> action;
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

        public static void InitializeTouch(MyTouch mt) 
        {
            mt.SetAction();
            mt.SetStartPos(Vector2Int.RoundToInt(mt.myTouch.position));
        }
        private void SetAction() 
        {
            if (touchId != -1)
            {
                if (touchId == 0)
                    action = player.MoveController;
                else
                    action = player.AttackController;
            }
            else
                action = null;
        }
        public void SetStartPos(Vector2Int pos)
        {
            startPos = pos;
           
        }

        internal void CallAction(Vector2Int pos)
        {
            
               this.action?.Invoke(pos, Mathf.RoundToInt(touchTimer));

        }
        public Vector2Int CheckGetDirection() 
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

        public static void ResetMyTouch(MyTouch mt) 
        {
            mt.touchId = -1;
            mt.touchTimer = 0;
            mt.action = null;
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

    
    
    // Start is called before the first frame update
    void Start()
    {
        player = this.gameObject.GetComponent<Player>();
        MyInputs = new MyTouch[2] { new MyTouch(deadZone, player), new MyTouch(deadZone, player) };
    }

    private void CheckTouchPhase(MyTouch mt)
    {
        mt.touchTimer += Time.deltaTime;

        switch (mt.GetMyTouchPhase())
        {
            case TouchPhase.Began:
                MyTouch.InitializeTouch(mt);
                if (mt.touchId == 1)
                    mt.CallAction(Vector2Int.zero);
                 
                break;
            case TouchPhase.Stationary:
                mt.CallAction(mt.CheckGetDirection());
                break;
            case TouchPhase.Ended:
                if (mt.touchId == 0)
                    mt.CallAction(Vector2Int.up);
                MyTouch.ResetMyTouch(mt);
                Debug.Log("TouchEnded");
                break;
            case TouchPhase.Canceled:
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
