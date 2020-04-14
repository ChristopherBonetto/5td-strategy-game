using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HFRadialLayout : LayoutGroup
{
    public float Radius;

    public override void CalculateLayoutInputVertical()
    {

        for (int i = 0; i < rectChildren.Count; i++)
        {
            float angle = 360 / rectChildren.Count * i;
            SetChildAlongAxis(rectChildren[i], 0, Radius * Mathf.Sin(angle * Mathf.Deg2Rad));
            SetChildAlongAxis(rectChildren[i], 1, Radius * -Mathf.Cos(angle * Mathf.Deg2Rad));
        }
    }

    public override void SetLayoutHorizontal()
    {
    }

    public override void SetLayoutVertical()
    {
    }
}
