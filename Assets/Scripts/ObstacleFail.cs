using UnityEngine;
using UnityEngine.SceneManagement;

public class ObstacleFail : MonoBehaviour
{
    public GameObject failText; // Drag FailMessage from Inspector here

    private void OnTriggerEnter(Collider other)
    {
        // If Ambulance (Player) touches this zone
        if (other.CompareTag("Player"))
        {
            failText.SetActive(true); // Show red text
            Time.timeScale = 0f; // Stop the game
        }
    }

    void Update()
    {
        // Restart on R press
        if (failText.activeSelf && Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}