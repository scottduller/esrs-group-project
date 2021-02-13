using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            float horizontal = Input.GetAxis("Horizontal_AD");
            float vertical = Input.GetAxis("Vertical_WS");
            Vector3 move = new Vector3(horizontal, 0, vertical);
        
            _rigidbody.velocity = move * speed;
        }
    }
}
