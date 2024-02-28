namespace BehaviorTree
{
    public interface IOperator
    {
        bool Compare(float left, float right);
    }
    [SerializeReferenceDropdownName("==")]
    internal class Equal : IOperator
    {
        public bool Compare(float left, float right) {
            return left == right;
        }
    }
    [SerializeReferenceDropdownName("!=")]
    internal class NotEqual : IOperator
    {
        public bool Compare(float left, float right) {
            return left != right;
        }
    }
    [SerializeReferenceDropdownName(">")]
    internal class Higherthan : IOperator
    {
        public bool Compare(float left, float right) {
            return left > right;
        }
    }
    [SerializeReferenceDropdownName("<")]
    internal class Lessthan : IOperator
    {
        public bool Compare(float left, float right) {
            return left < right;
        }
    }
    [SerializeReferenceDropdownName(">=")]
    internal class GreaterThanOrEqual : IOperator
    {
        public bool Compare(float left, float right) {
            return left >= right;
        }
    }
    [SerializeReferenceDropdownName("<=")]
    internal class LessThanOrEqual : IOperator
    {
        public bool Compare(float left, float right) {
            return left <= right;
        }
    }
}
