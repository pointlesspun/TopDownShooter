﻿/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameStateScripts
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
        /// Player object- if this is null or not active, assumes it's a game over state and
        /// will load the title screen
        /// </summary>
        private GameObject _player;

        public void Start()
        {
            _player = GameObject.FindGameObjectWithTag(GameTags.Player);
            GlobalGameState._playerObject = _player;
        }

        public void Update()
        {
            if (_player == null || !_player.activeInHierarchy)
            {
                SceneManager.LoadScene(_titleScreenScene);
            }
        }
    }
}
