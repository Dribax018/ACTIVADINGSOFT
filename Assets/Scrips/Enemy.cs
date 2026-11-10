using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyName = "Enemigo";
    public int damage = 10;
    public void Attack()
    {
        Debug.Log($"[Combate] {enemyName} ataca y causa {damage} de daño" );
    }
}
