using BehaviorTree;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviorTreeEditor
{
    public class BehaviorTreeGraphView : GraphView
    {
        private BehaviorTreeGraphWindow _window;
        private BehaviorTreeSaveLoadManager _saveLoadManager;

        private readonly Vector2 _rootNodePosition = new Vector2(500, 250);
        private readonly Rect _minimapRect = new Rect(x: 10, y: 30, width: 200, height: 140);
        public BehaviorTreeGraphView(BehaviorTreeGraphWindow window) : base() {

            graphViewChanged += OnGraphViewChanged;
            _window = window;

            Initialize();
            SelectNode();
        }

        /// <summary>
        /// Windowの初期化
        /// </summary>
        private void Initialize() {

            _saveLoadManager = new(_window, this);
            style.flexGrow = 1;
            style.flexShrink = 1;

            this.StretchToParentSize();
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            styleSheets.Add(Resources.Load<StyleSheet>("BackGround"));
            Insert(0, new GridBackground());

            GenerateMinimap();
            if (_window.Data is null || _window.Data.Root is null) {
                GenerateRootNode();
            }
            else {
                LoadGraph();
            }
        }
        public override List<Port> GetCompatiblePorts(Port startAnchor, NodeAdapter nodeAdapter) => ports.ToList();

        /// <summary>
        /// 選択中のNodeが変更された際に表示するNodeを更新する
        /// </summary>
        internal void OnSelectingNodeChanged(BehaviorTreeGraphNode node) => _window.Data.SetSelectingNode(node.Guid);

        /// <summary>
        /// Graph内に新しいNodeを作成する
        /// </summary>
        internal void CreateNodeView(BehaviorTreeNode node) {
            BehaviorTreeGraphNode nodeView = new BehaviorTreeGraphNode(node, this);
            AddElement(nodeView);
        }
        internal void NodeUpdate() {
            foreach (BehaviorTreeGraphNode targetNode in nodes) {
                targetNode.NodeUpdate();
            }
        }
        internal void SaveGraph() => _saveLoadManager.SaveGraph();
        internal void LoadGraph() => _saveLoadManager.LoadGraph();

        /// <summary>
        /// 初回生成時に親を作成
        /// </summary>
        private void GenerateRootNode() => CreateNodeView(_window.Data.CreateNode(typeof(Root), _rootNodePosition));

        /// <summary>
        /// 作成するNodeを選択するWindowの生成
        /// </summary>
        private void SelectNode() {
            var searchWindowProvider = ScriptableObject.CreateInstance<BehaviorTreeNodeSearch>();
            searchWindowProvider.Initialize(this, _window);

            nodeCreationRequest += context => {
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindowProvider);
            };
        }
        /// <summary>
        /// Minimapの作成
        /// </summary>
        private void GenerateMinimap() {
            var minimap = new MiniMap();
            minimap.anchored = true;
            minimap.SetPosition(_minimapRect);
            Add(minimap);
        }

        /// <summary>
        /// GraphviewのNodeが操作された時の処理
        /// </summary>
        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange) {
            // イベント発生時、削除されたnodeがbehaviortreeの物だった場合
            if (graphViewChange.elementsToRemove?.OfType<BehaviorTreeGraphNode>().Any() == true) {
                foreach (var node in graphViewChange.elementsToRemove.OfType<BehaviorTreeGraphNode>()) {
                    // 実行tree内から削除する
                    _window.Data.DeleteNode(node.Guid);
                }
            }
            return graphViewChange;
        }
    }
}