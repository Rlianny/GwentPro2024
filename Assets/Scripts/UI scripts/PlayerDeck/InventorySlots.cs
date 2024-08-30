using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventorySlots : MonoBehaviour
{   
    public TextMeshProUGUI CardCounter;

    private void Start()
    {
        //var CardCounter = this.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        if (this.transform.parent.transform.GetChild(0).childCount != 0)      // si el slot muestra las cartas de la lista de cartas del deck y no está vacío
        {
            GameObject cardInSlot = this.transform.parent.transform.GetChild(0).transform.GetChild(0).gameObject;
            UICard uiCard = cardInSlot.GetComponent<UICard>();
            Card card = uiCard.MotherCard;
            
            CardCounter.text = "x" + CardShowerManager.actualDeck.CardActualAppearances(card).ToString();
            ShowText(CardCounter);      // se muestra el contador que lleva la cantidad de veces que aparece la carta en la lista de cartas del deck
        }

        else if (this.transform.childCount == 0)        // si el slot está vacío
            HideText(CardCounter);      // se oculta el contador

    }

    private void ShowText(TextMeshProUGUI text)
    {
        // Mostrar el texto
        if (text != null)
            text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
    }

    private void HideText(TextMeshProUGUI text)
    {
        // Ocultar el texto
        if (text != null)
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
    }
}