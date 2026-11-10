
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Héroe 2D: recoge objetos (List), los consulta/elimina y realiza
/// acciones que se pueden deshacer (Stack).
///
/// Requisitos:
/// - Collider2D
/// - Rigidbody2D (Body Type: Kinematic)
///
/// Controles:
/// Flechas -> Movimiento
/// I        -> Consultar inventario
/// R        -> Eliminar objeto
/// 1        -> Acción: mover
/// 2        -> Acción: agrandar
/// 3        -> Acción: cambiar color
/// U        -> Deshacer última acción
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Hero : MonoBehaviour
{
    [Header("Movimiento libre")]
    [SerializeField] private float speed = 5f;
    [Header("Inventario")]
    [SerializeField] private string itemToRemove = "Objeto 1";
    [Header("Acciones")]
    [SerializeField] private Vector2 moveDelta = new Vector2(2f, 0f);
    [SerializeField] private float scaleFactor = 1.5f;
    [SerializeField] private Color newColor = Color.red;
    private readonly List<Item> inventory = new List<Item>();
    private readonly Stack<IGameAction> history = new Stack<IGameAction>();
    private SpriteRenderer rend;

    private void Awake()
    {
        rend = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        // Movimiento libre 2D
        Vector2 dir = Vector2.zero;

        if (kb.leftArrowKey.isPressed) dir.x -= 1f;
        if (kb.rightArrowKey.isPressed) dir.x += 1f;
        if (kb.upArrowKey.isPressed) dir.y += 1f;
        if (kb.downArrowKey.isPressed) dir.y -= 1f;

        transform.position += (Vector3)(dir.normalized * speed * Time.deltaTime);

        // Inventario
        if (kb.iKey.wasPressedThisFrame)
            Debug.Log($"[Inventario] Tengo {GetItemCount()} objeto(s).");

        if (kb.rKey.wasPressedThisFrame)
            RemoveItem(itemToRemove);

        // Acciones
        if (kb.digit1Key.wasPressedThisFrame)
            DoAction(new MoveAction(transform, moveDelta));

        if (kb.digit2Key.wasPressedThisFrame)
            DoAction(new ScaleAction(transform, scaleFactor));

        if (kb.digit3Key.wasPressedThisFrame && rend != null)
            DoAction(new ColorAction(rend, newColor));

        if (kb.uKey.wasPressedThisFrame)
            UndoLastAction();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Item item) && !inventory.Contains(item))
        {
            inventory.Add(item);
            item.gameObject.SetActive(false);

            Debug.Log($"[Inventario] Recogido: {item.itemName} (total: {inventory.Count})");
        }
    }

    public int GetItemCount()
    {
        return inventory.Count;
    }

    public bool RemoveItem(string itemName)
    {
        Item found = inventory.Find(i => i.itemName == itemName);

        if (found == null)
        {
            Debug.Log($"[Inventario] No tengo ningún objeto llamado '{itemName}'.");
            return false;
        }

        inventory.Remove(found);
        Destroy(found.gameObject);

        Debug.Log($"[Inventario] Eliminado: {itemName} (quedan: {inventory.Count})");
        return true;
    }

    private void DoAction(IGameAction action)
    {
        action.Execute();
        history.Push(action);

        Debug.Log($"[Acción] Realizada: {action.Name} (en la pila: {history.Count})");
    }

    private void UndoLastAction()
    {
        if (history.Count == 0)
        {
            Debug.Log("[Acción] No hay acciones que deshacer.");
            return;
        }

        IGameAction last = history.Pop();
        last.Undo();

        Debug.Log($"[Acción] Deshecha: {last.Name} (en la pila: {history.Count})");
    }
}
