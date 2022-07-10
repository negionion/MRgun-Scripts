using UnityEngine;
using System;
public enum Mode
{
    PVE,
    PVP,
    MIX
}

public static class Game
{
    public static Mode mode = Mode.MIX;
}

public class GameMode : MonoBehaviour
{
    public void selectGameMode(string _mode)
    {
        
        Game.mode = (Mode)Enum.Parse(typeof(Mode), _mode, false);
    }
}

