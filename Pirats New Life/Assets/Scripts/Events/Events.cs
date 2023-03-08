using System;
using UnityEngine;

public static class Events
{
    public struct Cursor
    {
        public static Action onDeselect;
    }

    public struct Unit
    {
        public static Action<GameObject> onUnitSpawn;
        public static Action<GameObject> onUnitDeath;
    }

    public struct Time
    {
        public static Action onMorning;
        public static Action onMidday;
        public static Action onEvening;
        public static Action onNight;
    }
}