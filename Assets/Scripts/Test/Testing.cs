using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense.Test
{
    public class Testing : MonoBehaviour
    {
        private void Awake()
        {
            List<int> t = new List<int>();
            for (int i = 0; i < 10; i++)
            {
                t.Add(i);
            }

            for (int i = 0; i < t.Count; i++)
            {
                t.QuickRemove(i--);
            }

            for (int i = 0; i < t.Count; i++)
            {
                Debug.Log(t[i]);
            }
        }
    }
}
