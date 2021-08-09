using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Algine.Aircraft
{
    
    public class ObjectSpawner : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] m_objects;
        private int index = 0;
        [SerializeField]
        private float m_distance= 30f;
        private void Start()
        {
           foreach(var item in PoisonDisc.GeneratePoints(m_distance, new Vector2(300, 300)))
           {
                var temp = Instantiate(m_objects[Random.Range(0, m_objects.Length)], transform);
                temp.transform.localPosition = new Vector3 { x = item.x, y = 0, z = item.y };
           }
        }
    }
}

