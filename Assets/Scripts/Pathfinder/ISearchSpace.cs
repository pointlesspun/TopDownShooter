/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.PathFinder
{
    /// <summary>
    /// Definition of a search space (https://en.wikipedia.org/wiki/Search_algorithm).
    /// </summary>
    /// <typeparam name="TElement"></typeparam>
    /// <typeparam name="TLocation"></typeparam>
    public interface ISearchSpace<TElement, TLocation> where TElement : class
    {
        /// <summary>
        /// Finds the nearest solution to a given location in the searchspace. Eg in case of a grid2d with T = string,
        /// the 'location' would be a 2d vector and the element a string
        /// </summary>
        /// <param name="location">Location in the search space</param>
        /// <param name="maxDistance">Max distance to the solution, if less than 0 no max distance is defined</param>
        /// <returns>The nearest element or null if there is no such element</returns>
        TElement FindNearestSolution(TLocation location, float maxDistance);

        /// <summary>
        /// Returns a location on the path when traversing from the 'from' element to the 'to' element. 
        /// Eg in case of a grid2d with T = Rect, the location from one rect to another could be a
        /// lerp between the two centers.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        TLocation GetInterpolatedLocation(TElement from, TElement to, float value, TLocation fallbackLocation);

        /// <summary>
        /// Creates waypoints from the 'from' node to the 'to' node.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="waypoints"></param>
        void GetWaypoints(TElement from, TElement to, TLocation[] waypoints, TLocation offset, bool randomize);

        /// <summary>
        /// Utility function returning a random element from the searchspace
        /// </summary>
        /// <returns></returns>
        TElement GetRandomElement();

        /// <summary>
        /// Verifies if these two elements are neighbours
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        bool AreNeighbours(TElement from, TElement to);
    }
}
