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




}
