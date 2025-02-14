using UnityEngine;

public class StateController : MonoBehaviour
{
    private PlayerState _currentPlayerState = PlayerState.Idle;

    void Start()
    {
        ChangeState(PlayerState.Idle);
    }

    public void ChangeState(PlayerState newPlayerState)
    {
        if (_currentPlayerState != newPlayerState)
        {
            _currentPlayerState = newPlayerState;
        }
    }

    public PlayerState GetCurrentState() => _currentPlayerState;
}
