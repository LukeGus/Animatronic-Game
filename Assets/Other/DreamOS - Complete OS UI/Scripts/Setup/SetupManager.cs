using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Michsky.DreamOS
{
    public class SetupManager : MonoBehaviour
    {
        // List
        public List<StepItem> steps = new List<StepItem>();

        // Resources
        public UserManager userManager;
        [SerializeField] private TMP_InputField emailInput;
        [SerializeField] private TMP_InputField passwordInput;
        [SerializeField] private TMP_InputField passwordRetypeInput;
        [SerializeField] private Button infoContinueButton;
        [SerializeField] private Animator errorMessageObject;
        [SerializeField] private TextMeshProUGUI errorMessageText;

        // Settings
        [SerializeField] private int currentPanelIndex = 0;
        [SerializeField] private bool enableBackgroundAnim = true;
        [SerializeField][TextArea] private string emailLengthError;
        [SerializeField][TextArea] private string passwordLengthError;
        [SerializeField][TextArea] private string passwordRetypeError;

        private GameObject currentStep;
        private GameObject currentPanel;
        private GameObject nextPanel;
        private GameObject currentBG;
        private GameObject nextBG;

        [HideInInspector] public Animator currentStepAnimator;
        [HideInInspector] public Animator currentPanelAnimator;
        [HideInInspector] public Animator currentBGAnimator;
        [HideInInspector] public Animator nextPanelAnimator;
        [HideInInspector] public Animator nextBGAnimator;

        string panelFadeIn = "Panel In";
        string panelFadeOut = "Panel Out";
        string BGFadeIn = "Panel In";
        string BGFadeOut = "Panel Out";
        string stepFadeIn = "Check";

        [System.Serializable]
        public class StepItem
        {
            public string title = "Step";
            public GameObject indicator;
            public GameObject panel;
            public GameObject background;
            public StepContent stepContent;
        }

        public enum StepContent { Default, Information, FinalTouches }

        void Awake()
        {
            currentPanel = steps[currentPanelIndex].panel;
            currentPanelAnimator = currentPanel.GetComponent<Animator>();

            if (currentPanelAnimator.transform.parent.gameObject.activeSelf == true)
            {
                currentPanelAnimator.Play(panelFadeIn);

                if (enableBackgroundAnim == true)
                {
                    currentBG = steps[currentPanelIndex].background;
                    currentBGAnimator = currentBG.GetComponent<Animator>();
                    currentBGAnimator.Play(BGFadeIn);
                }

                else
                {
                    currentBG = steps[currentPanelIndex].background;
                    currentBGAnimator = currentBG.GetComponent<Animator>();
                    currentBGAnimator.Play(BGFadeIn);
                }
            }
        }

        void Update()
        {
            if (userManager == null)
                return;

            LayoutRebuilder.ForceRebuildLayoutImmediate(errorMessageText.transform.parent.GetComponent<RectTransform>());

            if (steps[currentPanelIndex].stepContent == StepContent.Information)
            {
                if (emailInput.text.Length >= userManager.minEmailCharacter && emailInput.text.Length <= userManager.maxEmailCharacter)
                {
                    if (!userManager.emailOK)
                    {
                        userManager.emailOK = true;
                        
                        if (userManager.passwordOK)
                            infoContinueButton.interactable = true;
                    }
                } else
                {
                    userManager.emailOK = false;
                    infoContinueButton.interactable = false;

                    if (errorMessageText.text != emailLengthError && errorMessageText.text != passwordLengthError && errorMessageText.text != passwordRetypeError)
                    {
                        errorMessageText.text = emailLengthError;
                    }

                    if (!errorMessageObject.GetCurrentAnimatorStateInfo(0).IsName("In"))
                        errorMessageObject.Play("In");
                }
            }

            if (steps[currentPanelIndex].stepContent == StepContent.Information)
            {
                if (passwordInput.text.Length >= userManager.minPasswordCharacter && passwordInput.text.Length <= userManager.maxPasswordCharacter)
                {
                    userManager.passwordOK = true;

                    if (passwordInput.text != passwordRetypeInput.text)
                    {
                        userManager.passwordRetypeOK = false;
                        infoContinueButton.interactable = false;

                        if (errorMessageText.text != passwordRetypeError)
                        {
                            errorMessageText.text = passwordRetypeError;
                        }

                        if (!errorMessageObject.GetCurrentAnimatorStateInfo(0).IsName("In"))
                            errorMessageObject.Play("In");
                    }

                    else if (passwordInput.text == passwordRetypeInput.text && userManager.emailOK)
                    {
                        userManager.passwordRetypeOK = true;

                        if (userManager.emailOK)
                            infoContinueButton.interactable = true;

                        if (!errorMessageObject.GetCurrentAnimatorStateInfo(0).IsName("Out"))
                            errorMessageObject.Play("Out");
                    }
                }

                else
                {
                    userManager.passwordOK = false;
                    infoContinueButton.interactable = false;

                    if (errorMessageText.text != passwordRetypeError && userManager.emailOK)
                    {
                        errorMessageText.text = passwordLengthError;
                    }

                    if (!errorMessageObject.GetCurrentAnimatorStateInfo(0).IsName("In"))
                        errorMessageObject.Play("In");
                }
            }
        }

        public void PanelAnim(int newPanel)
        {
            if (newPanel != currentPanelIndex)
            {
                currentPanel = steps[currentPanelIndex].panel;
                currentPanelIndex = newPanel;
                nextPanel = steps[currentPanelIndex].panel;

                currentPanelAnimator = currentPanel.GetComponent<Animator>();
                nextPanelAnimator = nextPanel.GetComponent<Animator>();

                currentPanelAnimator.Play(panelFadeOut);
                nextPanelAnimator.Play(panelFadeIn);

                if (enableBackgroundAnim == true)
                {
                    currentBG = steps[currentPanelIndex].background;
                    currentPanelIndex = newPanel;
                    nextBG = steps[currentPanelIndex].background;

                    currentBGAnimator = currentBG.GetComponent<Animator>();
                    nextBGAnimator = nextBG.GetComponent<Animator>();

                    currentBGAnimator.Play(BGFadeOut);
                    nextBGAnimator.Play(BGFadeIn);
                }
            }
        }

        public void NextPage()
        {
            if (currentPanelIndex <= steps.Count - 2)
            {
                currentPanel = steps[currentPanelIndex].panel;
                currentPanelAnimator = currentPanel.GetComponent<Animator>();
                currentPanelAnimator.Play(panelFadeOut);

                currentStep = steps[currentPanelIndex].indicator;
                currentStepAnimator = currentStep.GetComponent<Animator>();
                currentStepAnimator.Play(stepFadeIn);

                if (enableBackgroundAnim == true)
                {
                    currentBG = steps[currentPanelIndex].background;
                    currentBGAnimator = currentBG.GetComponent<Animator>();
                    currentBGAnimator.Play(BGFadeOut);
                }

                currentPanelIndex += 1;
                nextPanel = steps[currentPanelIndex].panel;

                nextPanelAnimator = nextPanel.GetComponent<Animator>();
                nextPanelAnimator.Play(panelFadeIn);

                if (enableBackgroundAnim == true)
                {
                    nextBG = steps[currentPanelIndex].background;
                    nextBGAnimator = nextBG.GetComponent<Animator>();
                    nextBGAnimator.Play(BGFadeIn);
                }
            }
        }

        public void PrevPage()
        {
            if (currentPanelIndex >= 1)
            {
                currentPanel = steps[currentPanelIndex].panel;
                currentPanelAnimator = currentPanel.GetComponent<Animator>();
                currentPanelAnimator.Play(panelFadeOut);

                if (enableBackgroundAnim == true)
                {
                    currentBG = steps[currentPanelIndex].background;
                    currentBGAnimator = currentBG.GetComponent<Animator>();
                    currentBGAnimator.Play(BGFadeOut);
                }

                currentPanelIndex -= 1;
                nextPanel = steps[currentPanelIndex].panel;

                nextPanelAnimator = nextPanel.GetComponent<Animator>();
                nextPanelAnimator.Play(panelFadeIn);

                if (enableBackgroundAnim == true)
                {
                    nextBG = steps[currentPanelIndex].background;
                    nextBGAnimator = nextBG.GetComponent<Animator>();
                    nextBGAnimator.Play(BGFadeIn);
                }
            }
        }

        public void PlayLastStepAnim()
        {
            currentStep = steps[steps.Count].indicator;
            currentStepAnimator = currentStep.GetComponent<Animator>();
            currentStepAnimator.Play(stepFadeIn);
        }
    }
}