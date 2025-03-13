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
        InitialOrientation = animator.GetComponent<Boss>().isleft;
        Debug.Log("是否攻击过： "+boss.Attacked);
        if(boss.Attacked)
        {
            animator.SetBool("walk", false);
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bool attack = boss.CanAttack;
        bool isleft = boss.isleft;

        if (isleft != InitialOrientation || boss.Attacked)
        {
            boss.AttackNum++;
            animator.SetBool("普通攻击", false);
            animator.SetBool("walk", false);
        }
        else if (!boss.Attacked)
        {
            if (attack)
            {
                animator.SetBool("普通攻击", true);
            }
            else
            {
                move(isleft);
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
    void move(bool isleft)
    {
        if (!isleft)
        {
            rb.transform.rotation = Quaternion.Euler(0, 180, 0); // 面朝右
            rb.MovePosition(rb.position + new Vector2(boss.MoveSpeed * Time.deltaTime, 0));
            if (rb.position.x >= boss.RightPointX)
            {
                isleft = true;
            }
        }
        else
        {
            rb.transform.rotation = Quaternion.Euler(0, 0, 0); // 面朝左
            rb.MovePosition(rb.position + new Vector2(-boss.MoveSpeed * Time.deltaTime, 0));
            if (rb.position.x <= boss.LeftPointX)
            {
                isleft = false;
            }
        }
    }
}
