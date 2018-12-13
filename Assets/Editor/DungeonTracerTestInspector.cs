/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.PathFinder
{
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Editor component which allows generating levels in the editor (ie it adds some buttons)
    /// </summary>
    [CustomEditor(typeof(DungeonTracerTestBehaviour))]
    public class DungeonTracerTestInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            DungeonTracerTestBehaviour testBehaviour = (DungeonTracerTestBehaviour)target;

            if ( testBehaviour.IsFollowingTarget)
            {
                if (GUILayout.Button("Stop following target"))
                {
                    testBehaviour.ToggleFollowTarget();
                }
            }
            else
            {
                if (GUILayout.Button("Start following target"))
                {
                    testBehaviour.ToggleFollowTarget();
                }
            }
            
        }
    }
}
