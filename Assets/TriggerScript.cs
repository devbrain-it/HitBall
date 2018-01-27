using System;
using UnityEngine;

namespace Assets
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class TriggerScript : MonoBehaviour
    {
        public event Action<TriggerScript, Collider2D> TriggeredEvent;
        public event Action<TriggerScript, Collider2D> TriggeredStayEvent;
        public event Action<TriggerScript, Collider2D> TriggeredExitEvent;

        public TriggerType UsageType;

        private new BoxCollider2D collider;
        private new Rigidbody2D   rb;

        // Use this for initialization
        void Start()
        {
            collider = GetComponent<BoxCollider2D>();
            switch (UsageType)
            {
                case TriggerType.MOVED:
                    collider.isTrigger  = true;
                    gameObject.isStatic = true;
                    rb                  = null;
                    break;
                case TriggerType.FIXED:
                    collider.isTrigger  = false;
                    gameObject.isStatic = true;
                    rb                  = GetComponent<Rigidbody2D>();
                    rb.bodyType         = RigidbodyType2D.Kinematic;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            TriggeredEvent?.Invoke(this, col);
        }

        void OnTriggerStay2D(Collider2D col)
        {
            TriggeredStayEvent?.Invoke(this, col);
        }

        void OnTriggerExit2D(Collider2D col)
        {
            TriggeredExitEvent?.Invoke(this, col);
        }
    }

    public enum TriggerType
    {
        MOVED,
        FIXED
    }
}