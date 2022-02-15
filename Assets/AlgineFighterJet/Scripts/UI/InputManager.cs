using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Algine.Aircraft.UI
{
    public sealed class InputManager
    {
        public static Action<Vector2> OnTouchLook;
        public static Action<Vector2> OnSideStickMove;
        public static Action<float> OnRudderMove;
        public static Action<float> OnThrottleMove;
        public static Action<float> OnSetMinPOwer;

        public static void TouchLook(Vector2 dir)
        {
            OnTouchLook?.Invoke(dir);
        }
        public static void SideStickMove(Vector2 dir)
        {
            OnSideStickMove?.Invoke(dir);
        }
        public static void RudderMove(float val)
        {
            OnRudderMove?.Invoke(val);
        }
        public static void ThrottleMove(float val)
        {
            OnThrottleMove?.Invoke(val);
        }
        public static void SetMinPOwer(float val)
        {
            OnSetMinPOwer?.Invoke(val);
        }

    }
}

