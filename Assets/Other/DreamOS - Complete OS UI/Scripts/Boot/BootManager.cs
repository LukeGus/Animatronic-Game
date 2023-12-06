using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Michsky.DreamOS
{
    public class BootManager : MonoBehaviour
    {
        // Events
        public UnityEvent onBootStart;
        public UnityEvent eventsAfterBoot;

        // Resources
        [SerializeField] private UserManager userManager;

        // Settings
        [Range(0, 30)] public float bootTime = 3f;

        // Editor variables
#if UNITY_EDITOR
        public int currentEditorTab;
#endif

        void Start()
        {
            StartCoroutine("BootEventStart");
        }

        public void InvokeEvents()
        {
            StartCoroutine("BootEventStart");
        }

        IEnumerator BootEventStart()
        {
            yield return new WaitForSeconds(bootTime);

            userManager.lockScreen.gameObject.SetActive(true);
            eventsAfterBoot.Invoke();

            StopCoroutine("BootEventStart");
        }
    }
}