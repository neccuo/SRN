using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCard : MonoBehaviour
{
    public List<Card> cardList;

    [SerializeField] private List<GameObject> hand;

    public void Awake()
    {
        hand = new List<GameObject>();
    }

    public void Start() 
    {
        foreach(Transform t in transform)
        {
            hand.Add(t.gameObject);
        }
    }

    public void Draw()
    {
        
    }
        

}
