using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace OculusSampleFramework
{
    public class ChangeSceneButton: HandsInteractable
    {
        public SelectionCylinder selectionCylinder;
        public SceneState signifiedScene;

        private bool isRequesting;
        
        void Awake() {
            Assert.IsNotNull(selectionCylinder);
            isRequesting = false;
        }

        private IEnumerator PollingIsClubReady() {
            while(true) {  
                if(DataManager.instance.isRequestResultReady) {
                    DataManager.instance.isRequestResultReady = false;
                    DataManager.instance.contactText.SetActive(false);
                    if(!DataManager.instance.isClubReady[(int)signifiedScene]) {
                        DataManager.instance.failedText.transform.parent = DataManager.instance.clubPromptTransforms[(int)signifiedScene];
                        DataManager.instance.failedText.transform.localPosition = Vector3.zero;
                        DataManager.instance.failedText.transform.localRotation = Quaternion.identity;
                        DataManager.instance.failedText.SetActive(true);
                        yield return new WaitForSeconds(1.0f);
                        DataManager.instance.failedText.SetActive(false);
                        isRequesting = false;
                        yield break;
                    }
                }
                yield return null;
            }
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
            
            int sceneIndex = (int)signifiedScene;
            if(!DataManager.instance.isClubPlayed[sceneIndex]) {
            
                // Pinched && DeviceReady
                if (DataManager.instance.isDeviceFree[sceneIndex])
                {
                    DataManager.instance.isClubReady[sceneIndex] = true;
                } 
                else 
                {
                    if(!isRequesting) {
                        isRequesting = true;
                        switch(signifiedScene) {
                            case SceneState.SHOOTING_CLUB:
                                ClientSend.RequestDevice(ServerDevice.Controller);
                                break;
                            case SceneState.TENNIS_CLUB:
                                ClientSend.RequestDevice(ServerDevice.Shifty);
                                break;
                            case SceneState.MUSICGAME_CLUB:
                                ClientSend.RequestDevice(ServerDevice.Panel);
                                break;
                            default:
                                Debug.Log($"Invalid signified scene {signifiedScene}");
                                break;
                        }
                        DataManager.instance.contactText.transform.parent = DataManager.instance.clubPromptTransforms[(int)signifiedScene];
                        DataManager.instance.contactText.transform.localPosition = Vector3.zero;
                        DataManager.instance.contactText.transform.localRotation = Quaternion.identity;
                        DataManager.instance.contactText.SetActive(true);
                        StartCoroutine(PollingIsClubReady());
                    }
                }
            }
        }
    }
}