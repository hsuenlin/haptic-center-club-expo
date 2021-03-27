using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OculusSampleFramework;
public class BallContainerScript : HandsInteractable
{
    public GameObject pickBallMachineObj;

    public override void OnNoInput()
    {
        if(pickBallMachineObj.transform.childCount == 9) {
            for (int i = 1; i <= 8; i++)
            {
                pickBallMachineObj.transform.GetChild(i).GetComponent<PickBallRegion>().OnNoInput();
            }
        }
    }

    public override void OnPrimaryInputDown()
    {
        for(int i = 1; i <= 8; i++) {
            pickBallMachineObj.transform.GetChild(i).GetComponent<PickBallRegion>().OnPrimaryInputDown();
        }
    }
}
