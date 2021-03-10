using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace OculusSampleFramework
{
    public class AddTargetButtonManager : HandsInteractable
    {
        public Material defaultMaterial;
        public Material hoverMaterial;
        public Material downMaterial;
        public MeshRenderer renderer;
        void Awake()
        {
        }

        public override void OnNoInput()
        {
            renderer.material = defaultMaterial;
        }

        public override void OnPrimaryInputDown()
        {
            renderer.material = hoverMaterial;
        }

        public override void OnPrimaryInputDownStay()
        {
            renderer.material = downMaterial;
        }

        public override void OnPrimaryInputUp()
        {
            renderer.material = defaultMaterial;
            TargetManager.instance.UpdateHandbook();
            TargetManager.instance.AddTargetDemo();
        }
    }
}