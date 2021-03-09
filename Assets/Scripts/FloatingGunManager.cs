using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace OculusSampleFramework
{
    public class FloatingGunManager: MonoBehaviour
    {
        [SerializeField] private GameObject _startStopButton = null;
        [SerializeField] private SelectionCylinder _selectionCylinder = null;
        [SerializeField] private Transform _shootingClubOrigin = null;
        private InteractableTool _toolInteractingWithMe = null;

        void Awake() {
            Assert.IsNotNull(_startStopButton);
            Assert.IsNotNull(_selectionCylinder);
        }
        private void OnEnable()
        {
            _startStopButton.GetComponent<Interactable>().InteractableStateChanged.AddListener(StartStopStateChanged);
        }

        private void OnDisable()
        {
            if (_startStopButton != null)
            {
                _startStopButton.GetComponent<Interactable>().InteractableStateChanged.RemoveListener(StartStopStateChanged);
            }
        }

        private void StartStopStateChanged(InteractableStateArgs obj)
        {

            _toolInteractingWithMe = obj.NewInteractableState > InteractableState.Default ?
            obj.Tool : null;
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if (_toolInteractingWithMe == null)
            {
                _selectionCylinder.CurrSelectionState = SelectionCylinder.SelectionState.Off;
            }
            else
            {
                _selectionCylinder.CurrSelectionState = (
                _toolInteractingWithMe.ToolInputState == ToolInputState.PrimaryInputDown ||
                _toolInteractingWithMe.ToolInputState == ToolInputState.PrimaryInputDownStay)
                ? SelectionCylinder.SelectionState.Highlighted
                : SelectionCylinder.SelectionState.Selected;

                if(_toolInteractingWithMe.ToolInputState == ToolInputState.PrimaryInputUp) {
                    GameManager.instance.forestIslandRoot.localEulerAngles = new Vector3(0f, 90f, 0f);
                    GameManager.instance.forestIslandRoot.localPosition = new Vector3(0f, -4.9f, -52f);
                }
            }
        }
    }
}