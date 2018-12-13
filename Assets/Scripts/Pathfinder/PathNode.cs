/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.PathFinder
{
    using System.Collections.Generic;

    public class PathNode<T>
    {
        public PathNode<T> _parent;
        public List<PathNode<T>> _children = new List<PathNode<T>>();
        public float _cost = 0;
        public float _pathLength = 0;
        public T _data;

        public void AddChild(PathNode<T> node)
        {
            _children.Add(node);
            node._parent = this;
        }
    }
}
