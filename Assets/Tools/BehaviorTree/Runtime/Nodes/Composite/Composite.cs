using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace BehaviorTree
{
    public abstract class Composite : BehaviorTreeNode, IInputtable, IOutputtable
    {
        IOutputtable.OutputType IOutputtable.PortOutputType => IOutputtable.OutputType.Multi;
    }
}
