using UnityEngine;

public class Creck : MonoBehaviour
{
    int count = 0;
    public Sprite[] spp;
    SpriteRenderer sp;
    private void Awake()
    {
        sp = GetComponent<SpriteRenderer>();
    }
    public void Crecker()
    {
        sp.sprite = spp[count];
        count++;
    }
}