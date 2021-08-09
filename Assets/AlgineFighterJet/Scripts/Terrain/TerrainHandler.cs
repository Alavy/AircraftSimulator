using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Algine.Aircraft
{
    public class TerrainHandler : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_terrain_mid;

        [SerializeField]
        private float m_terrain_width = 3000f;
        [SerializeField]
        private float m_terrain_height = 3000f;

        [SerializeField]
        private float thresold = 2500;

        private GameObject[,] tiles;
        private int halfTiles = 3;

        private void Start()
        {

            GameEvents.Current.onAfterOriginChanged += OnAfterOriginChanged;

        }
		private void OnAfterOriginChanged(Vector3 playerPos)
        {
            
            
        }
        private void OnDestroy()
        {
            GameEvents.Current.onOffsetChanged -= OnAfterOriginChanged;
        }
    }
}

