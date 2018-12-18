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
        private GameObject _levelGridObject;
        private GameObject _directorObject;

        private LevelGrid _levelGrid;
        private Director  _director;
        
        public void Start()
        {
            // link all the object dependencies
            _player = GameObject.FindGameObjectWithTag(GameTags.Player);
            _levelGridObject = GameObject.FindGameObjectWithTag(GameTags.LevelLayout);
            _directorObject = GameObject.FindGameObjectWithTag(GameTags.Director);

            if (_levelGridObject != null && _player != null)
            {
                _levelGrid = _levelGridObject.GetComponent<LevelGrid>();

                if (_levelGrid != null)
                {
                    _player.transform.position = _levelGrid.StartPosition;

                    if (_directorObject != null)
                    {
                        _director = _directorObject.GetComponent<Director>();

                        if (_director != null)
                        {
                            _director.SetDungeonLayout(_levelGrid._layout, _levelGrid._levelOffset);
                        }
                    }
                }
            }
        }

        public void Update()
        {
            if (_player == null || !_player.activeInHierarchy)
            {
                // game over...
                SceneManager.LoadScene(_titleScreenScene);
            }
            else if (_levelGrid != null)
            {
                _levelGrid.UpdateFocus(_player.transform.position);
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
