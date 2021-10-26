using System;
using UnityEngine;

namespace Network
{
    public class OVRseerOverseerStartPosition : MonoBehaviour
    {
        private void Awake()
        {
            PlayerSpawnSystem.AddOverseerSpawnPoint(transform);
        }

        private void OnDestroy()
        {
            PlayerSpawnSystem.AddOverseerSpawnPoint(transform);
        }
    }
}