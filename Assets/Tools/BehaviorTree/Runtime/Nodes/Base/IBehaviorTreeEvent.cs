using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviorTree
{
    public interface IBehaviorTreeEvent
    {
        public void OnBehaviourTreeTick();
    }
}
