using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace Michsky.DreamOS
{
    public class UserManager : MonoBehaviour
    {
        // Resources
        public BootManager bootManager;
        public Animator setupScreen;
        public Animator lockScreen;
        public Animator desktopScreen;
        public ProfilePictureLibrary ppLibrary;
        public GameObject ppItem;
        public Transform ppParent;

        // Content
        [Range(1, 20)] public int minEmailCharacter = 1;
        [Range(1, 20)] public int maxEmailCharacter = 14;

        [Range(1, 20)] public int minPasswordCharacter = 4;
        [Range(1, 20)] public int maxPasswordCharacter = 16;

        // Settings
        public bool disableUserCreating = false;
        public bool saveProfilePicture = true;
        public bool deletePrefsAtStart = false;
        public int ppIndex;

        // Multi Instance Support
        public bool allowMultiInstance;
        public string machineID = "DreamOS";

        // User variables
        [HideInInspector] public string email;
        [HideInInspector] public string password;
        
        [HideInInspector] public bool emailOK;
        [HideInInspector] public bool passwordOK;
        [HideInInspector] public bool passwordRetypeOK;
        [HideInInspector] public int userCreated;

        [HideInInspector] public bool isLockScreenOpen = false;

        [HideInInspector] public List<GetUserInfo> guiList = new List<GetUserInfo>();
        
        // Other
        private bool canPlayLockScreenIn = false;

        void Awake()
        {
            // Delete given prefs if option is enabled
            if (deletePrefsAtStart == true) { PlayerPrefs.DeleteAll(); }

            // Check for multi instance support
            if (allowMultiInstance == false) { machineID = "DreamOS"; }

            InitializeUserManager();
        }

        public void InitializeUserManager()
        {
            // Find Boot manager in the scene
            if (bootManager == null)
                bootManager = (BootManager)GameObject.FindObjectsOfType(typeof(BootManager))[0];

            if (disableUserCreating == false)
            {
                userCreated = PlayerPrefs.GetInt(machineID + "User" + "Created");
                email = PlayerPrefs.GetString(machineID + "User" + "Email");
                password = PlayerPrefs.GetString(machineID + "User" + "Password");

                if (!PlayerPrefs.HasKey(machineID + "User" + "ProfilePicture"))
                {
                    ppIndex = 0;
                    PlayerPrefs.SetInt(machineID + "User" + "ProfilePicture", ppIndex);
                }
                else { ppIndex = PlayerPrefs.GetInt(machineID + "User" + "ProfilePicture"); }

                // If user is not created, show Setup screen
                if (userCreated == 0)
                {
                    bootManager.enabled = false;
                    setupScreen.gameObject.SetActive(true);
                    setupScreen.Play("Panel In");
                }
                else { BootSystem(); }
            }

            else
            {
                BootSystem();
            }
        }
        
        public void ChangeEmailTMP(TMP_InputField tmpVar)
        {
            email = tmpVar.text;
            if (disableUserCreating == false) { PlayerPrefs.SetString(machineID + "User" + "Email", email); }
        }

        public void ChangePasswordTMP(TMP_InputField tmpVar)
        {
            password = tmpVar.text;
            if (disableUserCreating == false) { PlayerPrefs.SetString(machineID + "User" + "Password", password); }
        }

        public void UpdateUserInfoUI()
        {
            for (int i = 0; i < guiList.Count; ++i)
                guiList[i].GetInformation();
        }

        public void GetAllUserInfoComps()
        {
            guiList.Clear();
            GetUserInfo[] list = FindObjectsOfType(typeof(GetUserInfo)) as GetUserInfo[];
            foreach (GetUserInfo obj in list) { guiList.Add(obj); }
        }

        public void CreateUser()
        {
            userCreated = 1;
            PlayerPrefs.SetInt(machineID + "User" + "Created", userCreated);
            password = PlayerPrefs.GetString(machineID + "User" + "Password");
         
            UpdateUserInfoUI();
        }

        public void BootSystem()
        {
            bootManager.enabled = true;
            setupScreen.gameObject.SetActive(false);

            Animator lockScreenAnimator = lockScreen.GetComponent<Animator>();
            
            if (lockScreenAnimator != null)
            {
                lockScreenAnimator.Play("Lock Screen In");
            }
        }
        
        public void BootFromSetup()
        {
            bootManager.enabled = true;

            Animator lockScreenAnimator = lockScreen.GetComponent<Animator>();

            if (lockScreenAnimator != null)
            {
                lockScreenAnimator.Play("Lock Screen Fast In");
                
                StartCoroutine(DisableUserSetupHelper());
                StartCoroutine(CanPlayLockScreenInHelper());
            }
        }

        public void LockOS()
        {
            lockScreen.gameObject.SetActive(true);
            lockScreen.Play("Lock Screen In");
            desktopScreen.Play("Desktop Out");
        }
        
        public void LockScreenOpenClose()
        {
            if (isLockScreenOpen == true)
            {
                lockScreen.Play("Lock Screen Out");
                desktopScreen.Play("Desktop In");
            }
            else if (!canPlayLockScreenIn)
            {
                lockScreen.Play("Lock Screen In"); isLockScreenOpen = true; canPlayLockScreenIn = true;
            }
        }

        public void GoToDesktop()
        {
            desktopScreen.Play("Desktop In");
            lockScreen.Play("Lock Screen Out");
        }

        IEnumerator DisableLockScreenHelper()
        {
            yield return new WaitForSeconds(1f);
            lockScreen.gameObject.SetActive(false);
        }
        
        IEnumerator DisableUserSetupHelper()
        {
            yield return new WaitForSeconds(1f);
            setupScreen.gameObject.SetActive(false);
        }
        
        IEnumerator CanPlayLockScreenInHelper()
        {
            yield return new WaitForSeconds(10f);
            canPlayLockScreenIn = true;
        }
    }
}