using System;
using UnityEngine;

namespace Assets.scripts
{
    public interface ILinearBounce
    {
        event Action<Collision2D> OnCollisionEvent;
        event Action              OnUpdateEvent;
    }
}