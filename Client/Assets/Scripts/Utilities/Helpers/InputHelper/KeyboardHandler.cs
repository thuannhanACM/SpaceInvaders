using System;
using UnityEngine;

namespace Core.Framework.Helpers
{
    public class KeyboardHandler :ICoreInputHandler
    {
        public Action OnDown;
        public Action OnUp;
        public Action OnLeft;
        public Action OnRight;
        public Vector2 Direction { get; private set; }

        public KeyboardHandler()
        {           
        }

        public KeyboardHandler(Action down, Action up, Action left, Action right)
        {
            OnDown += down;
            OnUp += up;
            OnLeft = left;
            OnRight = right;
        }

        public void HandleKeyInput()
        {
            Direction = Vector2.zero;

            if ( Input.GetKey("up") || Input.GetKey(KeyCode.W))
            {
                if ( OnUp != null )
                    OnUp();
                Direction += Vector2.up;
            }

            if ( Input.GetKey("down") || Input.GetKey(KeyCode.S))
            {
                if ( OnDown != null )
                    OnDown();
                Direction += Vector2.down;
            }

            if ( Input.GetKey("left") || Input.GetKey(KeyCode.A))
            {
                if ( OnLeft != null )
                    OnLeft();
                Direction += Vector2.left;
            }

            if ( Input.GetKey("right") || Input.GetKey(KeyCode.D))
            {
                if ( OnRight != null )
                    OnRight();
                Direction += Vector2.right;
            }
           
        }

    }
}
