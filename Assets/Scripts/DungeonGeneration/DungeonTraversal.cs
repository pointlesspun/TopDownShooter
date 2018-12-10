/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.DungeonGeneration
{
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// Algorithm which randomly traverses through a dungeon (list of dungeon nodes),
    /// creating a smaller dungeon with guaranteed path
    /// </summary>
    [Serializable]
    public class DungeonTraversal
    {
        private class PathNode
        {
            private List<PathNode> _children = new List<PathNode>();

            public PathNode Parent
            {
                get;
                set;
            }

            public DungeonNode DungeonElement
            {
                get;
                set;
            }

            public float PathLength
            {
                get;
                set;
            }

            public List<PathNode> Children
            {
                get { return _children; }
            }

            public PathNode AddChild(PathNode node)
            {
                node.Parent = this;
                _children.Add(node);
                return node;
            }
        }

        /// <summary>
        /// Max number of iterations, set to -1 if there no limit
        /// </summary>
        public int _maxDepth = 5;

        /// <summary>
        /// Max distance the paths can traverse
        /// </summary>
        public float _maxLength = -1;

        /// <summary>
        /// Random seed, set to -1 the program will use the default random and not a fixed seed.
        /// </summary>
        public int _seed = 42;

        public float _requiredIntersectionLength = 3.0f;
        
        public DungeonLayout Traverse(DungeonLayout source, DungeonNode startNode = null)
        {            
            var initiailRandomState = UnityEngine.Random.state;

            if (_seed >= 0)
            {
                UnityEngine.Random.InitState(_seed);
            }

            var pathEnd = BuildPath(startNode == null ? source.GetRandomNode() : startNode, new HashSet<DungeonNode>());
            var pathStart = pathEnd;

            while (pathStart.Parent != null)
            {
                pathStart = pathStart.Parent;
            }

            if (_seed >= 0)
            {
                UnityEngine.Random.state = initiailRandomState;
            }

            return CreateDungeonLayout( pathStart, pathEnd
                                       , new DungeonNode(pathStart.DungeonElement.Rect) {  Name = pathStart.DungeonElement.Name }
                                       , new DungeonLayout());
        }
        
        private DungeonLayout CreateDungeonLayout(PathNode parentPathNode, PathNode endNode,
                                                    DungeonNode parentDungeonNode, DungeonLayout dungeon )
        {
            if (dungeon.Start == null)
            {
                dungeon.Start = parentDungeonNode;
            }

            if ( endNode == parentPathNode )
            {
                dungeon.End = parentDungeonNode;
            }

            dungeon.AddNode(parentDungeonNode);

            foreach (var child in parentPathNode.Children)
            {
                var childDungeonNode = new DungeonNode(child.DungeonElement.Rect) { Name = child.DungeonElement.Name };
                DungeonNode.Connect(parentDungeonNode, childDungeonNode);

                CreateDungeonLayout(child, endNode, childDungeonNode, dungeon);
            }

            return dungeon;
        }

        /// <summary>
        /// Build a path with the given split as a root, returns the root node of the path
        /// </summary>
        /// <param name="dungeonElement"></param>
        /// <param name="closedList"></param>
        /// <returns></returns>
        private PathNode BuildPath(DungeonNode dungeonElement, HashSet<DungeonNode> closedList)
        {
            var currentNode = CreateNode(null, dungeonElement, closedList);
            var deepestNode = currentNode;

            for (int i = 0; CanContinueIteration(i, currentNode); ++i)
            {
                var neighbourElement = currentNode.DungeonElement.SelectRandomNeighbour((edge, neighbour) =>
                                            !closedList.Contains(neighbour) 
                                            && edge.IntersectionLength >= _requiredIntersectionLength);
                    
                if (neighbourElement == null)
                {
                    // backtrack - the loop will break if there are no more children
                    currentNode = currentNode.Parent;
                }
                else
                {
                    currentNode = CreateNode(currentNode, neighbourElement, closedList);

                    if (currentNode.PathLength > deepestNode.PathLength)
                    {
                        deepestNode = currentNode;
                    }
                }
            }

            return deepestNode;
        }

        private bool CanContinueIteration(int i, PathNode current)
        {
            return (i < _maxDepth || _maxDepth == -1)
                && (current != null)
                && (current.PathLength < _maxLength || _maxLength == -1);
        }

        private PathNode CreateNode(PathNode parent, DungeonNode dungeonElement, HashSet<DungeonNode> closedList)
        {
            var result = new PathNode()
            {
                DungeonElement = dungeonElement,
                Parent = parent,
                PathLength = parent == null ? 0 : parent.PathLength + dungeonElement.Distance(parent.DungeonElement)
            };

            closedList.Add(dungeonElement);

            if (parent != null)
            {
                parent.AddChild(result);
            }

            return result;
        }
    }
}
