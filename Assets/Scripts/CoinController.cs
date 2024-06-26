using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    // Add a variable to store the coin sound
    public AudioClip coinSound;
    // Add a variable to store the volume of the sound
    private float volume = 1.0f;

    void Update()
    {
        // Rotate the coin along the x-axis
        transform.Rotate(100 * Time.deltaTime, 0, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Play the coin sound
        AudioSource.PlayClipAtPoint(coinSound, transform.position, volume);
        // If the collider is the player
        if(other.tag == "Player")
        {
            // Increase the number of coins collected
            PlayerManager.coinsCollected += 1;

            // Destroy the coin
            Destroy(gameObject);
        }
    }
}