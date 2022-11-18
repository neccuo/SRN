using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class CardMovementHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler ,IPointerExitHandler
{
    private Vector3 originalPos;
    private Vector3 originalScale;

    public float dragSpeed = 4000.0f;
    private bool _isDragged = false;
    private bool _isHover = false;

    // RectTransform rectTransform = GetComponent<RectTransform>();

    public void OnBeginDrag(PointerEventData eventData)
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        _isDragged = true;
        _isHover = false;
        transform.position = Vector3.MoveTowards(transform.position, eventData.position, dragSpeed * Time.deltaTime);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _isDragged= false;
        // if on PlayArea, do something else

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        foreach(var raycastResult in raycastResults)
        {
            GameObject resObj = raycastResult.gameObject;
            // SHITTY HACK, PLEASE CHANGE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            if(resObj.name == "PlayArea")
            {
                CombatManager.Instance.CardPlayed(gameObject);
                Destroy(gameObject);
                return;
            }
        }

        transform.position = originalPos;


        // TODO: ADD ANIMATIONS LATER, NOT NOW...
        // transform.position = Vector3.MoveTowards(transform.position, originalPos, dragSpeed * Time.deltaTime);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Icerdeyim");
        //_isHover = true;
        //transform.localScale = Vector3.one;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //_isHover = false;
        //transform.position = originalPos;
    }

    void Start()
    {
        originalPos = transform.position;
        originalScale = transform.localScale;
    }

    void Update()
    {
        /*if(!_isDragged)
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPos, dragSpeed * Time.deltaTime);
        }
        if(_isHover)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, -100);
        }
        else
        {
            transform.localScale = originalScale;
        }*/

    }

    
}
