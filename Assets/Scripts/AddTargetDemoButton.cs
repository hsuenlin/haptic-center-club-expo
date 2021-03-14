using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace OculusSampleFramework
{
    public class AddTargetDemoButton : HandsInteractable
    {
        public Material defaultMaterial;
        public Material hoverMaterial;
        public Material downMaterial;
        public Renderer meshRenderer;
        void Awake()
        {
        }

        public override void OnNoInput()
        {
            meshRenderer.material = defaultMaterial;
        }

        public override void OnPrimaryInputDown()
        {
            meshRenderer.material = hoverMaterial;
        }

        public override void OnPrimaryInputDownStay()
        {
            meshRenderer.material = downMaterial;
        }

        public override void OnPrimaryInputUp()
        {
            meshRenderer.material = defaultMaterial;
            TargetMachineScript.instance.UpdateHandbook();
            TargetMachineScript.instance.AddTargetDemo();
        }
    }
}