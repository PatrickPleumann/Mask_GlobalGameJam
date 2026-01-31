using UnityEngine;

namespace UI
{
    [CreateAssetMenu(fileName = "LevelInfo", menuName = "UI/LevelInfo", order = 1)]
    public class LevelInfo : ScriptableObject
    {
        public string LevelName => m_levelName;
        public string SceneName => m_sceneName;
        public Sprite LevelImage => m_levelImage;

        [SerializeField]
        private string m_levelName;
        [SerializeField]
        private string m_sceneName;
        [SerializeField]
        private Sprite m_levelImage;
    }
}
