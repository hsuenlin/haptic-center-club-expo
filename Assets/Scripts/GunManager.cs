using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework
{ public class GunManager : HandsInteractable
    {
        public GameObject propStand;
        public BoxCollider gunCollider;

        public override void OnNoInput() { }
        public override void OnPrimaryInputDown() { }
        public override void OnPrimaryInputDownStay() { }
        public override void OnPrimaryInputUp() { 
            propStand.gameObject.SetActive(false);
            ShootingClubManager.instance.ExitWaiting();
            ShootingClubManager.instance.InitGame();
            ShootingClubManager.instance.state = ClubState.GAME;
        }
    }
}
