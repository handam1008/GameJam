using UnityEngine;

public abstract class AbstractItem : ScriptableObject
{
    public Sprite icon;
    
    
    public abstract void Use(GameObject target);
   
}
