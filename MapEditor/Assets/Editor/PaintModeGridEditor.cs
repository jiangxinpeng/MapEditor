using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 物体的类型
/// </summary>
public enum ObjectType
{
    Build,
    Enemy
}

public delegate void ProductTemplateCallBack(Vector3 position);

[CustomEditor(typeof(PaintModeGrid))]
public class PaintModeGridEditor :Editor
{
    public static GameObject template;      //笔刷模板
    public static ObjectType templateType;
    private static ProductTemplateCallBack ProductTemplateCallBack;
    Vector3 cursorPosition;      //鼠标的位置  实时刷新的
    Vector3 curSize = new Vector3(1,1,1);  //笔刷大小
    static Editor editor;

    private void OnSceneGUI()
    {
        var e = Event.current;
        UpdateCursorPosition();
        if (e.type==EventType.MouseDown&&e.button ==0)  //按下左键
        {
            ProductTemplate();
        }
        DrawCursor();

        Selection.activeGameObject = ((PaintModeGrid)target).gameObject;
        SceneView.RepaintAll();
    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (template != null)
        {
            if (editor == null)
            {
                editor = Editor.CreateEditor(template);
            }
            editor.DrawPreview(GUILayoutUtility.GetRect(500, 500));
            Repaint();
        }
    }

    /// <summary>
    /// 设置笔刷模板
    /// </summary>
    public static void SetTemplate(GameObject go, ProductTemplateCallBack callBack)
    {
        template = go;
        editor = null;
        ProductTemplateCallBack = callBack;
    }

    /// <summary>
    /// 产生模板
    /// </summary>
    private void ProductTemplate()
    {
        if (template==null)
        {
            MapGeneratorEditor.Tip("请设置笔刷！！");
            return;
        }
        ProductTemplateCallBack(cursorPosition);
        //GameObject go =Instantiate(template);
        //go.transform.localPosition = cursorPosition;
    }

    Vector3 SnapToGrid(Vector3 value)
    {
        var result = value;
      
        var gridSize = curSize;
        result.x = Mathf.FloorToInt(value.x / gridSize.x) * gridSize.x;
        result.y =0;
        result.z = Mathf.FloorToInt(value.z / gridSize.z) * gridSize.z;
        return result;
    }

    void UpdateCursorPosition()
    {
        float distance;
        var e = Event.current;
        Plane plane;
        var planePoint = new Vector3(0, 0, 0);
        plane = new Plane(Vector3.up, planePoint);
        //HandleUtility.GUI
        var ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        if (plane.Raycast(ray, out distance))
        {
            var hitpoint = ray.GetPoint(distance);
            hitpoint = SnapToGrid(hitpoint);
            cursorPosition = hitpoint;
        }
    }

    void DrawCursor()
    {
        var cursorSize =  curSize;
        DrawRect(cursorPosition, cursorSize, Color.red, 0.25f, 0.8f);
    }
 

    void DrawRect(Vector3 position, Vector3 size, Color color, float faceOpacity, float outlineOpacity)
    {
        var verts = new Vector3[] {   //四个顶点的位置
                position+new Vector3(-size.x/2,0,-size.z/2),
                position + new Vector3(size.x/2,0,-size.z/2),
                position + new Vector3(size.x/2,0,size.z/2),
                position + new Vector3(-size.x/2,0,size.z/2),
            };
        Color faceColor = new Color(color.r, color.g, color.b, faceOpacity);
        Color outlineColor = new Color(color.r, color.g, color.b, outlineOpacity);
        Handles.DrawSolidRectangleWithOutline(verts, faceColor, outlineColor);

    }
}
