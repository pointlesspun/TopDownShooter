﻿/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.DungeonGeneration
{
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Editor component which allows generating levels in the editor (ie it adds some buttons)
    /// </summary>
    [CustomEditor(typeof(SubdivisionAlgorithmTestBehaviour))]
    public class SubdivisionAlgorithmInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SubdivisionAlgorithmTestBehaviour testBehaviour = (SubdivisionAlgorithmTestBehaviour)target;

            if (GUILayout.Button("Build Level"))
            {
                testBehaviour.BuildLevel();
            }

            if (GUILayout.Button("Clear Level Debug Elements"))
            {
                testBehaviour.ClearDebugObjects();
            }
        }
    }
}
