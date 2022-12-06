using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;


[Serializable]
public class ColorEvent : UnityEvent<Color> { };


public class ColorPicker : MonoBehaviour
{
    #region Singleton ColorPicker
    public static ColorPicker instanceColorPicker;
    private void Awake()
    {
        instanceColorPicker = this;
    }
    #endregion

    RectTransform transform2D;
    Texture2D colorPicker;

    [SerializeField]
    Text previewText;
    [SerializeField]
    Image previewImage;

    public static ColorEvent OnColorPreview;
    //public static ColorEvent OnColorSelect;

    public event EventHandler OnColorSelect;

    // Start is called before the first frame update
    void Start()
    {
        transform2D = GetComponent<RectTransform>();
        colorPicker = GetComponent<Image>().mainTexture as Texture2D;
    }

    Vector2 delta;
    float width, height, x, y;
    int texX, texY;
    public Color color;
    // Update is called once per frame
    void Update()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform2D, Input.mousePosition, null, out delta);
        width = transform2D.rect.width;
        height = transform2D.rect.height;

        delta += new Vector2(width * 0.5f, height * 0.5f);
        x = Mathf.Clamp(delta.x / width, 0f, 1f);
        y = Mathf.Clamp(delta.y / height, 0f, 1f);

        texX = Mathf.RoundToInt(x * colorPicker.width);
        texY = Mathf.RoundToInt(y * colorPicker.height);

        color = colorPicker.GetPixel(texX, texY);

        OnColorPreview?.Invoke(color);
        if (Input.GetMouseButtonDown(0) && RectTransformUtility.RectangleContainsScreenPoint(transform2D, Input.mousePosition))
        {
            previewText.text = ColorUtility.ToHtmlStringRGB(color);
            previewImage.color = color;
            OnColorSelect?.Invoke(this, EventArgs.Empty);

        }
    }

    
    
}
