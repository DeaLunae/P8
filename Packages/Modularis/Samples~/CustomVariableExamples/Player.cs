using System;
using UnityEngine;

namespace Devkit.Modularis.Examples
{
    [Serializable]
    public class Player
    {
        public string name;
        [Range(1,2)]public int HP;
        
        public int AP;
        public SubPlayer SubPlayer;
        public int AD;
    }

    [Serializable]
    public class SubPlayer
    {
        public int weight;
        public Vector3 pos;
    }
}