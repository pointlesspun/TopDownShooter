/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Component capturing the behaviour of a gamepoint with hitpoints specifically for the player
    /// </summary>
    public class PlayerHitpoints : Hitpoints
    {
        public override void Start()
        {
            base.Start();

            // if there are no hitpoints stored in the gamestate use the default hitspoints
            _hitpoints = GlobalGameState._playerHealth < 0 ? _maxHitpoints : GlobalGameState._playerHealth;
        }
    }
}
