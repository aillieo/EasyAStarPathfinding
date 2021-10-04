using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AillieoUtils.Pathfinding
{
    public abstract class LPAStar<T> : IIncrementalSolver<T> where T : IGraphNode
    {
        public PathfindingState state { get; protected set; }

        private readonly List<T> result = new List<T>();

        internal readonly PathfindingContextEx<T> context;
        internal T startingNode;
        internal T endingNode;

        public LPAStar(IGraphData<T> graphData, Algorithms algorithm)
        {
            this.context = new PathfindingContextEx<T>(graphData, algorithm);
            this.state = PathfindingState.Uninitialized;
        }

        public virtual void CleanUp()
        {
            context.Reset();
            result.Clear();
            state = PathfindingState.Uninitialized;
        }

        public void Init()
        {
            this.state = PathfindingState.Initialized;
        }

        public void Init(T startingNode, T endingNode)
        {
            if (startingNode.Equals(this.startingNode) && endingNode.Equals(this.endingNode))
            {
                return;
            }

            this.context.Reset();

            this.startingNode = startingNode;
            this.endingNode = endingNode;
            this.Init();
        }

        public PathfindingState Step()
        {
            if (state == PathfindingState.Initialized)
            {
                state = PathfindingState.Finding;

                NodeWrapperEx<T> startingNodeWrapper = context.GetOrCreateNode(this.startingNode);
                startingNodeWrapper.g = float.PositiveInfinity;
                startingNodeWrapper.rhs = 0;
                startingNodeWrapper.h = CalculateH(startingNodeWrapper);
                startingNodeWrapper.key = CalculateKey(startingNodeWrapper);

                NodeWrapperEx<T> endingNodeWrapper = context.GetOrCreateNode(this.endingNode);
                endingNodeWrapper.g = float.PositiveInfinity;
                endingNodeWrapper.rhs = float.PositiveInfinity;
                endingNodeWrapper.h = 0;
                endingNodeWrapper.key = CalculateKey(endingNodeWrapper);

                context.openList.Enqueue(startingNodeWrapper);

                return state;
            }

            if (state != PathfindingState.Finding)
            {
                throw new Exception($"Unexpected state {state}");
            }

            NodeWrapperEx<T> first = context.openList.TryDequeue();
            if (first == null)
            {
                // 寻路失败
                state = PathfindingState.Failed;
                TraceBackForPath(null);
                return state;
            }

            NodeWrapperEx<T> endingNodeWrapper0 = context.GetOrCreateNode(endingNode);
            endingNodeWrapper0.key = CalculateKey(endingNodeWrapper0);
            if (endingNodeWrapper0.GetConsistency() == NodeConsistency.Consistent
                && first.CompareTo(endingNodeWrapper0) >= 0)
            {
                // 终点局部一致
                // 此时有一条有效路径
                state = PathfindingState.Found;
                TraceBackForPath(endingNodeWrapper0);
                return state;
            }

            if (first.GetConsistency() == NodeConsistency.Overconsistent)
            {
                // 障碍被清除 或者cost变低
                first.g = first.rhs;
            }
            else
            {
                first.g = float.PositiveInfinity;
                UpdateNode(first);
            }

            var neighbors = context.GetGraphData().CollectNeighbor(first.node).Select(n => context.GetOrCreateNode(n));
            foreach (NodeWrapperEx<T> nei in neighbors)
            {
                UpdateNode(nei);
            }

            return state;
        }

        protected float CalculateG(NodeWrapperEx<T> nodeWrapper)
        {
            throw new NotImplementedException();
        }

        protected float CalculateH(NodeWrapperEx<T> nodeWrapper)
        {
            return HeuristicFunc(nodeWrapper.node, this.endingNode);
        }

        protected float CalculateRHS(NodeWrapperEx<T> nodeWrapper)
        {
            var neighbors = context.GetGraphData().CollectNeighbor(nodeWrapper.node).Select(n => context.GetOrCreateNode(n));
            return neighbors.Select(nei => nei.g + HeuristicFunc(nei.node, nodeWrapper.node) * (1 + nodeWrapper.node.cost * 100f)).Min();
        }

        protected Vector2 CalculateKey(NodeWrapperEx<T> nodeWrapper)
        {
            float k2 = Mathf.Min(nodeWrapper.g, nodeWrapper.rhs);
            float k1 = k2 + nodeWrapper.h;
            return new Vector2(k1, k2);
        }

        protected abstract float HeuristicFunc(T nodeFrom, T nodeTo);

        private void UpdateNode(NodeWrapperEx<T> nodeWrapper)
        {
            if (nodeWrapper.node.Equals(startingNode))
            {
            }
            else
            {
                // 更新这些节点的 previous
                var neighbors = context.GetGraphData().CollectNeighbor(nodeWrapper.node).Select(n => context.GetOrCreateNode(n));
                nodeWrapper.rhs = CalculateRHS(nodeWrapper);
            }

            if (nodeWrapper.GetConsistency() == NodeConsistency.Consistent)
            {
                context.openList.Remove(nodeWrapper);
            }
            else
            {
                nodeWrapper.h = CalculateH(nodeWrapper);
                nodeWrapper.key = CalculateKey(nodeWrapper);

                if (context.openList.Contains(nodeWrapper))
                {
                    context.openList.Update(nodeWrapper);
                }
                else
                {
                    context.openList.Enqueue(nodeWrapper);
                }
            }
        }

        protected bool TraceBackForPath(INodeWrapper<T> endingNode)
        {
            result.Clear();
            NodeWrapperEx<T> nodeWrapper = endingNode as NodeWrapperEx<T>;
            if (nodeWrapper == null || nodeWrapper.g == float.PositiveInfinity)
            {
                return false;
            }

            while (!nodeWrapper.node.Equals(startingNode))
            {
                NodeWrapperEx<T> previous = default;
                foreach (var nei in context.GetGraphData().CollectNeighbor(nodeWrapper.node).Select(n => context.GetOrCreateNode(n)))
                {
                    if (previous == null || nei.g < previous.g)
                    {
                        previous = nei;
                    }
                }

                result.Add(nodeWrapper.node);
                nodeWrapper = previous;
            }
            result.Add(nodeWrapper.node);

            return true;
        }

        public void GetResult(List<T> toFill)
        {
            toFill.Clear();
            toFill.AddRange(result);
        }

        public void NotifyNodeDataModified(T nodeData)
        {
            if (state == PathfindingState.Found || state == PathfindingState.Failed)
            {
                state = PathfindingState.Finding;

                NodeWrapperEx<T> endingNodeWrapper = context.GetOrCreateNode(this.endingNode);
                endingNodeWrapper.g = float.PositiveInfinity;
                endingNodeWrapper.rhs = float.PositiveInfinity;
                endingNodeWrapper.h = 0;
                endingNodeWrapper.key = CalculateKey(endingNodeWrapper);
                if (context.openList.Contains(endingNodeWrapper))
                {
                    context.openList.Remove(endingNodeWrapper);
                }
            }

            NodeWrapperEx<T> nodeWrapper = this.context.GetOrCreateNode(nodeData);
            nodeWrapper.rhs = CalculateRHS(nodeWrapper);

            if (nodeWrapper.GetConsistency() == NodeConsistency.Overconsistent)
            {
                nodeWrapper.g = nodeWrapper.rhs;
            }
            else
            {
                nodeWrapper.g = float.PositiveInfinity;
                UpdateNode(nodeWrapper);
            }

            var neighbors = context.GetGraphData().CollectNeighbor(nodeData).Select(n => context.GetOrCreateNode(n));
            foreach (var nei in neighbors)
            {
                UpdateNode(nei);
            }
        }
    }
}
