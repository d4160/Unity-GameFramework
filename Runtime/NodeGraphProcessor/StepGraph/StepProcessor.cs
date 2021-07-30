using System.Collections.Generic;
using System.Linq;
using GraphProcessor;
using Debug = UnityEngine.Debug;

namespace d4160.NodeGraphProcessor
{
    public class StepProcessor : ConditionalProcessor
    {
        public BaseNode Current { get; protected set; }

        // static readonly float   maxExecutionTimeMS = 100; // 100 ms max execution time to avoid infinite loops

        /// <summary>
        /// Manage graph scheduling and processing
        /// </summary>
        /// <param name="graph">Graph to be processed</param>
        public StepProcessor(BaseGraph graph) : base(graph) {}

        public override void Step()
        {
            if (currentGraphExecution == null)
            {
                Stack<BaseNode> nodeToExecute = new Stack<BaseNode>();
                if(startNodeList.Count > 0)
                    startNodeList.ForEach(s => nodeToExecute.Push(s));

                currentGraphExecution = startNodeList.Count == 0 ? RunTheGraph() : RunTheGraph(nodeToExecute);
	            currentGraphExecution.MoveNext(); // Advance to the first node
            }

            if (Current is StepNodeBase)
            {
                MoveNext();
            }

            while (!(Current is StepNodeBase))
            {
                if (!MoveNext())
                {
                    break;
                }
            }
        }

        private bool MoveNext()
        {
            if (currentGraphExecution == null) return false;

            bool moveNext = currentGraphExecution.MoveNext();
            
            if (moveNext)
            {
                Current = currentGraphExecution.Current;
            }
            else
            {
                currentGraphExecution = null;
                Current = null;
            }

            return moveNext;
        }
    }
}
