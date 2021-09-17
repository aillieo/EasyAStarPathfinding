using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AillieoUtils.Pathfinding
{
    public abstract class LPAStar<T> : ISolver<T> where T : IGraphNode
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
            //foreach (var node in context.GetGraphData().GetAllNodes())
            //{
            //    NodeWrapperEx<T> nodeWrapper = context.GetOrCreateNode(node) as NodeWrapperEx<T>;
            //    nodeWrapper.g = float.MaxValue;
            //    nodeWrapper.rhs = float.MaxValue;
            //    context.AddToOpen(node, nodeWrapper);
            //}

            this.state = PathfindingState.Initialized;
        }

        public void Init(T startingNode, T endingNode)
        {
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

                NodeWrapperEx<T> endingNodeWrapper = context.GetOrCreateNode(this.endingNode);
                endingNodeWrapper.g = float.PositiveInfinity;
                endingNodeWrapper.rhs = float.PositiveInfinity;

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
                Debug.LogError("case 2");
                state = PathfindingState.Failed;
                TraceBackForPath(null);
                return state;
            }

            NodeWrapperEx<T> endingNodeWrapper0 = context.GetOrCreateNode(endingNode);
            if (endingNodeWrapper0.GetConsistency() == NodeConsistency.Consistent && first.CompareTo(endingNodeWrapper0) >= 0)
            {
                // 终点局部一致
                // 此时有一条有效路径
                Debug.LogError("case 1");
                state = PathfindingState.Found;
                TraceBackForPath(endingNodeWrapper0);
                return state;
            }

            if (first.GetConsistency() == NodeConsistency.Overconsistent)
            {
                // 障碍被清除 或者cost变低
                first.g = first.rhs;
                var neighbors = context.GetGraphData().CollectNeighbor(first.node);
                foreach (T p in neighbors)
                {
                    float cost = this.HeuristicFunc(first.node, p);
                    NodeWrapperEx<T> pNodeWrapper = context.GetOrCreateNode(p);
                    if (pNodeWrapper.rhs > first.g + cost)
                    {
                        pNodeWrapper.previous = first;
                        pNodeWrapper.rhs = first.g + cost;
                    }

                    if (pNodeWrapper.GetConsistency() == NodeConsistency.Consistent)
                    {
                        context.openList.Remove(pNodeWrapper);
                    }
                    else
                    {
                        context.openList.Enqueue(pNodeWrapper);
                    }
                }
            }
            else
            {
                first.g = float.PositiveInfinity;
                var neighbors = context.GetGraphData().CollectNeighbor(first.node);
                foreach (T p in neighbors)
                {
                    float cost = this.HeuristicFunc(first.node, p);
                    NodeWrapperEx<T> pNodeWrapper = context.GetOrCreateNode(p);
                    SelectAndUpdatePrevious(pNodeWrapper);

                    if (pNodeWrapper.GetConsistency() == NodeConsistency.Consistent)
                    {
                        context.openList.Remove(pNodeWrapper);
                    }
                    else
                    {
                        context.openList.Enqueue(pNodeWrapper);
                    }
                }

                context.openList.Enqueue(first);
            }

            if (first.node.Equals(this.endingNode))
            {
                Debug.LogError("case 3");
                state = PathfindingState.Found;
                TraceBackForPath(first);
                return state;
            }

            return state;
        }

        protected float CalculateG(NodeWrapperEx<T> nodeWrapper)
        {
            if (nodeWrapper.previous == null)
            {
                return 0f;
            }

            return nodeWrapper.previous.g + HeuristicFunc(nodeWrapper.node, nodeWrapper.previous.node) * nodeWrapper.node.cost;
        }

        protected float CalculateH(NodeWrapperEx<T> nodeWrapper)
        {
            return HeuristicFunc(nodeWrapper.node, this.endingNode);
        }

        protected float CalculateRHS(NodeWrapperEx<T> nodeWrapper)
        {
            // (nodeWrapper as NodeWrapperEx<T>).rhs
            return 0f;
        }

        protected Vector2 CalculateKey(NodeWrapperEx<T> nodeWrapper)
        {
            float k2 = Mathf.Min(nodeWrapper.g, nodeWrapper.rhs);
            float k1 = k2 + nodeWrapper.h;
            return new Vector2(k1, k2);
        }

        protected abstract float HeuristicFunc(T nodeFrom, T nodeTo);

        private void SelectAndUpdatePrevious(NodeWrapperEx<T> nodeWrapper)
        {
            var neighbors = context.GetGraphData().CollectNeighbor(nodeWrapper.node).Select(n => context.GetOrCreateNode(n));
            NodeWrapperEx<T> bestPrev = neighbors.First();
            foreach (var nei in neighbors)
            {
                nei.g = CalculateG(nei);
                nei.h = CalculateH(nei);
                nei.rhs = CalculateRHS(nei);
                nei.key = CalculateKey(nei);

                if (nei.CompareTo(bestPrev) < 0)
                {
                    bestPrev = nei;
                }
            }

            nodeWrapper.rhs = bestPrev.rhs;
            nodeWrapper.previous = bestPrev.previous;
        }

        protected bool TraceBackForPath(INodeWrapper<T> endingNode)
        {
            result.Clear();
            NodeWrapperEx<T> node = endingNode as NodeWrapperEx<T>;
            if (node == null || node.g == float.PositiveInfinity)
            {
                return false;
            }

            while (node.previous != null)
            {
                result.Add(node.node);
                node = node.previous;
            }
            result.Add(node.node);
            return true;
        }

        public void GetResult(List<T> toFill)
        {
            toFill.Clear();
            toFill.AddRange(result);
        }

        public void NotifyNodeDataModified(T nodeData)
        {
            NodeWrapperEx<T> nodeWrapper = this.context.TryGetNode(nodeData);
            context.openList.Update(nodeWrapper);
        }
    }
}
