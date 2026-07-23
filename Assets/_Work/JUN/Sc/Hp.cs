using UnityEngine;
using UnityEngine.InputSystem;

public class Hp : MonoBehaviour
{
    public MegaPlayer player;
    public Creck creck;
    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            player.Bounce();
            creck.Crecker();
        }
    }
}
