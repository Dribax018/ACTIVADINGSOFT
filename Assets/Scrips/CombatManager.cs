using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class CombatManager : MonoBehaviour
{
    [Header("Arrastra aquí a los 3 enemigos")]
    [SerializeField] private Enemy[] enemies = new Enemy[3];

    // COLA: orden de turnos
    private readonly Queue<Enemy> turnQueue = new Queue<Enemy>();
    private bool inCombat;

    private void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        if (kb.bKey.wasPressedThisFrame) StartCombat();
        if (kb.tKey.wasPressedThisFrame) NextTurn();
    }

    public void StartCombat()
    {
        if (inCombat)
        {
            Debug.Log("[Combate] El combate ya ha empezado.");
            return;
        }

        turnQueue.Clear();
        foreach (Enemy enemy in enemies)
        {
            if (enemy == null) continue;
            turnQueue.Enqueue(enemy);
            Debug.Log($"[Combate] {enemy.enemyName} entra en combate (posición {turnQueue.Count}).");
        }

        inCombat = true;
    }

    public void NextTurn()
    {
        if (!inCombat || turnQueue.Count == 0)
        {
            Debug.Log("[Combate] No hay combate en curso. Pulsa B para empezar.");
            return;
        }

        Enemy current = turnQueue.Dequeue();
        current.Attack();
        turnQueue.Enqueue(current);          
    }
}