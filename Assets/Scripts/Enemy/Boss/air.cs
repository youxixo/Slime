using UnityEngine;

public class air : StateMachineBehaviour
{
    private GameObject aircoll;
    private Boss boss;
    private Rigidbody2D rb;
    private float AirAttackPointY;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss = animator.GetComponent<Boss>();
        rb = animator.GetComponent<Rigidbody2D>();
        aircoll = boss.AirColl;
        AirAttackPointY = boss.AirAttackPoint.transform.position.y;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(aircoll != null && !aircoll.activeSelf)
        {
            aircoll.SetActive(true);
        }
        float bossY = boss.transform.position.y;
        if(bossY <= AirAttackPointY)
        {
            Boss.downYspeed = rb.linearVelocity.y;
            rb.constraints = RigidbodyConstraints2D.FreezePositionY;
            animator.SetBool("空中", false);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
