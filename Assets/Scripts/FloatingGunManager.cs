using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace OculusSampleFramework
{
    public class FloatingGunManager: HandsInteractable
    {
        public SelectionCylinder selectionCylinder;
        void Awake() {
            Assert.IsNotNull(selectionCylinder);
        }

        public override void OnNoInput() {
            selectionCylinder.CurrSelectionState = SelectionCylinder.SelectionState.Off;
        }

        public override void OnPrimaryInputDown() {
            selectionCylinder.CurrSelectionState = SelectionCylinder.SelectionState.Highlighted;
        }

        public override void OnPrimaryInputDownStay() {
            selectionCylinder.CurrSelectionState = SelectionCylinder.SelectionState.Highlighted;
        }

        public override void OnPrimaryInputUp() {
            selectionCylinder.CurrSelectionState = SelectionCylinder.SelectionState.Selected;
            GameManager.instance.ChangeSceneTo(SceneState.SHOOTING_CLUB);
        }
    }
}