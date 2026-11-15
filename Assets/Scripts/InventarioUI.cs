using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DatosFruta
{
    public string nombre;
    public Sprite icono;

    public DatosFruta(string nombre, Sprite icono)
    {
        this.nombre = nombre;
        this.icono = icono;
    }
}
public class InventarioUI : MonoBehaviour
{
    public Transform contenedorFrutas;      
    public GameObject prefabBotonFruta;     
    public Button botonUndo;                
    public TextMeshProUGUI textoContador;   
    private List<DatosFruta> inventario = new List<DatosFruta>();
    private Stack<DatosFruta> pilaDeshacer = new Stack<DatosFruta>();

    void Start()
    {
        //botonUndo.onClick.AddListener(DeshacerConUndo);
        ActualizarInterfaz();
    }

    public void RecogerFruta(string nombre, Sprite icono)
    {
        DatosFruta nuevaFruta = new DatosFruta(nombre, icono);
        inventario.Add(nuevaFruta);
        ActualizarInterfaz();
    }

    public void EliminarFruta(DatosFruta frutaSeleccionada)
    {
        if (inventario.Remove(frutaSeleccionada))
        {
            pilaDeshacer.Push(frutaSeleccionada);
            ActualizarInterfaz();
        }
    }

    public void DeshacerConUndo()
    {
        if (pilaDeshacer.Count > 0)
        {
            DatosFruta frutaRecuperada = pilaDeshacer.Pop();
            inventario.Add(frutaRecuperada);
            ActualizarInterfaz();
        }
    }

    private void ActualizarInterfaz()
    {
        foreach (Transform hijo in contenedorFrutas)
        {
            Destroy(hijo.gameObject);
        }

        foreach (DatosFruta fruta in inventario)
        {
            GameObject nuevoIcono = Instantiate(prefabBotonFruta, contenedorFrutas);
            nuevoIcono.GetComponent<Image>().sprite = fruta.icono;

            DatosFruta frutaActual = fruta;
            nuevoIcono.GetComponent<Button>().onClick.AddListener(() => EliminarFruta(frutaActual));
        }
        textoContador.text = "Objetos: " + inventario.Count;

        botonUndo.gameObject.SetActive(pilaDeshacer.Count > 0);
    }
}