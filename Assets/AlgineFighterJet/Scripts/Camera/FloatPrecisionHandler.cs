using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Algine.Aircraft
{
    public class FloatPrecisionHandler : MonoBehaviour
    {
        [SerializeField]
        private float thresold;
        [SerializeField]
        private Transform m_player;
        [SerializeField]
        private Transform[] m_MoveableEnvironments;
        void LateUpdate()
        {
            
            if (m_player.position.magnitude > thresold)
            {
                Vector3 pos = m_player.position;

                GameEvents.Current.OffsetChanged(pos);

                //move Environments
                foreach (var item in m_MoveableEnvironments)
                {
                    item.position -= m_player.position;
                }
                //move players
                m_player.position -= m_player.position;

                GameEvents.Current.AfterOriginChanged(pos);
            }

        }
    }
}

