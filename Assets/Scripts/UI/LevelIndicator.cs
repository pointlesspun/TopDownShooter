/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.UI
{
    using Tds.GameScripts;
    using UnityEngine;
    using UnityEngine.UI;

    public class LevelIndicator : MonoBehaviour
    {
        private Text _text;
        private GameStateBehaviour _gameState;

        void Start()
        {
            _text = GetComponent<Text>();
            _gameState = GameStateBehaviour.Resolve();

            if (_gameState != null)
            {
                _text.text = _gameState._level.ToString();
            }
        }
    }
}
