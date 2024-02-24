using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace Roguelike.Damages.Hitbox
{
    public struct OverlapResult
    {
        public Collider Collider;
        public Vector3 EnterPosition;
        public Vector3 EnterDirection;
    }
    public class OverlapCalculator
    {
        private struct OverlapData
        {
            public IHitboxShape Shape { get; private set; }
            public Collider[] Result { get; private set; }

            public OverlapData(IHitboxShape shape, int maxOverlaps) {
                Shape = shape;
                Result = new Collider[maxOverlaps];
            }
        }

        private const int MaxOverlapCount = 4;
        private const int MaxPositions = 4;

        /// <summary>
        /// 現在の位置と1フレーム前の位置を一定時間overlapによって補間する
        /// </summary>
        /// <param name="start">対象のtransform</param>
        /// <param name="shape">overlapの形状</param>
        /// <param name="duration">生成時間</param>
        /// <param name="callback">返却されるcollider</param>
        /// <param name="token">cancellationtoken</param>
        /// <returns></returns>
        public async UniTask DoContinuousOverlapAsync(Transform start, IHitboxShape shape, float duration, Action<OverlapResult> callback, CancellationToken token) {

            float remainingTime = duration;
            int currentIndex = 0;
            var overlapData = new OverlapData(shape, MaxOverlapCount);
            var startPositions = new Vector3[MaxPositions];

            // 時間が経過するまで位置を補完し続ける
            while (remainingTime > 0 && start != null) {
                // 開始地点を現在の位置に変更
                startPositions[currentIndex] = start.position;

                SetCompletionPositions(currentIndex, overlapData, startPositions, callback);
                // 次の位置を最大保存数の範囲でループさせる
                currentIndex = (currentIndex + 1) % MaxPositions;

                await UniTask.Yield(cancellationToken: token);
                remainingTime -= Time.deltaTime;
            }
        }

        /// <summary>
        /// 始点と終点の間に一定時間overlapを作成する
        /// </summary>
        /// <param name="start">開始地点</param>
        /// <param name="end">終了地点</param>
        /// <param name="shape">overlapの形状</param>
        /// <param name="duration">生成時間</param>
        /// <param name="callback">返却されるcollider</param>
        /// <param name="token">cancellationtoken</param>
        /// <returns></returns>
        public async UniTask DoOverlapAsync(Vector3 start, Vector3 end, IHitboxShape shape, float duration, Action<OverlapResult> callback, CancellationToken token) {
            float remainingTime = duration;
            var overlapData = new OverlapData(shape, MaxOverlapCount);

            // 始点と終点が存在する場合時間経過までループ
            while (remainingTime > 0 && start != null && end != null) {
                // 接触があった場合そのcolliderを送信
                InvokeOverlapResult(overlapData, start, end, callback);

                await UniTask.Yield(cancellationToken: token);
                remainingTime -= Time.deltaTime;
            }
        }
        /// <summary>
        /// 保存されている位置から現在フレームで保管する位置を指定し生成する
        /// </summary>
        /// <param name="currentIndex">現在の開始地点</param>
        /// <param name="overlapData">最大接触判定数と形状</param>
        /// <param name="positions">現在保存されている位置</param>
        /// <param name="callback">返却されるcollider</param>
        private void SetCompletionPositions(int currentIndex, OverlapData overlapData, Vector3[] positions, Action<OverlapResult> callback) {

            // 更新に対応できるよう新しいものから順にループさせる
            for (int i = MaxPositions - 1; i > 0; i--) {
                // 現在のフレームと1フレーム前指定
                int startIndex = (currentIndex + i) % MaxPositions;
                // 後から追加されたものがその瞬間のフレームのためstartが1フレーム前となる
                int endIndex = (currentIndex + i + 1) % MaxPositions;

                var start = positions[startIndex];
                var end = positions[endIndex];

                // 移動していない場合生成しない
                if (start == end || start == Vector3.zero || end == Vector3.zero) continue;

                InvokeOverlapResult(overlapData, start, end, callback);
            }
        }
        /// <summary>
        /// overlapの接触を順番に実行
        /// </summary>
        private void InvokeOverlapResult(OverlapData overlapData, Vector3 start, Vector3 end, Action<OverlapResult> callback) {
            int overlapCount = overlapData.Shape.Overlap(start, end, overlapData.Result);

            for (int j = 0; j < overlapCount; j++) {
                if (overlapData.Result[j] is null) continue;

                var col = overlapData.Result[j];
                var collideObjectData = new OverlapResult {
                    Collider = col,
                    EnterPosition = col.ClosestPoint(start),
                    EnterDirection = (start - end).normalized
                };

                callback(collideObjectData);
            }
        }
    }
}
