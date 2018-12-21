/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */


namespace Tds.DungeonPathfinding
{
    using UnityEngine;

    using Tds.DungeonGeneration;
    using Tds.PathFinder;
    using Tds.Util;

    /// <summary>
    /// Search space implementation for a layout
    /// </summary>
    public class DungeonLayoutSearchSpace : ISearchSpace<DungeonNode, Vector2>
    {
        public DungeonLayout Layout
        {
            get;
            set;
        }

        public DungeonNode FindNearestSolution(Vector2 location, float maxDistance)
        {
            return Layout.FindNearestSolution(location, maxDistance);
        }

        public Vector2 GetInterpolatedLocation(DungeonNode from, DungeonNode to, float value, Vector2 fallbackLocation)
        {
            var edge = from.GetEdgeTo(to);

            if (edge != null)
            {
                return edge.NodeIntersection.Interpolation(value);
            }

            return fallbackLocation;
        }

        public DungeonNode GetRandomElement()
        {
            return Layout.GetRandomElement();
        }

        public void GetWaypoints(DungeonNode from, DungeonNode to, Vector2[] waypoints, Vector2 offset, bool randomize)
        {
            Contract.RequiresNotNull(from, "cannot get waypoints from a null 'from' node.");
            Contract.RequiresNotNull(to, "cannot get waypoints from a null 'to' node.");
            Contract.Requires(from != to, "cannot get waypoints when a 'from' is the same as 'to' node.");
            Contract.RequiresNotNull(from.GetEdgeTo(to), "cannot get waypoints when there is no edge between 'from' and 'to' node.");

            var edge = from.GetEdgeTo(to);
            var crossOffset = 0.5f;

            if (randomize)
            {
                var length = edge.NodeIntersection.Length;
                var value = 0.5f * (length - 1.0f) / length;

                crossOffset = Random.Range(0.5f - value, 0.5f + value);
            }

            var cross = edge.NodeIntersection.Cross(crossOffset);

            if (from.ContainsPoint(cross.to))
            {
                waypoints[0] = cross.to + offset;
                waypoints[1] = cross.from - (cross.to - cross.from) + offset;
            }
            else
            {
                waypoints[0] = cross.from - (cross.to - cross.from) + offset;
                waypoints[1] = cross.to + offset;
            }
        }
    }
}
