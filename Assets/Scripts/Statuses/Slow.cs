using Base_Classes;
using UnityEngine;

namespace Statuses
{
    public class Slow : Status
    {
        public Slow(float time) : base("slow", time)
        {
            statusColor = Color.grey;
            speedMultiplier *= 0.3f;
            Reset();
        } //Slow
    }
} //Slow