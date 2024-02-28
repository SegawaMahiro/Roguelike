
namespace BehaviorTree
{
    public interface IOutputtable
    {
        public enum OutputType {
            Single,
            Multi
        }
        public OutputType PortOutputType { get; }
    }
}
