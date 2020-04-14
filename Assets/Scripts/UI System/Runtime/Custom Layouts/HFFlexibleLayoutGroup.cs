using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HFFlexibleLayoutGroup : LayoutGroup
{
    public enum FitType
    {
        Uniform,
        Width,
        Height,
        FixedRows,
        FixedColumns,
    }

    public int Rows;
    public int Columns;

    public Vector2 CellSize;
    public Vector2 Spacing;

    public FitType m_FitType;
    public bool FitX;
    public bool FitY;

    public override void CalculateLayoutInputVertical()
    {
        switch (m_FitType)
        {
            case FitType.Width:
                Rows = Mathf.CeilToInt(transform.childCount / (float)Columns);
                break;
            case FitType.Height:
                Columns = Mathf.CeilToInt(transform.childCount / (float)Rows);
                break;
            case FitType.FixedRows:
                Rows = Mathf.CeilToInt(transform.childCount / (float)Columns);
                break;
            case FitType.FixedColumns:
                Rows = Mathf.CeilToInt(transform.childCount / (float)Columns);
                break;
            case FitType.Uniform:
                float sqrRt = Mathf.Sqrt(transform.childCount);
                Rows = Mathf.CeilToInt(sqrRt);
                Columns = Mathf.CeilToInt(sqrRt);
                break;
        }

        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        float cellWidth = parentWidth / (float)Columns - ((Spacing.x / (float)Columns) * 2) - 
            (padding.left / (float)Columns) - 
            (padding.right / (float)Columns);

        float cellHeight = parentHeight / (float)Rows - ((Spacing.y / (float)Rows) * 2) - 
            (padding.top / (float)Rows) - 
            (padding.top / (float)Rows);

        CellSize.x = FitX ? cellWidth : CellSize.x;
        CellSize.y = FitY ? cellHeight : CellSize.y;

        int columnCount = 0;
        int rowCount = 0;

        for (int i = 0; i < rectChildren.Count; i++)
        {
            rowCount = i / Columns;
            columnCount = i % Columns;

            var item = rectChildren[i];

            var xPos = (CellSize.x * columnCount) + (Spacing.x * columnCount);
            var yPos = (CellSize.y * rowCount) + (Spacing.y * rowCount);

            SetChildAlongAxis(item, 0, xPos, CellSize.x);
            SetChildAlongAxis(item, 1, yPos, CellSize.y);
        }
    }

    public override void SetLayoutHorizontal()
    {
    }

    public override void SetLayoutVertical()
    {
    }
}
