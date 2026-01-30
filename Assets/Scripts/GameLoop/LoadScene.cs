using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameLoop
{
    public class LoadScene : MonoBehaviour
    {
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.1f);
            SceneManager.LoadScene(GameManager.NextSceneIndex);
        }
    }
}
