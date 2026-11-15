using UnityEngine;

public class Collectible : MonoBehaviour
{
    public string nombreFruta = "Manzana";
    public Sprite imagenFruta;
    public InventarioUI gestorInventario;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gestorInventario.RecogerFruta(nombreFruta, imagenFruta);
            Destroy(gameObject); // Desaparece la fruta del suelo
        }
    }
}
