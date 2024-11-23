using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SlashParadox.Essence
{
    public abstract class GameMode : EssenceBehaviour
    {
        private List<PlayerController> _players = new List<PlayerController>();
    }
}
