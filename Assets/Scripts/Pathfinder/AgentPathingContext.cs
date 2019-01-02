/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.PathFinder
{
    using UnityEngine;

    public struct AgentPathingContext<T> where T : class
    {
        public AgentPathfindingSettings settings;
        public ISearchSpace<T, Vector2> searchSpace;
        public SearchService<T> service;
        public float time;

        /// <summary>
        /// Checks if all required members are presetn
        /// </summary>
        /// <returns></returns>
        public bool IsInitialized()
        {
            return settings != null && searchSpace != null && service != null;
        }

        public bool IsTimeout(float checkpoint)
        {
            return (time - checkpoint) > settings.pathValidatyCheckTimeout;
        }
    }
}
