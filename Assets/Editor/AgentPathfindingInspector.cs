/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.DungeonPathfinding
{
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Editor component which allows generating levels in the editor (ie it adds some buttons)
    /// </summary>
    [CustomEditor(typeof(AgentPathfindingTestBehaviour))]
    public class AgentPathfindingInspector : Editor
    {
        private AgentPathfindingTestBehaviour _testBehaviour;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            _testBehaviour = (AgentPathfindingTestBehaviour)target;

            if (_testBehaviour.IsFollowingTarget)
            {
                if (GUILayout.Button("Stop following target"))
                {
                    _testBehaviour.ToggleFollowTarget();
                    EditorApplication.update -= OnUpdate;

                }
            }
            else
            {
                if (GUILayout.Button("Start following target"))
                {
                    _testBehaviour.ToggleFollowTarget();
                    EditorApplication.update += OnUpdate;
                }
            }
        }

        public void OnUpdate()
        {
            _testBehaviour.OnUpdate();
        }
    }
}

