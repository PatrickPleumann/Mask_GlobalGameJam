using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace GameLoop
{
    [DisallowMultipleComponent]
    public class ResetToMenu : MonoBehaviour
    {
        private void Update()
        {
            if (Keyboard.current.anyKey.wasPressedThisFrame)
            {
                SceneManager.LoadScene("Menu");
            }
        }
    }
}
