using UnityEngine;

namespace Environment
{
    public abstract class AControllable : MonoBehaviour
    {
        public abstract void Activate();
        public abstract void Deactivate();
    }
}
