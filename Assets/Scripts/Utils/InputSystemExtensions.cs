using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace UnityEngine.InputSystem
{
    public static class InputSystemExtension
    {
        /// <summary>
        /// 入力の開始を通知
        /// </summary>
        /// <param name="action">監視する入力</param>
        /// <returns></returns>
        public static IObservable<InputAction.CallbackContext> ObserveStarted(this InputAction action) {
            return Observable.FromEvent<InputAction.CallbackContext>(
                e => action.started += e,
                e => action.started -= e
            );
        }
        /// <summary>
        /// 入力の終了を通知
        /// </summary>
        /// <param name="action">監視する入力</param>
        /// <returns></returns>
        public static IObservable<InputAction.CallbackContext> ObserveCanceled(this InputAction action) {
            return Observable.FromEvent<InputAction.CallbackContext>(
                e => action.canceled += e,
                e => action.canceled -= e
              );
        }
        /// <summary>
        /// 入力情報の更新を通知
        /// </summary>
        /// <param name="action">監視する入力</param>
        /// <returns></returns>
        public static IObservable<InputAction.CallbackContext> ObserveChanged(this InputAction action) {
            return Observable.FromEvent<InputAction.CallbackContext>(
                e => action.performed += e,
                e => action.performed -= e
            );
        }
        /// <summary>
        /// 入力が続行する間毎フレーム通知
        /// </summary>
        /// <param name="action">監視する入力</param>
        /// <returns></returns>
        public static IObservable<InputAction.CallbackContext> ObserveEveryPressing(this InputAction action) =>
        Observable.Create<InputAction.CallbackContext>(observer => {
            bool isPressing = false;

            void OnStarted(InputAction.CallbackContext context) {
                isPressing = true;

                Observable.EveryFixedUpdate()
                    .TakeUntil(Observable.EveryFixedUpdate().Where(_ => !isPressing))
                    .Subscribe(_ => observer.OnNext(context));
            }

            void OnCanceled(InputAction.CallbackContext context) => isPressing = false;

            action.started += OnStarted;
            action.canceled += OnCanceled;

            return Disposable.Create(() => {
                action.started -= OnStarted;
                action.canceled -= OnCanceled;
            });
        });
    }
}