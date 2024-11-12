using System;
using UnityEngine;


public static class EventHandler
{
    public static event Action<SlimeType> SlimeTypeEnterEvent;
    public static void CallSlimeTypeEnterEvent(SlimeType tpye)
    {
        SlimeTypeEnterEvent?.Invoke(tpye);
    }

    public static event Action SlimeTypeLeaveEvent;
    public static void CallSlimeTypeLeaveEvent()
    {
        SlimeTypeLeaveEvent?.Invoke();
    }

    public static event Action AttackEvent;
    public static void CallAttackEvent()
    {
        AttackEvent?.Invoke();
    }

    public static event Action AttackCheckStartEvent;
    public static void CallAttackCheckStartEvent()
    {
        AttackCheckStartEvent?.Invoke();
    }

    public static event Action AttackCheckEndEvent;
    public static void CallAttackCheckEndEvent()
    {
        AttackCheckEndEvent?.Invoke();
    }

    public static event Action<TagHandle> BulletHitPlayerEvent;
    public static void CallBulletHitPlayerEvent(TagHandle tag)
    {
        BulletHitPlayerEvent?.Invoke(tag);
    }

    public static event Action<TagHandle> BulletHitEnemyEvent;
    public static void CallBulletHitEnemyEvent(TagHandle tag)
    {
        BulletHitEnemyEvent?.Invoke(tag);
    }

}
