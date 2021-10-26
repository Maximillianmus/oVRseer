using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Network
{
    public class KeyStartPos : MonoBehaviour
    {

        private void Awake()
        {
            KeySpawnSystem.AddKeySpawnPoint(transform);
        }

        private void OnDestroy()
        {
            KeySpawnSystem.AddKeySpawnPoint(transform);
        }

    }
}
