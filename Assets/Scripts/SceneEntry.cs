using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace OculusSampleFramework
{
    public class SceneEntry: HandsInteractable
    {
        public SelectionCylinder selectionCylinder;
        public SceneState signifiedScene;
        
        void Awake() {
            Assert.IsNotNull(selectionCylinder);
            Assert.IsNotNull(signifiedScene);
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
            // Pinched && DeviceReady
            if (ArenaManager.instance.isDeviceReadyDict[signifiedScene]
                && !GameManager.instance.isClubPlayedDict[signifiedScene])
            {
                ArenaManager.instance.SetIsClubReady(signifiedScene, true);
            }
        }
    }
}