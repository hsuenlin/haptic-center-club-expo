using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace OculusSampleFramework
{    public abstract class HandsInteractable : MonoBehaviour
    {
        public GameObject controlButton;
        public virtual void OnNoInput() {}
        public virtual void OnPrimaryInputDown() {}
        public virtual void OnPrimaryInputDownStay() {}
        public virtual void OnPrimaryInputUp() {}

        private InteractableTool toolInteractingWithMe;

        void Awake()
        {
            Assert.IsNotNull(controlButton);
        }
        private void OnEnable()
        {
            controlButton.GetComponent<Interactable>().InteractableStateChanged.AddListener(OnButtonStateChanged);
        }

        private void OnDisable()
        {
            if (controlButton != null)
            {
                controlButton.GetComponent<Interactable>().InteractableStateChanged.RemoveListener(OnButtonStateChanged);
            }
        }

        private void OnButtonStateChanged(InteractableStateArgs obj)
        {

            toolInteractingWithMe = obj.NewInteractableState > InteractableState.Default ?
            obj.Tool : null;
        }

        // Update is called once per frame
        void Update()
        {
            if (toolInteractingWithMe == null)
            {
                OnNoInput();
            }
            else
            {
                if (toolInteractingWithMe.ToolInputState == ToolInputState.PrimaryInputDown)
                {
                    OnPrimaryInputDown();
                }
                else if (toolInteractingWithMe.ToolInputState == ToolInputState.PrimaryInputDownStay)
                {
                    OnPrimaryInputDownStay();
                }
                else if (toolInteractingWithMe.ToolInputState == ToolInputState.PrimaryInputUp)
                {
                    OnPrimaryInputUp();
                }
            }
        }
    }
}
