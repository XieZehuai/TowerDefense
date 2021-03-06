using System;
using CodeMonkey.Utils;
using UnityEngine;
using TowerDefense;

namespace TowerDefense.Test
{
    public class Testing : MonoBehaviour
    {
        private Grid<int> grid;
        
        private void Awake()
        {
            grid = new Grid<int>(4, 2, 1);
        }

        private void Start()
        {
            UIManager.Instance.Open<UIMainScene>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 pos = Input.mousePosition;
                grid.SetValue(UtilsClass.GetMouseWorldPosition(pos), 3);
            }
        }
    }
}
