using UnityEngine;

namespace Environment
{
    [DisallowMultipleComponent]
    public class Gate : AControllable
    {
        [SerializeField]
        private bool m_initialVisibility = true;

        private void Awake()
        {
            this.gameObject.SetActive(m_initialVisibility);
        }

        public override void Activate()
        {
            this.gameObject.SetActive(!m_initialVisibility);
        }

        public override void Deactivate()
        {
        }
    }
}
