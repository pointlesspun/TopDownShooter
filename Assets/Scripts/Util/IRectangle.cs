/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.Util
{
    using UnityEngine;

    /// <summary>
    /// Interface which is used (for instance) to define constraints on generics where the 
    /// type is requied to have a 'bounds' member
    /// </summary>
    public interface IBounds
    {
        Rect Bounds { get; }
    }

}
