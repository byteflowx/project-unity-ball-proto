using System.Collections.Generic;
using UnityEngine;

namespace App.Scripts.Composition.Components
{
    public sealed class DontDestroyOnLoadComponent : MonoBehaviour
    {
        private static readonly HashSet<string> _instances = new();

        private string _registeredKey;

        private void Awake()
        {
            string key = gameObject.name;

            if (_instances.Add(key))
            {
                _registeredKey = key;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (_registeredKey != null)
                _instances.Remove(_registeredKey);
        }
    }
}
