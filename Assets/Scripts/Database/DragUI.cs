using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DragUI : VisualElement

{
    [UnityEngine.Scripting.Preserve]
    public new class UxmlFactory : UxmlFactory<DragUI> { }


    private UIController rootDragger;
    public DragUI()
    {
        VisualElement element = new VisualElement();


        element.RegisterCallback<MouseDownEvent>(OnMouseDown, TrickleDown.TrickleDown);
        element.RegisterCallback<MouseUpEvent>(OnMouseUp, TrickleDown.TrickleDown);
        element.style.width = new Length(100, LengthUnit.Percent);
        element.style.height = new Length(100, LengthUnit.Percent);

        element.style.backgroundColor = new Color(127f / 255f, 118f / 255f, 121f / 255f);

        rootDragger = GameObject.FindGameObjectWithTag("uiDrag").GetComponent<UIController>();

        hierarchy.Add(element);
    }

    private void OnMouseUp(MouseUpEvent evt)
    {
        VisualElement targetElement = (VisualElement)evt.target;
        targetElement.style.backgroundColor = new Color(127f / 255f, 118f / 255f, 121f / 255f);

        rootDragger.SetDrag(false);
    }

    private void OnMouseDown(MouseDownEvent evt)
    {
        VisualElement targetElement = (VisualElement)evt.target;
        targetElement.style.backgroundColor = new Color(197f / 255f, 198f / 255f, 191f / 255f);

        rootDragger.SetDrag(true);

    }
}
