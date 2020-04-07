using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.Character
{
    public class OutsideCharBehaviour : CharBehaviour
    {

        [Header("Movement")]
        public float rotationSpeed = 2f;
        public float moveSpeed;

        public override void OnMovement(Vector2 movement)
        {
            base.OnMovement(movement);
            Rotate(movement);
        }

        private void Rotate(Vector2 direction)
        {
            Quaternion newRot = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, newRot, rotationSpeed * Time.deltaTime);
        }

        public override void OnMoveAssistPerformed()
        {
            base.OnMoveAssistPerformed();
            Move();
        }

        private void Move()
        {
            transform.Translate(-transform.right * moveSpeed);
        }
    }

}
