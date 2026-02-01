using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor.UI
{
    [DisallowMultipleComponent]
    public class PlaceholderTile : MonoBehaviour
    {
        public Placeholder AssociatedPrefab => m_associatedPrefab;

        [SerializeField]
        private Image m_thumbnailImage = null;

        private Placeholder m_associatedPrefab = null;

        public void Initialize(PlaceholderThumbnail _placeholderThumbnail)
        {
            m_associatedPrefab = _placeholderThumbnail.PlaceholderPrefab;
            m_thumbnailImage.sprite = _placeholderThumbnail.Thumbnail;
        }
    }
}