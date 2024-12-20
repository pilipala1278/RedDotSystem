using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonRightLeftClick : MonoBehaviour, IPointerClickHandler
{
    private Color _redColor = new Color(1f, 83f / 255, 74f / 255);

    public Action OnLeftClick;
    public Action OnRightClick;

    private Image _imaget;
    private Text _nameText;
    private Text _valueText;

    private void Awake()
    {
        _imaget = GetComponent<Image>();
        _valueText = transform.Find("Text").GetComponent<Text>();
        _nameText = transform.Find("NameText").GetComponent<Text>();
    }

    public void SetNameText(string str)
    {
        _nameText.text = str;
    }

    public void SetValue(int value)
    {
        if (value > 0)
        {
            _imaget.color = _redColor;
        }
        else
        {
            _imaget.color = Color.white;
        }

        _valueText.text = value.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick?.Invoke();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick?.Invoke();
        }
    }
}
