using UnityEngine;
using UnityEngine.SceneManagement;

public class ObstacleFail : MonoBehaviour
{
    public GameObject failText; // Inspector bata FailMessage tanera yaha halne

    private void OnTriggerEnter(Collider other)
    {
        // Yedi Ambulance (Player) le yo zone lai chhuyo vane
        if (other.CompareTag("Player"))
        {
            failText.SetActive(true); // Red text dekhaune
            Time.timeScale = 0f; // Game stop garne
        }
    }

    void Update()
    {
        // R thichda restart hune
        if (failText.activeSelf && Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}