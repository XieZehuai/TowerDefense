using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    /// <summary>
    /// 执行各种常用方法以及扩展方法的工具类
    /// </summary>
    public static class Utils
    {
        #region 常量
        public const int ENEMY_LAYER_MASK = 1 << 8;
        public const int MAP_CELL_SIZE = 1;
        public const float GRAVITY = 20f;
        #endregion

        #region 各种常用方法
        /// <summary>
        /// 获取世界坐标内鼠标点击的位置
        /// </summary>
        /// <returns>鼠标点击位置的世界坐标</returns>
        public static Vector3 GetMousePosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                return hit.point;
            }

            return Vector3.zero;
        }
        #endregion

        #region MonoBehaviour扩展方法
        /// <summary>
        /// 延迟一段时间执行事件
        /// </summary>
        /// <param name="action">要执行的事件</param>
        /// <param name="delay">延迟时间</param>
        public static Coroutine Invoke(this MonoBehaviour obj, Action action, float delay)
        {
            return obj.StartCoroutine(DoInvoke(action, new WaitForSeconds(delay)));
        }

        /// <summary>
        /// 按延迟条件延迟执行事件
        /// </summary>
        /// <param name="action">要执行的事件</param>
        /// <param name="condition">延迟条件</param>
        public static Coroutine Invoke(this MonoBehaviour obj, Action action, YieldInstruction condition)
        {
            return obj.StartCoroutine(DoInvoke(action, condition));
        }

        private static IEnumerator DoInvoke(Action action, YieldInstruction condition)
        {
            yield return condition;
            action?.Invoke();
        }
        #endregion

        #region 集合扩展方法
        /// <summary>
        /// 快速从List中删除一个元素，删除后会打乱原有顺序
        /// </summary>
        /// <param name="index">要删除元素的索引</param>
        public static void QuickRemove<T>(this List<T> list, int index)
        {
            int last = list.Count - 1;

            if (last > 1 && index != last)
            {
                T temp = list[index];
                list[index] = list[last];
                list[last] = temp;
                list.RemoveAt(last);
            }
            else
            {
                list.RemoveAt(index);
            }
        }

        public static T[] SubArray<T>(this T[] arr, int start, int length)
        {
            T[] subArr = new T[length];

            for (int i = start; i < start + length; i++)
            {
                subArr[i - start] = arr[i];
            }

            return subArr;
        }
        #endregion
    }
}
