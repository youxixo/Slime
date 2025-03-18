using UnityEngine;

public class Boss_walk : StateMachineBehaviour
{
    private Boss boss;
    private Rigidbody2D rb;
    public bool InitialOrientation;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss = animator.GetComponent<Boss>();
        rb = animator.GetComponent<Rigidbody2D>();
        InitialOrientation = boss.Isleft();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bool attack = boss.CanAttack;

        if (boss.Isleft() != InitialOrientation && !attack)
        {
            animator.SetBool("walk", false);
        }
        else if (boss.Isleft() == InitialOrientation && attack)
        {
            animator.SetBool("普通攻击", true);
        }
        else
        {
            move();
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
    void move()
    {
        if (!boss.Isleft())
        {
            rb.MovePosition(rb.position + new Vector2(boss.MoveSpeed * Time.deltaTime, 0));
            boss.Isleft();
        }
        else
        {
            rb.MovePosition(rb.position + new Vector2(-boss.MoveSpeed * Time.deltaTime, 0));
            boss.Isleft();
        }
    }
}
