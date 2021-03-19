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
    
    private int frameCnt = 0;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.applyRootMotion = false;
        if(isFirst) {
            startingPosition = animator.gameObject.transform.localPosition;
            startingRotation = animator.gameObject.transform.localRotation;
        } else {
            animator.gameObject.transform.localPosition = startingPosition;
            animator.gameObject.transform.localRotation = startingRotation;
        }
        isFirst = false;
        frameCnt = 0;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(frameCnt == 135) {
            DataManager.instance.isSenpaiSwing = true;
        } else {
            DataManager.instance.isSenpaiSwing = false;
        }
        frameCnt++;
        animator.applyRootMotion = true;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
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
