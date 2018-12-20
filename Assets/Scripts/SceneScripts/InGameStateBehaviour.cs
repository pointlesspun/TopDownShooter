/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.SceneScripts
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    using Tds.GameScripts;
    using Tds.DungeonGeneration;
    using Tds.Util;

    /// <summary>
    /// Expressed the behavior of the state when in game. Will track the state of the player.
    /// When the player is dead, will end the game state and go to the the title screen.
    /// </summary>
    public class InGameStateBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Name of the scene containing the title page (for game over situations)
        /// </summary>
        public string _titleScreenScene;

        /// <summary>
        /// Name of the scene containing the next level
        /// </summary>
        public string _nextLevelScene;

        /// <summary>
        /// Player object- if this is null or not active, assumes it's a game over state and
        /// will load the title screen
        /// </summary>
        private GameObject _player;
       
        private LevelBehaviour _level;
        private Director  _director;
        private GameStateBehaviour _gameState;

        public void Start()
        {
            // link all the object dependencies
            _player = GameObject.FindGameObjectWithTag(GameTags.Player);

            _level = CommonExtensions.RetrieveComponent<LevelBehaviour>(GameTags.LevelLayout);
            _director = CommonExtensions.RetrieveComponent<Director>(GameTags.Director, true);
            _gameState = CommonExtensions.RetrieveComponent<GameStateBehaviour>(GameTags.GameState, true);

            _level.BuildGrid(_gameState == null ? 1 : _gameState._levelScale);
            _player.transform.position = _level.StartPosition;

            if (_director != null)
            {
                _director.SetDungeonLayout(_level._layout, _level._levelOffset);
            }        
        }
        
        public void Update()
        {
            if (_player == null || !_player.activeInHierarchy)
            {
                // game over...
                SceneManager.LoadScene(_titleScreenScene);
            }
            else if (_level != null)
            {
                _level.UpdateFocus(_player.transform.position);
            }
        }

        /// <summary>
        /// Callback when the player reaches the exit so the game needs to transition to the next scene.
        /// </summary>
        public void OnPlayerReachesExit()
        {
            SceneManager.LoadScene(_nextLevelScene);
        }
    }
}
