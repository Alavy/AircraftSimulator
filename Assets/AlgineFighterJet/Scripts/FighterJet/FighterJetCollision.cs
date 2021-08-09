using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Algine.FighterJet
{
    public class FighterJetCollision : MonoBehaviour
    {
        [SerializeField]
        private Transform[] flamesPoint;

        [SerializeField]
        private GameObject flamePrefabs;

        private GameObject tempFlameObect;
        private bool isReadyToFlame=true;
        private void Start()
        {
            tempFlameObect = Instantiate(flamePrefabs);
            tempFlameObect.SetActive(false);
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.GetContact(0).thisCollider.CompareTag("AircraftParent"))
            {
                if (isReadyToFlame)
                {
                    Vector3 point = collision.GetContact(0).point;
                    Transform closePoint = flamesPoint[0];
                    float smallMatitude = (closePoint.position - point).magnitude;
                    foreach (var item in flamesPoint)
                    {
                        if ((item.position - point).magnitude <=
                            smallMatitude)
                        {
                            smallMatitude = (item.position - point).magnitude;
                            closePoint = item;
                        }
                    }
                    tempFlameObect.transform.parent = closePoint;
                    tempFlameObect.transform.localPosition = Vector3.zero;
                    tempFlameObect.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    tempFlameObect.SetActive(true);
                    isReadyToFlame = false;
                }
                //Debug.Log("---->collision");
            }
        }
    }
}   
