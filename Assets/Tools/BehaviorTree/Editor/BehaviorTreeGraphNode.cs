using BehaviorTree;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviorTreeEditor
{
    public class BehaviorTreeGraphNode : Node
    {
        private BehaviorTreeNode _node;
        private string _guid;

        private Label _description;
        private Toggle _breakpoint;

        private BehaviorTreeGraphView _graphView;

        public BehaviorTreeNode Node { get { return _node; } }
        public string Guid { get { return _guid; } }

        public BehaviorTreeGraphNode(BehaviorTreeNode node, BehaviorTreeGraphView view) : base("Assets/Tools/BehaviorTree/Resources/NodeUI.uxml") {
            _graphView = view;
            _node = node;
            _guid = node.Guid;
            title = node.GetType().Name;
            style.left = node.NodePosition.x;
            style.top = node.NodePosition.y;

            InitializeUIStatus(node);
            SetNodeParameters(node);
        }

        internal void NodeUpdate() {
            RemoveFromClassList("Running");
            RemoveFromClassList("Success");
            RemoveFromClassList("Failure");
            if (_node.State == BehaviorTreeNode.NodeState.Running && _node.IsRunning) {
                AddToClassList("Running");
            }
            if(_node.State == BehaviorTreeNode.NodeState.Success) {
                AddToClassList("Success");
                _node.State = BehaviorTreeNode.NodeState.Running;
            }
            if (_node.State == BehaviorTreeNode.NodeState.Failure) {
                AddToClassList("Failure");
                _node.State = BehaviorTreeNode.NodeState.Running;
            }
            title = _node.Name;
        }


        private void InitializeUIStatus(BehaviorTreeNode node) {
            _description = this.Q<Label>("description");
            _description.text = node.Description;

            _breakpoint = this.Q<Toggle>("breakpoint");
            _breakpoint.value = node.Breakpoint;
            _breakpoint.RegisterValueChangedCallback(e => OnBreakpointToggleValueChanged(e.newValue));
        }



        /// <summary>
        /// GraphViewに表示するNodeの設定を行う
        /// </summary>
        private void SetNodeParameters(BehaviorTreeNode node) {
            // RootNodeは移動不可
            if (node is Root) { capabilities = Capabilities.Deletable; }
            if (node is IInputtable) {  CreatePort(Port.Capacity.Single, Orientation.Vertical, Direction.Input); }

            if (node is IOutputtable) {
                var outputtableNode = (IOutputtable)node;

                // 出力先の数に応じて設定を変更
                if (outputtableNode.PortOutputType == IOutputtable.OutputType.Single) {
                    CreatePort(Port.Capacity.Single, Orientation.Vertical, Direction.Output);
                }
                else { CreatePort(Port.Capacity.Multi, Orientation.Vertical, Direction.Output); }
            }
            // NodeごとにWindowでの見た目を変更するためクラス名を設定
            AddToClassList(node.GetType().BaseType.Name);
        }



        /// <summary>
        /// NodeのPortを追加
        /// </summary>
        private void CreatePort(Port.Capacity capacity, Orientation orientation, Direction direction) {
            var port = Port.Create<Edge>(orientation, direction, capacity, typeof(Port));
            port.portName = "";
            // portの位置が左右によってしまうため、中央へ固定する
            port.style.flexDirection = direction == Direction.Input ? FlexDirection.Column : FlexDirection.ColumnReverse;

            if (direction == Direction.Input) {
                port.portColor = new Color(0.2f ,0.5f ,0.7f);
                inputContainer.Add(port);
            }
            else {
                port.portColor = new Color(0.2f, 0.5f, 0.7f);
                outputContainer.Add(port);
            }
        }



        private void OnBreakpointToggleValueChanged(bool value) => _node.Breakpoint = value;

        public override void OnSelected() => _graphView.OnSelectingNodeChanged(this);
    }
}
