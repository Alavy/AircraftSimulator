using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Algine.Aircraft
{
    public sealed class GameEvents
    {
        private static GameEvents m_current;

        public static GameEvents Current { get {
                if (m_current == null)
                {
                    m_current = new GameEvents();
                    return m_current;
                }
                else
                {
                    return m_current;
                }
            } 
        }

        public Action onAircraftRotationChanged;
        public Action<Vector3> onOffsetChanged;
        public Action<Vector3> onAfterOriginChanged;
        public Action<Vector3> onTargetLock;
        public Action onTargetUnLock;

        public void TargetLock(Vector3 targetPos)
        {
            onTargetLock?.Invoke(targetPos);
        }
        public void TargetUnLock()
        {
            onTargetUnLock?.Invoke();
        }
        public void AfterOriginChanged(Vector3 offset)
        {
            onAfterOriginChanged?.Invoke(offset);
        }

        public void OffsetChanged(Vector3 offset)
        {
            onOffsetChanged?.Invoke(offset);
        }
        public void AircraftRotationChanged()
        {
            onAircraftRotationChanged?.Invoke();
        }
    }

}
