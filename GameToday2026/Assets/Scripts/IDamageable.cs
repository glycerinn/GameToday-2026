using Unity.VisualScripting;
using UnityEngine;

public interface IDamageable
{
    public float health { get; set; }
    public void takeDamage(float damage);
    
}