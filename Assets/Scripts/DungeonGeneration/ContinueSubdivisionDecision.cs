/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.DungeonGeneration
{
    /// <summary>
    /// Enum capturing the decision whether or not to continue splitting a rect.
    /// Mostly for debug purposes
    /// </summary>
    public enum ContinueSubdivisionDecision
    {
        /// <summary>
        /// Rect can potentially still be divivde d over the other axis
        /// </summary>
        AttemptOtherAxis,

        /// <summary>
        /// Randomly decided not to split this any further
        /// </summary>
        RandomRollFailed,

        /// <summary>
        /// MAx depth has been reached, no need to further divide this
        /// </summary>
        MaxDepthReached,

        /// <summary>
        /// No more space , cannot subdivide this rect any further
        /// </summary>
        RectTooSmall,

        /// <summary>
        /// Do continue
        /// </summary>
        ContinueSubdivision
    }
}
