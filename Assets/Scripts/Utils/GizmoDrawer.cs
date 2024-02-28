using System;
using System.Collections.Generic;
using UnityEngine;

public class GizmoDrawer : Singleton<GizmoDrawer>
{
    private struct GizmoData
    {
        public Action DrawMethod;
        public float Duration;
        public float StartTime;
    }

    private readonly List<GizmoData> _gizmoDataList = new List<GizmoData>();

    public void AddGizmoDrawer(Action drawMethod, float duration) {
        _gizmoDataList.Add(
            new GizmoData {
                DrawMethod = drawMethod,
                Duration = duration,
                StartTime = Time.time
            });
    }

    private void OnDrawGizmos() {
        for (int i = _gizmoDataList.Count - 1; i >= 0; i--) {
            var gizmoData = _gizmoDataList[i];
            if (gizmoData.StartTime + gizmoData.Duration < Time.time) {
                _gizmoDataList.RemoveAt(i);
                continue;
            }
            gizmoData.DrawMethod?.Invoke();
        }
    }
}
