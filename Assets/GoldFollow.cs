
using UnityEngine;
using UnityEngine.VFX;

public class GoldFollow : MonoBehaviour
{
    GameObject _player;
    VisualEffect _effect;
    private void Start() {
        _player = GameObject.FindWithTag("Player");
        _effect = GetComponent<VisualEffect>();
    }
    private void Update() {
        _effect.SetVector3("PlayerPosition", _player.transform.position);
    }
}
