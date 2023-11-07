using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
     // Controls the character's movement
    private CharacterController characterController;

    // Defines the character's direction
    private Vector3 movementDirection;

    // Defines the character's forward racing speed
    public float racingSpeed;
    // Defines the character's maximum speed
    private float maximumSpeed = 30;

    // Defines the position of the track: 0 Left, 1 Middle, 2 Right
    private int positionOnTrack = 1;

    // Defines the distance between these positions
    public float distanceBetween = 5;

    // Defines the upward jump force
    public float upwardJumpForce;

    // Defines the gravity on the player
    public float gravity = -20;

    // Defines the box colliders for the capsule
    private BoxCollider boxColliderX;
    private BoxCollider boxColliderY;

    // Add a variable to store the jump sound
    public AudioClip jumpSound;
    // Add a variable to store the land sound
    public AudioClip landSound;
    // Add a variable to store the swerve sound
    public AudioClip swerveSound;
    private float volume = 1.0f;

    /// <summary>
    /// This method is called when the game starts before the first frame update
    /// </summary>
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    /// <summary>
    /// This method is called once per frame
    /// </summary>
    void Update()
    {
        // If the game is not started, do not move the player
        if (!PlayerManager.isGameStarted)
            return;

        // Increasing player's speed
        if (racingSpeed < maximumSpeed)
            racingSpeed += Time.deltaTime * 0.2f;
        // Sets player's speed
        movementDirection.z = racingSpeed;

        // Prevent mid-air jumps
        if(characterController.isGrounded)
        {
            // No gravity when grounded
            movementDirection.y = -1;
            // Movement up
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                // Play the jump sound
                AudioSource.PlayClipAtPoint(jumpSound, transform.position, volume);
                CharacterJump();
            }
            // Movement down
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                StartCoroutine(Roll());
            }
        }
        else
        {
            // Movement down
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                // Play the land sound
                AudioSource.PlayClipAtPoint(landSound, transform.position, volume);
                CharacterFall();
            }
            else
            {
            // Player is affected by gravity
            movementDirection.y += gravity * Time.deltaTime;
            }
        
        }

        // Movement to the right
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            // Play the swerve sound
            AudioSource.PlayClipAtPoint(swerveSound, transform.position, volume);
            positionOnTrack++;
            if (positionOnTrack == 3)
                positionOnTrack = 2;
        }
        // Movement to the left
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // Play the swerve sound
            AudioSource.PlayClipAtPoint(swerveSound, transform.position, volume);
            positionOnTrack--;
            if (positionOnTrack == -1)
                positionOnTrack = 0;
        }

        // Calculate future positions
        Vector3 futurePosition = transform.position.z * transform.forward + transform.position.y * transform.up;
        // Move to the right
        if (positionOnTrack == 0)
        {
            futurePosition += Vector3.left * distanceBetween;
        }
        // Move to the left
        if (positionOnTrack == 2)
        {
            futurePosition += Vector3.right * distanceBetween;
        }
        // Smoothen the transition in movement
        if (transform.position == futurePosition)
            return;

        Vector3 difference = futurePosition - transform.position;
        Vector3 direction = difference.normalized * 25 * Time.deltaTime;

        if (direction.sqrMagnitude < difference.sqrMagnitude)
            characterController.Move(direction);
        else
            characterController.Move(difference);

    }

    IEnumerator Roll()
    {
        Quaternion originalRotation = transform.rotation;
        Quaternion targetRotation = originalRotation * Quaternion.Euler(0, 0, -90);

        Debug.Log(positionOnTrack);


        transform.rotation = targetRotation;
        if (transform.rotation == targetRotation)
        {
            // Turn off Y-axis capsule collider
            GetComponent<CapsuleCollider>().direction = 1;
            GetComponent<CapsuleCollider>().enabled = false;

            // Turn on X-axis capsule collider
            GetComponent<CapsuleCollider>().direction = 0;
            GetComponent<CapsuleCollider>().enabled = true;
        }

        yield return new WaitForSeconds(0.5f);

        transform.rotation = originalRotation;
        if (transform.rotation == originalRotation)
        {
            // Turn off X-axis capsule collider
            GetComponent<CapsuleCollider>().direction = 0;
            GetComponent<CapsuleCollider>().enabled = false;

            // Turn on Y-axis capsule collider
            GetComponent<CapsuleCollider>().direction = 1;
            GetComponent<CapsuleCollider>().enabled = true;
        }
    
    }

    /// <summary>
    /// This method is responsible for moving the player. 
    /// Preferred to Update because it runs at a fixed rate/per delta time while Update runs per frame
    /// </summary>
    private void FixedUpdate()
    {
        // If the game is not started, do not move the player
        if (!PlayerManager.isGameStarted)
            return;
        
        // Move the player
        characterController.Move(movementDirection * Time.fixedDeltaTime);
    }

    /// <summary>
    /// This method makes the player jump
    /// </summary>
    private void CharacterJump()
    {
        movementDirection.y  = upwardJumpForce;
        characterController.Move(movementDirection * Time.fixedDeltaTime);
    }

    private void CharacterFall()
    {
        movementDirection.y  = -upwardJumpForce;
        characterController.Move(movementDirection * Time.fixedDeltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // If the player hits an obstacle
        if(hit.transform.tag == "Obstacle" || hit.transform.tag == "Moving Obstacle")
        {
            // Set the game over condition to true
            PlayerManager.gameOver = true;
        }
    }

}
