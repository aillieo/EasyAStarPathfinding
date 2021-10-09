using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AillieoUtils.Pathfinding
{
    public class DStarLite<T> : IIncrementalSolver<T> where T : IGraphNode
    {
        public PathfindingState state { get; protected set; }

        private readonly List<T> result = new List<T>();

        internal readonly PathfindingContextEx<T> context;
        internal T oldStartingNode;
        internal T startingNode;
        internal T endingNode;
        internal T currentNode;

        private float km;

        public DStarLite(IGraphData<T> graphData, Algorithms algorithm)
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
            this.context.Reset();

            this.startingNode = startingNode;
            this.oldStartingNode = startingNode;
            this.currentNode = startingNode;
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
                startingNodeWrapper.rhs = float.PositiveInfinity;
                startingNodeWrapper.h = 0;
                startingNodeWrapper.key = CalculateKey(startingNodeWrapper);

                NodeWrapperEx<T> endingNodeWrapper = context.GetOrCreateNode(this.endingNode);
                endingNodeWrapper.g = float.PositiveInfinity;
                endingNodeWrapper.rhs = 0;
                endingNodeWrapper.h = 0;
                endingNodeWrapper.key = CalculateKey(endingNodeWrapper);

                context.openList.Enqueue(endingNodeWrapper);

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

            NodeWrapperEx<T> startingNodeWrapper0 = context.GetOrCreateNode(startingNode);
            startingNodeWrapper0.key = CalculateKey(startingNodeWrapper0);
            if (startingNodeWrapper0.GetConsistency() == NodeConsistency.Consistent
                && first.CompareTo(startingNodeWrapper0) >= 0)
            {
                // 终点局部一致
                // 此时有一条有效路径
                // state = PathfindingState.Found;
                // TraceBackForPath(startingNodeWrapper0);
                if (MoveAgent())
                {
                    state = PathfindingState.Found;
                }

                return state;
            }

            Vector2 keyOld = first.key;
            Vector2 key = CalculateKey(first);
            if (keyOld.x < key.x && keyOld.y < key.y)
            {
                first.key = key;
                context.openList.Enqueue(first);
            }
            else
            {
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
            }

            return state;
        }

        protected float CalculateH(NodeWrapperEx<T> nodeWrapper)
        {
            return HeuristicFunc(nodeWrapper.node, this.startingNode);
        }

        protected float CalculateRHS(NodeWrapperEx<T> nodeWrapper)
        {
            var neighbors = context.GetGraphData().CollectNeighbor(nodeWrapper.node).Select(n => context.GetOrCreateNode(n));
            return neighbors.Select(nei => nei.g + HeuristicFunc(nei.node, nodeWrapper.node) * (1 + nodeWrapper.node.cost)).Min();
        }

        protected Vector2 CalculateKey(NodeWrapperEx<T> nodeWrapper)
        {
            float k2 = Mathf.Min(nodeWrapper.g, nodeWrapper.rhs);
            float k1 = k2 + nodeWrapper.h + km;
            return new Vector2(k1, k2);
        }

        protected virtual float HeuristicFunc(T nodeFrom, T nodeTo)
        {
            return context.GetGraphData().DefaultHeuristicFunc(nodeFrom, nodeTo);
        }

        private void UpdateNode(NodeWrapperEx<T> nodeWrapper)
        {
            if (nodeWrapper.node.Equals(endingNode))
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

        protected bool TraceBackForPath(INodeWrapper<T> startingNode)
        {
            result.Clear();
            NodeWrapperEx<T> nodeWrapper = startingNode as NodeWrapperEx<T>;
            if (nodeWrapper == null || nodeWrapper.g == float.PositiveInfinity)
            {
                return false;
            }

            while (!nodeWrapper.node.Equals(endingNode))
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

        public IEnumerable<T> GetResult()
        {
            foreach (var t in result)
            {
                yield return t;
            }
        }

        public void NotifyNodeDataModified(T nodeData)
        {
            if (state != PathfindingState.Finding)
            {
                throw new InvalidOperationException();
            }

            km = km + HeuristicFunc(oldStartingNode, currentNode);
            oldStartingNode = currentNode;

            NodeWrapperEx<T> nodeWrapper = this.context.GetOrCreateNode(nodeData);
            nodeWrapper.g = float.PositiveInfinity;
            nodeWrapper.rhs = float.PositiveInfinity;
            UpdateNode(nodeWrapper);

            var neighbors = context.GetGraphData().CollectNeighbor(nodeData).Select(n => context.GetOrCreateNode(n));
            foreach (var nei in neighbors)
            {
                UpdateNode(nei);
            }
        }

        private bool MoveAgent()
        {
            var neighbors = context.GetGraphData().CollectNeighbor(currentNode).Select(n => context.GetOrCreateNode(n));
            currentNode = neighbors.MinFor<NodeWrapperEx<T>>(nei => nei.g + HeuristicFunc(nei.node, currentNode) * (1 + currentNode.cost)).node;

            if (currentNode.Equals(endingNode))
            {
                // 到达
                return true;
            }

            return false;
        }
    }
}
