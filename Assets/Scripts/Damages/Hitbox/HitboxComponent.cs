using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Roguelike.Damages.Hitbox
{
    /// <summary>
    /// 当たり判定の生成と管理を行う
    /// </summary>
    public static class HitboxUtility
    {
        private static readonly OverlapCalculator _calculator = new OverlapCalculator();

        /// <summary>
        /// 2点の間に当たり判定を生成
        /// </summary>
        public static async UniTask CreateOverlapAsync(Func<Vector3> getStart, Func<Vector3> getEnd, IHitboxShape shape, float delay, float duration, Action<OverlapResult> callback, CancellationToken token) {
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: token);
            _calculator.DoOverlapAsync(getStart(), getEnd(), shape, duration, callback, token).Forget();
        }

        /// <summary>
        /// 連続的な当たり判定を生成
        /// </summary>
        public static async UniTask CreateContinuousOverlapAsync(Transform transform, IHitboxShape shape, float delay, float duration, Action<OverlapResult> callback, CancellationToken token) {
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: token);
            _calculator.DoContinuousOverlapAsync(transform, shape, duration, callback, token).Forget();
        }



        public static void CreateContinuousOverlap(Transform transform, IHitboxShape shape,float delay,float duration, Action<OverlapResult> callback, CancellationToken token)
            => CreateContinuousOverlapAsync(transform,shape,delay,duration, callback, token).Forget();




        /// <summary>
        /// 2点の間に当たり判定を生成
        /// </summary>
        public static void CreateOverlap(Vector3 start, Vector3 end, IHitboxShape shape, float delay, float duration, Action<OverlapResult> callback, CancellationToken token)
            => CreateOverlapAsync(() => start, () => end, shape, delay, duration, callback, token).Forget();

        /// <summary>
        /// 2点の間に当たり判定を生成
        /// </summary>
        public static void CreateOverlap(Transform start, Transform end, IHitboxShape shape, float delay, float duration, Action<OverlapResult> callback, CancellationToken token)
            => CreateOverlapAsync(() => start.position, () => end.position, shape, delay, duration, callback, token).Forget();

        /// <summary>
        /// 2点の間に当たり判定を生成
        /// </summary>
        public static void CreateOverlap(Vector3 start, Transform end, IHitboxShape shape, float delay, float duration, Action<OverlapResult> callback, CancellationToken token)
            => CreateOverlapAsync(() => start, () => end.position, shape, delay, duration, callback, token).Forget();

        /// <summary>
        /// 2点の間に当たり判定を生成
        /// </summary>
        public static void CreateOverlap(Transform start, Vector3 end, IHitboxShape shape, float delay, float duration, Action<OverlapResult> callback, CancellationToken token)
            => CreateOverlapAsync(() => start.position, () => end, shape, delay, duration, callback, token).Forget();
    }
}
