using UnityEngine;

namespace GameLoop
{
    [DisallowMultipleComponent]
    public class GoalPoint : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D _collision)
        {
            if (_collision.CompareTag("Player"))
            {
                GameManager.Instance.PlayerReachedGoal();
            }
        }
    }
}