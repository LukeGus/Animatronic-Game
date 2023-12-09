#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.DreamOS
{
    [CustomEditor(typeof(SetupManager))]
    public class SetupManagerEditor : Editor
    {
        private SetupManager setupTarget;
        private GUISkin customSkin;
        private int currentTab;

        private void OnEnable()
        {
            setupTarget = (SetupManager)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = DreamOSEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = DreamOSEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            DreamOSEditorHandler.DrawComponentHeader(customSkin, "Setup Top Header");

            GUIContent[] toolbarTabs = new GUIContent[3];
            toolbarTabs[0] = new GUIContent("Content");
            toolbarTabs[1] = new GUIContent("Resources");
            toolbarTabs[2] = new GUIContent("Settings");

            currentTab = DreamOSEditorHandler.DrawTabs(currentTab, toolbarTabs, customSkin);

            if (GUILayout.Button(new GUIContent("Content", "Content"), customSkin.FindStyle("Tab Content")))
                currentTab = 0;
            if (GUILayout.Button(new GUIContent("Resources", "Resources"), customSkin.FindStyle("Tab Resources")))
                currentTab = 1;
            if (GUILayout.Button(new GUIContent("Settings", "Settings"), customSkin.FindStyle("Tab Settings")))
                currentTab = 2;

            GUILayout.EndHorizontal();

            var steps = serializedObject.FindProperty("steps");
            var enableBackgroundAnim = serializedObject.FindProperty("enableBackgroundAnim");
            var userManager = serializedObject.FindProperty("userManager");
            var emailInput = serializedObject.FindProperty("emailInput");
            var passwordInput = serializedObject.FindProperty("passwordInput");
            var passwordRetypeInput = serializedObject.FindProperty("passwordRetypeInput");
            var infoContinueButton = serializedObject.FindProperty("infoContinueButton");
            var errorMessageObject = serializedObject.FindProperty("errorMessageObject");
            var errorMessageText = serializedObject.FindProperty("errorMessageText");
            var emailLengthError = serializedObject.FindProperty("emailLengthError");
            var passwordLengthError = serializedObject.FindProperty("passwordLengthError");
            var passwordRetypeError = serializedObject.FindProperty("passwordRetypeError");

            switch (currentTab)
            {             
                case 0:
                    DreamOSEditorHandler.DrawHeader(customSkin, "Content Header", 6);
                    GUILayout.BeginHorizontal();
                    EditorGUI.indentLevel = 1;
                    EditorGUILayout.PropertyField(steps, new GUIContent("Step List"), true);
                    EditorGUI.indentLevel = 0;
                    GUILayout.EndHorizontal();
                    break;

                case 1:
                    DreamOSEditorHandler.DrawHeader(customSkin, "Core Header", 6);
                    DreamOSEditorHandler.DrawProperty(userManager, customSkin, "User Manager");
                    DreamOSEditorHandler.DrawProperty(emailInput, customSkin, "Email Input");
                    DreamOSEditorHandler.DrawProperty(passwordInput, customSkin, "Password Input");
                    DreamOSEditorHandler.DrawProperty(passwordRetypeInput, customSkin, "Password RT Input");
                    DreamOSEditorHandler.DrawProperty(infoContinueButton, customSkin, "Info Button");
                    DreamOSEditorHandler.DrawProperty(errorMessageObject, customSkin, "Error Message Object");
                    DreamOSEditorHandler.DrawProperty(errorMessageText, customSkin, "Error Message Text");  
                    break;

                case 2:
                    DreamOSEditorHandler.DrawHeader(customSkin, "Options Header", 6);
                    enableBackgroundAnim.boolValue = DreamOSEditorHandler.DrawToggle(enableBackgroundAnim.boolValue, customSkin, "Enable Background Anim");
                    DreamOSEditorHandler.DrawPropertyCW(emailLengthError, customSkin, "Email Length Error", -3);
                    DreamOSEditorHandler.DrawPropertyCW(passwordLengthError, customSkin, "Password Length Error", -3);
                    DreamOSEditorHandler.DrawPropertyCW(passwordRetypeError, customSkin, "Password Retype Error", -3);
                    break;            
            }

            serializedObject.ApplyModifiedProperties();
            this.Repaint();
        }
    }
}
#endif