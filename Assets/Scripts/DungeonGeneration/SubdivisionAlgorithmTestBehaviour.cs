/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.DungeonGeneration
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Behaviour providing an interface to define and test level generation
    /// </summary>
    public class SubdivisionAlgorithmTestBehaviour : MonoBehaviour
    {
        public SubdivisionAlgorithm _algorithm = new SubdivisionAlgorithm();
        public GameObject _rectPrefab;
        public Color[] _rectColors = new Color[] { Color.white, Color.yellow, Color.cyan, Color.blue, Color.green, Color.red };
        public int _width = 20;
        public int _height = 20;
        public float _scaleMultiplier = 3;

        private List<GameObject> _debugObjects = new List<GameObject>();

        public void Start()
        {
            BuildLevel();
        }

        public void BuildLevel()
        {
            ClearDebugObjects();
            CreateDebugObjects(_algorithm.Subdivide(new SplitRect(0, 0, _width, _height)));
        }

        private void CreateDebugObjects(List<SplitRect> splitRectangles)
        {
            var colorIndex = 0;
            var offset = new Vector3(-_width * 0.5f, -_height * 0.5f, 0);

            splitRectangles.ForEach((split) =>
            {
                var debugObject = Instantiate(_rectPrefab);

                debugObject.transform.parent = gameObject.transform;

                debugObject.name = "splitrect: " + split._rect;

                debugObject.transform.position = new Vector3(split._rect.position.x + 0.5f * split._rect.width
                                                       , split._rect.position.y + 0.5f * split._rect.height
                                                       , 0) + offset;

                debugObject.transform.localScale = new Vector3(split._rect.width * _scaleMultiplier
                                                        , split._rect.height * _scaleMultiplier,  1);

                debugObject.GetComponent<SpriteRenderer>().color = _rectColors[colorIndex % _rectColors.Length];
                debugObject.GetComponent<SplitRectDebugBehaviour>()._rect = split;

                split.DebugElement = debugObject;

                _debugObjects.Add(debugObject);

                colorIndex++;

            });
        }

        public void ClearDebugObjects()
        {
            if (Application.isEditor)
            {
                _debugObjects.ForEach(obj => DestroyImmediate(obj));
            }
            else
            {
                _debugObjects.ForEach(obj => Destroy(obj));
            }

            _debugObjects.Clear();
        }
    }
}
