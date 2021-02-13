using UnityEngine;

namespace Player
{
    public class PlayerAttack : MonoBehaviour
    {

        [SerializeField] private float horizontal;
        [SerializeField] private float vertical;

        private void FixedUpdate()
        {
            horizontal = Input.GetAxis("Horizontal_LeftRight");
            vertical = Input.GetAxis("Vertical_DownUp");
            RotateToDirection();
        

        }



        private void RotateToDirection()
        {
            if(horizontal != 0)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, horizontal > 0 ? Quaternion.Euler(0f,90f,0f) : Quaternion.Euler(0f,270f,0f), 0.1f);
            }

            if (vertical == 0) return;
            transform.rotation = Quaternion.Lerp(transform.rotation, vertical > 0 ? Quaternion.Euler(0f,0f,0f) : Quaternion.Euler(0f,180f,0f), 0.1f);
        }
    }
}

