using UnityEngine;

namespace LevelEditor
{
    [DisallowMultipleComponent]
    public class Placeholder : MonoBehaviour
    {
        public int PlaceholderId => m_placeholderId;
        public event System.Action<Placeholder> OnSelect
        {
            add
            {
                m_onSelect -= value;
                m_onSelect += value;
            }
            remove
            {
                m_onSelect -= value;
            }
        }
        public event System.Action<Placeholder> OnDeselect
        {
            add
            {
                m_onDeselect -= value;
                m_onDeselect += value;
            }
            remove
            {
                m_onDeselect -= value;
            }
        }

        [SerializeField]
        private int m_placeholderId = -1;

        private event System.Action<Placeholder> m_onSelect;
        private event System.Action<Placeholder> m_onDeselect;

        public void Select()
        {
            m_onSelect?.Invoke(this);
        }

        public void Deselect()
        {
            m_onDeselect?.Invoke(this);
        }
    }
}
