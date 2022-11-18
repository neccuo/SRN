using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CardDisplay : MonoBehaviour
{
    private CardData _cardData;
    private Card _card;

    public Image cardBG;
    public Text cardNameText;


    public Text descriptionText;
    public Image artworkImage;


    public Text manaText;
    public Text attackText;
    public Text healthText;



    private void Start()
    {
        _cardData = GetComponent<CardData>();
        _card = _cardData.card;
        SetAttributes();
    }

    private void SetAttributes()
    {
        cardNameText.text = _card.name;
        descriptionText.text = _card.description;
        artworkImage.sprite = _card.artwork;
        manaText.text = _card.manaCost.ToString();
        attackText.text = _card.attack.ToString();
        healthText.text = _card.health.ToString();
        SetColor();
    }

    private void SetColor()
    {
        CardType cardType = _card.cardType;
        Color bgColor = Color.gray;
        switch(cardType)
        {
            case CardType.Standard:
                bgColor = Color.gray;
                break;
            case CardType.Shield:
                bgColor = Color.blue;
                break;
            case CardType.Attack:
                bgColor = Color.red;
                break;
            case CardType.Recovery:
                bgColor = Color.green;
                break;
            default:
                Debug.LogError("Something went wrong");
                break;
        }
        cardBG.color = bgColor;
    }

}
