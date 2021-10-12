using System;
using UnityEngine;

namespace Network
{
    public class OVRseerTinyStartPosition : MonoBehaviour
    {
        private void Awake()
        {
            PlayerSpawnSystem.AddTinySpawnPoint(transform);
        }

        private void OnDestroy()
        {
            PlayerSpawnSystem.AddTinySpawnPoint(transform);
        }
    }
}