using UnityEngine;

public class Boss_Idle : StateMachineBehaviour
{
    private Boss boss;
    private Rigidbody2D rb;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponent<Rigidbody2D>();
        boss = animator.GetComponent<Boss>();
        animator.SetBool("walk", false);
        animator.SetBool("普通攻击", false);
        animator.SetBool("跳跃攻击", false);
        animator.SetBool("降落", false);
        animator.SetBool("刀波", false);
        animator.SetBool("空中", false);
        animator.SetBool("空中攻击", false);
        boss.Attacked = false;
          if(boss.Attacked)
        {
            Debug.Log("没变");
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
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
