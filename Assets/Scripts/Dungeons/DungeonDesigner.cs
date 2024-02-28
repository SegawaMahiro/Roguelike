using Cysharp.Threading.Tasks;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace Roguelike.Dungeons
{
    public class DungeonDesigner : MonoBehaviour
    {
        [SerializeField] int _xSize = 6;
        [SerializeField] int _ySize = 6;
        [SerializeField] DungeonRoomData _roomData;
        [SerializeField] int _roomSize = 30;
        [SerializeField] GameObject _startRoomSymbol;

        [SerializeField] NavMeshSurface _dungeonSurface;
        [SerializeField] Material _minimapDefaultMaterial;

        private RoomData[,] _grid;

        private void Awake() {
            OnStart();
        }
        private void OnStart() {
            _roomData = Instantiate(_roomData);
            _grid = new RoomData[_xSize, _ySize];
            for (int x = 0; x < _xSize; x++) {
                for (int y = 0; y < _ySize; y++) {
                    _grid[x, y] = new DungeonRoom1x1();
                }
            }
            Vector2Int startRoomPosition = SetStartRoom();
            Instantiate(_startRoomSymbol, new Vector3(startRoomPosition.x, 0, startRoomPosition.y) * 30, Quaternion.identity); ;

            GenerateGrid();
            DungeonConnector connector = new DungeonConnector(_grid, _roomSize);
            // 現在の配置を接続後の配置へ変更
            _grid = connector.StartConnect(startRoomPosition);
            BuildNavMesh().Forget();
        }
        private async UniTask BuildNavMesh() {
            await UniTask.Delay(300);
            _dungeonSurface.BuildNavMesh();

            NavMeshTriangulation triangles = NavMesh.CalculateTriangulation();

            //三角形集合からMeshを生成
            Mesh mesh = new Mesh();
            mesh.vertices = triangles.vertices;
            mesh.triangles = triangles.indices;

            MeshFilter filter = _dungeonSurface.gameObject.GetComponent<MeshFilter>();
            Renderer renderer = _dungeonSurface.gameObject.GetComponent<Renderer>();
            filter.mesh = mesh;
            renderer.material = _minimapDefaultMaterial;
        }
        /// <summary>
        /// 周囲4辺のランダムな位置に開始地点を作成
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        private Vector2Int SetStartRoom() {
            // 4面のうちの一つを指定
            var side = Random.Range(0, 4);

            // 面の中からランダムな位置をスタート位置に変更
            int x, y;
            GetRandomWallSide(side, out x, out y);

            _grid[x, y] = _roomData.StartRoom;
            return new Vector2Int(x, y);
        }

        /// <summary>
        /// list内の4辺の中からいずれかの面のランダムな位置を取得
        /// </summary>
        /// <param name="side"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void GetRandomWallSide(int side, out int x, out int y) {
            (x, y) = side switch {
                0 => (0, Random.Range(0, _ySize)), // 上辺
                1 => (_xSize - 1, Random.Range(0, _ySize)), // 下辺
                2 => (Random.Range(0, _xSize), 0), // 左辺
                3 => (Random.Range(0, _xSize), _ySize - 1), // 右辺
                _ => throw new System.NotImplementedException(),
            };
        }
        /// <summary>
        /// 部屋の位置情報を設定
        /// </summary>
        void GenerateGrid() {
            for (int x = 0; x < _xSize; x++) {
                for (int y = 0; y < _ySize; y++) {
                    _grid[x, y].ArrayPosition = new Vector2Int(x, y);
                }
            }
        }
    }
}