using UnityEngine;

public class TestingScript : MonoBehaviour
{
    int score;

    // function to run before the game starts (runs 1 time)
    void Awake()
    {
        score = 0;
        Debug.Log("Awake");
    }

    // function to run when the game starts (runs 1 time)
    void Start()
    {
        score = 0;
        Debug.Log("Start");
    }

    // update is called every frame
    void Update()
    {
        // Debug.Log("Update");
    }

    // frame-rate independent for physics calculations.

    void FixedUpdate()
    {
        // Debug.Log("Fixed Update");
    }
}
