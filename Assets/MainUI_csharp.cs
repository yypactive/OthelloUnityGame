using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUI_csharp : MonoBehaviour
{
    public RectTransform DashBoardRectTran;
    public float DashBoardMinWidth;
    public float DashBoardMaxWidth;
    public float DashBoardMinHight;
    public RectTransform ChessBoardRectTran;

    // Start is called before the first frame update
    void Start()
    {
        this.Resize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Resize()
    {
        var rectTran = this.gameObject.GetComponent<RectTransform>();
        Debug.Log("Resize");
        if (DashBoardRectTran == null || DashBoardRectTran == null || rectTran == null)
            return;
        var height = rectTran.rect.height;
        var width = rectTran.rect.width;
        var whDelta = width - height;
        var dashWidth = 0f;
        var dashHight = 0f;
        if (whDelta > DashBoardMaxWidth)
        {
            dashWidth = DashBoardMaxWidth;
        }
        else if (whDelta > DashBoardMinWidth)
        {
            dashWidth = whDelta;
        }
        else if (whDelta > 0 && width > 2 * DashBoardMinWidth)
        {
            dashWidth = DashBoardMinWidth;
        }
        else
        {
            dashWidth = width / 3;
        }
        if (height > DashBoardMinHight)
        {
            dashHight = height;
        }
        else
        {
            dashHight = DashBoardMinHight;
        }
        Debug.Log("dashWidth: " + dashWidth);
        Debug.Log("dashHight: " + dashHight);
        DashBoardRectTran.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal, Mathf.Max(dashWidth, DashBoardMinWidth));
        DashBoardRectTran.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dashHight);
        var dashScale = Mathf.Min(Mathf.Min(1f, dashHight / DashBoardMinHight), Mathf.Min(1f, dashWidth / DashBoardMinWidth));
        Debug.Log("dashScale: " + dashScale);
        DashBoardRectTran.transform.localScale = new Vector2(dashScale, dashScale);
        ChessBoardRectTran.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width - Mathf.Max(dashWidth, DashBoardMinWidth) * dashScale);
    }
}
