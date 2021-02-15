using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        private float horizontalAD;
        private float verticalWS;


        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            movePos();
            RotateToDirection();

        }


        private void movePos()
        {
            float horizontalAD = Input.GetAxis("Horizontal_AD");
            float verticalWS = Input.GetAxis("Vertical_WS");
            Vector3 move = new Vector3(horizontalAD, 0, verticalWS);
            _rigidbody.velocity = move * speed;
        }



        private void RotateToDirection()
        {
            Vector3 mousePos = UtilsClass.GetMouseWorldPosition();
            if (float.IsNegativeInfinity(mousePos.x)) return;
            mousePos.y = transform.position.y;
            transform.LookAt(mousePos);
            Debug.DrawRay(transform.position,transform.forward*100,Color.red);
            
        }
    }
}