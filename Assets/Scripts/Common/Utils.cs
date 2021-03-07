using System;
using UnityEngine;

namespace TowerDefense
{
    public static class Utils
    {
        private static Camera mainCamera = Camera.main;

        public static Vector3 GetMousePosition()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                return hit.point;
            }

            return Vector3.zero;
        }
    }
}
