using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPhaseOne : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //if the malwart's health is less than 2/3rds of its max health 
        if (animator.GetComponent<Enemy>().currentHealth <
            (animator.GetComponent<Enemy>().enemyProperties.GetHealthAmount) / 3)
        {
            animator.SetTrigger("phaseTwo");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}


}
