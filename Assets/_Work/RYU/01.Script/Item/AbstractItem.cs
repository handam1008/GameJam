using UnityEngine;

public abstract class AbstractItem : ScriptableObject
{
    public Sprite icon;
    public string description;
    
    
    public abstract void Use(GameObject target);
   
}
