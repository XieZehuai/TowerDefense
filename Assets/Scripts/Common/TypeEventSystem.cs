using System;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    /// <summary>
    /// 基于类型的事件系统
    /// </summary>
    public static class TypeEventSystem
    {
        // 以参数类型作为Key，用HashSet作为Value，保存每一种类型的事件
        private static readonly Dictionary<Type, HashSet<Delegate>> actions = new Dictionary<Type, HashSet<Delegate>>();

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="action">对应参数类型的委托</param>
        public static void Register<T>(Action<T> action)
        {
            Type type = typeof(T);

            if (!actions.ContainsKey(type))
            {
                actions[type] = new HashSet<Delegate> { action };
            }
            else
            {
                if (!actions[type].Contains(action))
                {
                    actions[type].Add(action);
                }
                else
                {
                    Debug.LogWarning("已经注册了该事件" + action.ToString());
                }
            }
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="action">对应参数类型的委托</param>
        public static void UnRegister<T>(Action<T> action)
        {
            Type type = typeof(T);

            if (!actions.ContainsKey(type) || !actions[type].Contains(action))
            {
                Debug.LogWarning("没有注册该事件" + action.ToString());
            }
            else
            {
                actions[type].Remove(action);
            }
        }

        /// <summary>
        /// 发送消息给所有已经注册的对应参数类型的委托
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="context">对应参数类型的数据</param>
        public static void Send<T>(T context)
        {
            Type type = typeof(T);

            if (!actions.ContainsKey(type))
            {
                Debug.LogWarning("没有注册该类型的事件" + type);
                return;
            }

            HashSet<Delegate> set = actions[type];
            foreach (Delegate item in set)
            {
                Action<T> action = (Action<T>)item;
                action(context);
            }
        }
    }
}