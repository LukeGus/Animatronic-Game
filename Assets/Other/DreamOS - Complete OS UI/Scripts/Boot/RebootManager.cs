using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Michsky.DreamOS
{
    public class RebootManager : MonoBehaviour
    {
        // Resources
        [SerializeField] private GameObject mainCanvas;
        [SerializeField] private BootManager bootManager;

        // Resources
        [Range(0, 2f)] public float waitTime = 1.5f;

        bool isFirstTime = true;

        void OnEnable()
        {
            // Reboot the system if parent (such as Canvas) is disabled and enabled again. 
            if (isFirstTime == false)
                RecoverSystem();

            bootManager.onBootStart.Invoke();
            isFirstTime = false;
        }

        public void RebootSystem()
        {
            StartCoroutine("WaitForRestart");
        }

        public void RecoverSystem()
        {
            bootManager.StartCoroutine("BootEventStart");
        }

        public void ShutdownSystem()
        {
            StartCoroutine("WaitForShutdown");
        }

        public void RunSystem()
        {
            mainCanvas.SetActive(true);
        }

        public void WipeSystem()
        {
            StartCoroutine("WaitForWipe");
        }

        IEnumerator WaitForRestart()
        {
            yield return new WaitForSeconds(waitTime);
            mainCanvas.SetActive(false);
            mainCanvas.SetActive(true);

            bootManager.StartCoroutine("BootEventStart");
            StopCoroutine("WaitForRestart");
        }

        IEnumerator WaitForShutdown()
        {
            yield return new WaitForSeconds(waitTime);
            mainCanvas.SetActive(false);
        }

        IEnumerator BootScreenHelper(float timeMultiplier)
        {
            yield return new WaitForSeconds(timeMultiplier);
            bootManager.InvokeEvents();
        }
    }
}