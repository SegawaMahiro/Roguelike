using Cysharp.Threading.Tasks;
using Roguelike.Elements;
using Roguelike.Weapons.Combo;
using System.Threading;
using UnityEngine;

namespace Roguelike.Weapons
{
    public abstract class MeleeWeapon : ScriptableObject
    {
        [SerializeField] protected GameObject _weaponModel;
        [SerializeField] protected WeaponCombo _combo;
        [SerializeField] protected int _strength;
        [SerializeField] protected BaseElement _weaponElement;

        protected GameObject _weaponOwner;
        protected Transform _weaponCenter;
        protected CancellationTokenSource _source;

        protected const string WeaponHitboxCenter = "HitboxCenter";

        private GameObject _holdingWeapon;

        public WeaponCombo Combo { get { return _combo; } }

        /// <summary>
        /// scriptableobjectのcloneを作成
        /// </summary>
        public MeleeWeapon CreateWeapon() {
            return Instantiate(this);
        }
        /// <summary>
        /// 武器本体が存在しない場合オブジェクトを作成
        /// </summary>
        /// <param name="hand">武器の親となるtransform</param>
        /// <returns></returns>
        public GameObject HoldWeapon(Transform hand) {
            // 現在の武器を装備させる
            var weapon = Instantiate(_weaponModel, hand.transform);
            _holdingWeapon = weapon;
            return weapon;
        }
        public void CreateHitbox(GameObject owner, int comboCount) {
            if (_weaponOwner == null) {
                _weaponOwner = owner;
                _weaponCenter = _holdingWeapon.transform.Find(WeaponHitboxCenter);
            }
            _source?.Cancel();
            _source = new CancellationTokenSource();
            if (_source.IsCancellationRequested) {
                _source = new();
                _source.AddTo(_weaponOwner.GetCancellationTokenOnDestroy());
            }
            OnAttack(comboCount);
        }
        public void CancelAttack() {
            _source?.Cancel();  
        }
        public bool IsMovableAnimation(int comboCount) => _combo.GetMoveAnimationToggle(comboCount);
        public MoveSettings GetMove(int comboCount) => _combo.GetMoveSettings(comboCount);
        public AnimationClip GetAttackAnimation(int comboCount) => _combo.GetComboAnimation(comboCount);
        public (float delay, float dura) GetAttackTime(int comboCount) => _combo.GetHitboxLifetime(comboCount);
        protected abstract void OnAttack(int comboCount);
    }
}
