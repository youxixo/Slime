using UnityEngine;
using UnityEngine.InputSystem.DualShock;

public class Boss_Idle : StateMachineBehaviour
{
    private Boss boss;
    private Transform PlayerTr;
    private Animator ani;
    private bool attacking;
    private bool startFight = false;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attacking = false;

        animator.SetBool("walk", false);
        animator.SetBool("普通攻击", false);
        animator.SetBool("跳跃攻击", false);
        animator.SetBool("降落", false);
        animator.SetBool("刀波", false);
        animator.SetBool("空中", false);
        animator.SetBool("空中攻击", false);
        boss = animator.GetComponent<Boss>();
        ani = animator;
    }
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();

        if(PlayerTr.position.x > boss.LeftPointX && PlayerTr.position.x < boss.RightPointX)
        {
            Face();
            startFight = true;
            if(startFight)
            {
                boss.HPUI.SetActive(true);
            }
        }

        if(boss.isGround &&
            !attacking
            && startFight)
        {
            Attack();
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(Boss.AttackNum >= 3)
        {
            Boss.AttackNum = 0;
        }
        else
        {
            Boss.AttackNum++;
        }
        attacking = false;
    }

    void Attack()
    {
        if(boss.CanAttack && Boss.AttackNum < 3)
        {
            ani.SetBool("普通攻击", true);
        }
        else
        {
            if(Boss.AttackNum >= 3)
            {
                AttackChange();
            }
            if(boss.Isleft() && Boss.AttackNum < 3)
            {
                if(PlayerTr.position.x >= boss.Panding.transform.position.x)
                {
                    attacking = true;
                    ani.SetBool("walk", true);
                }
                if(PlayerTr.position.x < boss.Panding.transform.position.x)
                {
                    AttackChange();
                }
            }
            if(!boss.Isleft() && Boss.AttackNum < 3)
            {
                if(PlayerTr.position.x <= boss.Panding.transform.position.x)
                {
                    attacking = true;
                    ani.SetBool("walk", true);
                }
                if(PlayerTr.position.x > boss.Panding.transform.position.x)
                {
                    AttackChange();
                }
            }
        }
    }

    void AttackChange()
    {
        attacking = true;
        int num;
        if(Boss.AttackNum >= 3)
        {
            num = 4;
        }
        else 
        {
            num = Random.Range(1, 4);
        }
        
        switch (num)
        {
            case 1:
                ani.SetBool("walk", true);
                break;
            case 2:
                ani.SetBool("跳跃攻击", true);
                break;
            case 3:
                ani.SetBool("刀波", true);
                break;
            case 4:
                boss.Air = true;
                ani.SetBool("空中攻击", true);
                break;
        }
    }

    void Face()
    {
        if (boss.Isleft() == false)
        {
            if(PlayerTr.position.x >= boss.Panding.transform.position.x)
            {
                attacking = false; // 重新允许攻击判断
            }
            boss.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            if(PlayerTr.position.x < boss.Panding.transform.position.x)
            {
                attacking = false;
            }
            boss.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
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
