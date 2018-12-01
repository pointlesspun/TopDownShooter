/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.Util
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Utility to define pre conditions
    /// </summary>
    public static class Contract
    {
        /// <summary>
        /// The given test needs to be true otherwise the application may get in an error state.
        /// When test is false will stop the unity editor.
        /// </summary>
        /// <param name="test"></param>
        /// <param name="message"></param>
        public static void Requires( bool test, string message)
        {
            if (!test)
            {
                Debug.LogError("Failed requirement: " + message);
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                throw new InvalidProgramException("Failed requirement: " + message);
#endif                
            }
        }

        /// <summary>
        /// The given gameobject requires the given component.
        /// When component is not present this will stop the unity editor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameObject"></param>
        /// <param name="message"></param>
        public static void RequiresComponent<T>( GameObject gameObject, string message)
        {
            if ( gameObject.GetComponent<T>() == null)
            {
                Debug.LogError("Game object requires component: " + typeof(T) + " but is missing. " + message);
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                throw new InvalidProgramException("Failed requirement: " + message);
#endif
            }
        }
    }
}
