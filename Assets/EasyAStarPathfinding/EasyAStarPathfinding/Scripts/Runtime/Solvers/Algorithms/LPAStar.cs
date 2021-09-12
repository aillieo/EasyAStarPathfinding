using System;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public abstract class LPAStar<T> : AStar<T> where T : IGraphNode
    {
        public LPAStar(IGraphData<T> graphData, Algorithms algorithm)
            : base(graphData, algorithm)
        { }

        public override void Init()
        {
            foreach (var node in context.GetGraphData().GetAllNodes())
            {
                NodeWrapperEx<T> nodeWrapper = context.GetOrCreateNode(node) as NodeWrapperEx<T>;
                nodeWrapper.g = float.MaxValue;
                nodeWrapper.rhs = float.MaxValue;
                context.AddToOpen(node, nodeWrapper);
            }

            base.Init();
        }

        public override PathfindingState Step()
        {
            if (state == PathfindingState.Initialized)
            {
                state = PathfindingState.Finding;
                NodeWrapperEx<T> startNode = context.GetOrCreateNode(this.startingNode) as NodeWrapperEx<T>;
                context.AddToOpen(this.startingNode, startNode);
                return state;
            }

            if (state != PathfindingState.Finding)
            {
                throw new Exception($"Unexpected state {state}");
            }

            NodeWrapperEx<T> first = context.TryGetFrontier() as NodeWrapperEx<T>;
            if (first != null)
            {
                //context.RemoveFromMapping(first.node);

                if (first.GetConsistency() == NodeConsistency.LocallyOverconsistent)
                {
                    // 障碍被清除 或者cost变低
                    first.g = first.rhs;
                }
                else
                {

                    first.g = float.MaxValue;
                }

                // 把周围点 加入open
                var neighbors = context.GetGraphData().CollectNeighbor(first.node);
                foreach (T p in neighbors)
                {
                    Collect(p, first);
                }

                first.state = NodeState.Closed;

                if (first.node.Equals(this.endingNode))
                {
                    this.endingNodeWrapper = first;
                    state = PathfindingState.Found;
                    TraceBackForPath();
                    return state;
                }
            }
            else
            {
                state = PathfindingState.Failed;
                TraceBackForPath();
                return state;
            }

            return state;
        }

        protected override bool Collect(T node, NodeWrapper<T> parentNode)
        {
            if (this.context.TryGetClosedNode(node) != null)
            {
                return false;
            }

            bool changed = false;
            NodeWrapper<T> nodeWrapper = context.GetOrCreateNode(node) as NodeWrapper<T>;
            nodeWrapper.previous = parentNode;
            nodeWrapper.g = CalculateG(nodeWrapper);
            nodeWrapper.h = CalculateH(nodeWrapper);

            NodeWrapper<T> oldNodeWrapper = context.TryGetOpenNode(node) as NodeWrapper<T>;
            if (oldNodeWrapper == null)
            {
                changed = true;
                this.context.AddToOpen(node, nodeWrapper);
            }
            else if (nodeWrapper.g < oldNodeWrapper.g)
            {
                changed = true;
                oldNodeWrapper.g = nodeWrapper.g;
                this.context.UpdateFrontier(oldNodeWrapper);
            }

            return changed;
        }

        protected override float CalculateG(NodeWrapper<T> nodeWrapper)
        {
            return base.CalculateG(nodeWrapper);
        }

        protected override float CalculateH(NodeWrapper<T> nodeWrapper)
        {
            return base.CalculateH(nodeWrapper);
        }

        protected virtual float CalculateRHS(NodeWrapper<T> nodeWrapper)
        {
            // (nodeWrapper as NodeWrapperEx<T>).rhs
            return 0f;
        }

        // 当发生变化后 重新计算之前的 NodeWrapper
        private void RecalculateNode(NodeWrapperEx<T> nodeWrapper)
        {

        }
    }
}
