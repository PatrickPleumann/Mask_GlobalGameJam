using UnityEngine;

namespace LevelEditor
{
    [DisallowMultipleComponent]
    public class GoalLevelEditor : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D _collision)
        {
            if (_collision.CompareTag("Player"))
            {
                LevelEditorManager.Instance.StopPlaying();
            }
        }
    }
}
