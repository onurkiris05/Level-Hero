using UnityEngine;
using VP.Nest.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float acceleration;

    public float MaxSpeed;

    private void FixedUpdate()
    {
        if (LevelManager.IsLevelPlaying)
        {
            if (InputManager.Direction.y != 0f || InputManager.Direction.x != 0f)
            {
                rb.AddForce((Vector3.forward * InputManager.Direction.y +
                             Vector3.right * InputManager.Direction.x) *
                            acceleration, ForceMode.VelocityChange);
            }
            else
            {
                rb.velocity *= 0.95f;
                rb.angularVelocity *= 0.95f;
            }

            rb.velocity = Vector3.ClampMagnitude(rb.velocity, MaxSpeed);
        }
    }

    public void Stop()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        this.enabled = false;
    }
}