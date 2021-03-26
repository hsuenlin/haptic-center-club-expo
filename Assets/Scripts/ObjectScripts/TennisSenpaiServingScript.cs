using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TennisSenpaiServingScript : StateMachineBehaviour
{
    public Vector3 startingPosition;
    public Quaternion startingRotation;
    public Vector3 pos;
    public Quaternion rot;
    public bool isFirst = true;

    public GameObject ballPrefab;

    public bool isSwing;

    private GameObject ball;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.applyRootMotion = false;
        
        if(isFirst) {
            startingPosition = animator.gameObject.transform.localPosition;
            startingRotation = animator.gameObject.transform.localRotation;
            Debug.Log(animator.gameObject.transform.parent);
            Debug.Log(startingPosition);
            Debug.Log(startingRotation);
        } else {
            animator.gameObject.transform.localPosition = startingPosition;
            animator.gameObject.transform.localRotation = startingRotation;
        }
        
        isFirst = false;
        isSwing = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        if(stateInfo.normalizedTime % 1 > 0.87f && !isSwing) {
            DataManager.instance.isSenpaiSwing = true;
            isSwing = true;
        } else {
            DataManager.instance.isSenpaiSwing = false;
        }
        animator.applyRootMotion = true;
    }
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(DataManager.instance.OnServeStateEnd != null) {
            DataManager.instance.OnServeStateEnd();
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
