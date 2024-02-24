using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    [Serializable]
    public struct ObjectPoolConfig
    {
        public GameObject prefab;
        public int maxPoolSize;
    }

    // 作成するpoolの情報
    [SerializeField] private List<ObjectPoolConfig> _objectPoolConfigs = new List<ObjectPoolConfig>();

    // managerに登録されているobjectpool
    private Dictionary<GameObject, ObjectPool<GameObject>> _pools = new Dictionary<GameObject, ObjectPool<GameObject>>();

    // gameobjectからそのオブジェクトが登録されているpoolを返却ための物
    private Dictionary<GameObject, GameObject> _objectPrefabMap = new Dictionary<GameObject, GameObject>();

    public override void OnAwake() {
        // configに基づいてpoolを作成
        foreach (var config in _objectPoolConfigs) {
            ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
                () => OnCreatePooledObject(config.prefab),
                OnGetFromPool,
                OnReleaseToPool,
                OnDestroyPooledObject,
                false,
                config.maxPoolSize,
                config.maxPoolSize
            );
            _pools.Add(config.prefab, pool);
        }
    }

    private GameObject OnCreatePooledObject(GameObject prefab) {
        return Instantiate(prefab);
    }

    private void OnGetFromPool(GameObject obj) {
        obj.SetActive(true);
    }

    private void OnReleaseToPool(GameObject obj) {
        obj.SetActive(false);
    }

    private void OnDestroyPooledObject(GameObject obj) {
        Destroy(obj);
    }



    /// <summary>
    /// 一致するobjectをpoolから生成する
    /// </summary>
    /// <param name="prefab">取得したいobject</param>
    /// <param name="position">生成位置</param>
    /// <param name="rotation">回転</param>
    /// <returns></returns>
    public GameObject Get(GameObject prefab, Vector3 position = default, Quaternion rotation = default) {
        // objectpoolに登録されていない場合生成不可
        if (!_pools.ContainsKey(prefab)) {
            Debug.LogError($"指定されたPrefabがObjectPoolに指定されていません");
            return null;
        }

        // poolからobjectを取得し、transformを設定して返却する
        ObjectPool<GameObject> pool = _pools[prefab];
        GameObject obj = pool.Get();
        _objectPrefabMap[obj] = prefab;
        Transform tf = obj.transform;
        tf.position = position;
        tf.rotation = rotation * Quaternion.Euler(prefab.transform.rotation.eulerAngles);

        return obj;
    }
    /// <summary>
    /// 一致するobjectをpoolから生成する
    /// </summary>
    /// <param name="prefab">取得したいobject</param>
    /// <param name="transform">生成情報</param>
    /// <returns></returns>
    public GameObject Get(GameObject prefab, Transform transform = default) {
        return Get(prefab, transform.position, transform.rotation);
    }

    /// <summary>
    /// gameobjectを解放する
    /// </summary>
    /// <param name="obj">解放する対象</param>
    /// <param name="delay">解放までの時間/s</param>
    public void Release(GameObject obj, float delay = 0) => ReleaseAsync(obj, delay).Forget();

    /// <summary>
    /// 一定時間後にgameobjectを解放
    /// </summary>
    private async UniTask ReleaseAsync(GameObject obj, float delay) {
        // 遅延が存在する場合await
        if (delay > 0) {
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: obj.GetCancellationTokenOnDestroy());
        }
        // objectが見つからない場合return
        if (obj is null) return;

        // 対象のobjectがpool対象か
        if (_objectPrefabMap.TryGetValue(obj, out GameObject prefab)) {
            // objectがpool可能な物だった場合解放
            if (_pools.TryGetValue(prefab, out ObjectPool<GameObject> pool)) {
                pool.Release(obj);
                _objectPrefabMap.Remove(obj);
                return;
            }
        }
        Debug.LogError($"指定されたPrefabがObjectPoolに指定されていません");
    }
}
