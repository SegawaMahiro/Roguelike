using UnityEngine;

namespace Roguelike.Managers
{
    internal class GameManager : MonoBehaviour
    {
        [SerializeField,Range(0,200)] int _framerate = 60;
        private void Start() {
            Application.targetFrameRate = _framerate;
            QualitySettings.vSyncCount = 0;
        }
    }
}
