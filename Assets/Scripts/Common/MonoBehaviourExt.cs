using System;
using System.Collections;
using UnityEngine;

namespace TowerDefense
{
    public static class MonoBehaviourExt
    {
        public static void Invoke(this MonoBehaviour obj, Action action, float delay)
        {
            obj.StartCoroutine(DoInvoke(action, new WaitForSeconds(delay)));
        }

        public static void Invoke(this MonoBehaviour obj, Action action, YieldInstruction condition)
        {
            obj.StartCoroutine(DoInvoke(action, condition));
        }

        private static IEnumerator DoInvoke(Action action, YieldInstruction condition)
        {
            yield return condition;
            action?.Invoke();
        }
    }
}
