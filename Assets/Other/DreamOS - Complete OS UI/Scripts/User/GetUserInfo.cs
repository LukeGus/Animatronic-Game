using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Michsky.DreamOS
{
    [AddComponentMenu("DreamOS/User/Get User Info")]
    public class GetUserInfo : MonoBehaviour
    {
        [Header("Resources")]
        public UserManager userManager;

        [Header("Settings")]
        public Reference getInformation;
        public bool updateOnEnable = true;

        TextMeshProUGUI textObject;
        Image imageObject;

        // Reference list
        public enum Reference
        {
            Email,
            Password
        }

        void OnEnable()
        {
            // Find User manager in the scene
            if (userManager == null) { userManager = (UserManager)GameObject.FindObjectsOfType(typeof(UserManager))[0]; }

            // If it's true, then get the requested information at start
            if (updateOnEnable == true) { GetInformation(); }
        }

        public void GetInformation()
        {
            if (userManager == null)
                return;
            
            else if (getInformation == Reference.Email)
            {
                textObject = gameObject.GetComponent<TextMeshProUGUI>();
                textObject.text = userManager.email;
            }
            
            else if (getInformation == Reference.Password)
            {
                textObject = gameObject.GetComponent<TextMeshProUGUI>();
                textObject.text = userManager.password;
            }
        }

        public void AddToGUIList() { if (userManager != null) { userManager.guiList.Add(this); } }
    }
}