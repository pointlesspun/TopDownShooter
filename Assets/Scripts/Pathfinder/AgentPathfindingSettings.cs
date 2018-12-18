/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.PathFinder
{
    using System;
    using UnityEngine;

    [Serializable]
    public class AgentPathfindingSettings
    {
        /// <summary>
        /// Distance from the current target location to the state's target location at which the current path will be 
        /// invalid.
        /// </summary>
        public float targetDistanceThreshold;

        /// <summary>
        /// Time at which a path update will be considered to prevent too much oscillation between different states 
        /// </summary>
        public float pathValidatyCheckTimeout;

        /// <summary>
        /// Distance to waypoint where it is considered 'reached'
        /// </summary>
        public float waypointDistance;

        /// <summary>
        /// Offset of the world 
        /// </summary>
        public Vector2 worldOffset = Vector3.zero;
    }
}
