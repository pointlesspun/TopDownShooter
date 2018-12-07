/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.DungeonGeneration
{
    using UnityEngine;

    /// <summary>
    /// Behaviour to add all debug related functionality to splitRects
    /// </summary>
    public class TraversalNodeDebugBehaviour : MonoBehaviour
    {
        public TraversalNode _node;
        public float _scale = 1.0f;
        public Vector3 _offset = Vector3.zero;

        public void OnDrawGizmos()
        {
            if (_node != null)
            {
                Gizmos.color = Color.black;

                // draw a line to - and the intersection with the parent
                if ( _node._parent != null )
                {
                    var intersection = GetIntersectionVectors(_node._parentIntersection);
                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(intersection[0], intersection[1]);

                    Gizmos.color = Color.black;
                    Gizmos.DrawLine(gameObject.transform.position, intersection[0] + (intersection[1] - intersection[0]) * 0.5f);
                }
                // draw lines to all children
                foreach (var child in _node._children)
                {
                    if (child.DebugElement != null)
                    {
                        var intersection = GetIntersectionVectors(child._parentIntersection);
                        Gizmos.color = Color.black;
                        Gizmos.DrawLine(gameObject.transform.position, intersection[0] + (intersection[1] - intersection[0]) * 0.5f);
                    }
                }
            }
        }

        private Vector3[] GetIntersectionVectors(Vector2Int[] v)
        {
            return new Vector3[]
            {
                new Vector3(v[0].x, v[0].y, 0) + _offset,
                new Vector3(v[1].x, v[1].y, 0) + _offset
            };
        }

        private Vector3 GetIntersectionPoint( Vector2Int[] v)
        {
            Vector3 va = new Vector3(v[0].x, v[0].y, 0);
            Vector3 vb = new Vector3(v[1].x, v[1].y, 0);

            return va + ((vb - va) * 0.5f);
        }
    }
}
