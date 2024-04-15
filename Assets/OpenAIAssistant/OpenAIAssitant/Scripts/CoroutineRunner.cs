using System;
using System.Collections;
using UnityEngine;
using UHTTP;

namespace OpenAIAssistant
{
    public class CoroutineRunner : MonoBehaviour
    {
        private static CoroutineRunner instance;
        private static CoroutineRunner Instance =>
             instance ?? (instance = new GameObject(typeof(CoroutineRunner).Name).AddComponent<CoroutineRunner>());

        public static Coroutine Run(Action action,float delay) =>
            Instance.StartCoroutine(ActionRunner(action, delay));

        public static void Stop(Coroutine coroutine) =>
            Instance.StopCoroutine(coroutine);

        private static IEnumerator ActionRunner(Action action, float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            action?.Invoke();
        }
    }
}