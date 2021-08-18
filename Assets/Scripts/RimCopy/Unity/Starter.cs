using RimCopy;
using UnityEngine;

namespace DefaultNamespace
{
    public class Starter : MonoBehaviour
    {
        private void Awake()
        {
            Game.StartGame();
        }
    }
}