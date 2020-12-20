using Base_Classes;
using UnityEngine;

namespace Statuses
{
    public class Freeze : Status
    {
        public Freeze(float time, int damage = 2) : base("freeze", time)
        {
            statusColor = Color.blue;
            speedMultiplier *= 0.5f;
            this.damage = damage;
            canStack = true;
        }
    }
}