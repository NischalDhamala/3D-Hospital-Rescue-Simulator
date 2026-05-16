using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RescueTimer : MonoBehaviour
{
    [Header("UI Settings")]
    public Text timerText;          // Text to display timer
    public Text resultText;         // Text to display Passed/Failed
    public GameObject startMessage; // Old "Press M to start" text

    [Header("Time Settings")]
    public float timeRemaining = 120f; // 2 minutes
    
    private bool isTimerRunning = false;
    private bool missionEnded = false;

    void Start()
    {
        UpdateTimerDisplay();
        resultText.gameObject.SetActive(false); // Hide result at start
    }

    void Update()
    {
        // Timer starts on 'M' press (with ambulance)
        if (Input.GetKeyDown(KeyCode.M) && !isTimerRunning && !missionEnded)
        {
            isTimerRunning = true;
            if (startMessage != null) startMessage.SetActive(false); 
        }

        if (isTimerRunning && !missionEnded)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerDisplay();
            }
            else
            {
                // If time is up - Failed
                FinishMission(false);
            }
        }

        // Restart logic (on R press)
        if (missionEnded && Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // Called by Hospital's trigger
    public void MissionComplete()
    {
        if (!missionEnded && isTimerRunning)
        {
            FinishMission(true);
        }
    }

    void FinishMission(bool isSuccess)
    {
        missionEnded = true;
        isTimerRunning = false;
        resultText.gameObject.SetActive(true);

        if (isSuccess)
        {
            resultText.text = "Mission Passed!\nPatient Survived";
            resultText.color = Color.green;
        }
        else
        {
            resultText.text = "No, time's up! Mission Failed — the patient died.\nPlease play again.";
            resultText.color = Color.red;
            timerText.text = "00:00";
        }
        
        Time.timeScale = 0f; // Freeze the game
    }
}