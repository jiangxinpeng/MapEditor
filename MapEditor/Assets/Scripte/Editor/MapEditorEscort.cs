using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ArrowLegend.MapEditor
{
    /// <summary>
    /// 护送的车次的路线
    /// </summary>
    public class MapEditorEscort : BaseHandle/*, IHandle*/
    {
    //    private List<double[]> path = new List<double[]>();

    //    public int PointOldIndex;   //怪物巡逻点的旧编号
    //    public int PointIndex;    //怪物巡逻点编号
    //    public List<string> PointList = new List<string>();
    //    public Vector3 PointPos;   //巡逻点的坐标
    //    private GameObject point;    //选中的巡逻点的物体

    //    public void AfterChangeLevel()
    //    {
    //        InitInfo();
    //    }

    //    public void ChangeLevel(int level)
    //    {
    //        if (level != 0)  //不是从创建新的关卡过来的
    //        {
    //            //保存最后一个路径点的位置
    //            SavePointPos(PointIndex);
    //        }
    //    }

    //    public void CreateNewLevel()
    //    {
    //        InitInfo();
    //        ChangeLevel(0);   //待会check下这个问题
    //    }

    //    public void Destory()
    //    {
    //        //保存最后一个路径点的位置
    //        SavePointPos(PointIndex);
    //    }

    //    public void Init()
    //    {
    //        InitInfo();
    //    }

    //    private void InitInfo()
    //    {
    //        DRLevel level = MapGeneratorEditor.levelInfo;
    //        path = level.escortList;

    //        Debug.Log(path.Count);

    //        InitPointObject();
    //    }

    //    /// <summary>
    //    /// 初始化路径点的建筑
    //    /// </summary>
    //    public void InitPointObject()
    //    {
    //        InitPointPos(0);

    //        Transform father = GameObject.Find("Point").transform;
    //        Debug.Log("删除"+father.childCount);
    //        int count = father.childCount;
    //        for (int j=0;j< count; j++)
    //        {
    //            Debug.Log(j);
    //            Debug.Log(father.GetChild(j).gameObject);
    //            GameObject.DestroyImmediate(father.GetChild(j).gameObject);
    //            j--;
    //            count--;
    //        }
    //        Debug.Log("添加" + path.Count);

    //        GameObject asset = Resources.Load("path") as GameObject;
    //        for (int i=0;i<path.Count;i++)
    //        {
    //            Vector3 pos = new Vector3((float)path[i][0], (float)path[i][1], (float)path[i][2]);
    //            GameObject point = GameObject.Instantiate(asset, pos, Quaternion.identity);
    //            point.name = $"路径点_{i + 1}";
    //            point.transform.SetParent(father);
    //        }
    //    }

    //    /// <summary>
    //    /// 初始化路径点
    //    /// </summary>
    //    public void InitPointPos(int index)
    //    {
    //        PointIndex = index;
    //        PointList.Clear();

    //        int count = path.Count;
    //        if (count == 0)
    //        {
    //            PointPos = Vector3.zero;
    //            return;
    //        }
    //        double[] d = path[PointIndex];
    //        PointPos = new Vector3((float)d[0], (float)d[1], (float)d[2]);
    //        for (int i = 1; i <= count; i++)
    //        {
    //            PointList.Add("路径点_" + i);
    //        }
    //    }

    //    /// <summary>
    //    /// 保存路径点  index是路径点编号
    //    /// </summary>
    //    public void SavePointPos(int index)
    //    {
    //        point = null;
    //        int count = path.Count;
    //        if (count == 0)
    //        {
    //            return;
    //        }
    //        if (count > index)
    //        {
    //            path[index] = new double[] { Math.Round(PointPos[0], 2), Math.Round(PointPos[1], 2), Math.Round(PointPos[2], 2) };
    //        }
    //    }

    //    /// <summary>
    //    /// 设置巡逻点笔刷模板
    //    /// </summary>
    //    public void SetPointtTemplate()
    //    {
    //        GameObject asset = Resources.Load("path") as GameObject;
    //        SetTemplate(asset, new ProductTemplateCallBack(AddPointPos));
    //    }

    //    public void AddPointPos(Vector3 pos)
    //    {
    //        SavePointPos(PointIndex);

    //        PointPos = pos;
    //        path.Add(new double[] { pos.x, pos.y, pos.z });
    //        PointList.Add("路径点_" + path.Count);
    //        PointIndex = path.Count - 1;
    //        GameObject asset = Resources.Load("path") as GameObject;
    //        GameObject point = GameObject.Instantiate(asset, pos, Quaternion.identity);
    //        point.name = $"路径点_{path.Count}";
    //        Transform parent = GameObject.Find("Point").transform;
    //        point.transform.SetParent(parent);
    //    }

    //    public void DeletePoint()
    //    {
    //        if (PointList.Count == 0)
    //        {
    //            return;
    //        }

    //        Transform parent = GameObject.Find("Point").transform;
    //        GameObject.DestroyImmediate(parent.Find($"路径点_{PointIndex + 1}").gameObject);
    //        //全部重命名
    //        int index = 1;
    //        for (int i = 0; i < parent.childCount; i++)
    //        {
    //            Transform transforms = parent.GetChild(i);
    //            if (transforms.name.StartsWith("路径点"))
    //            {
    //                transforms.name = $"路径点_{index}";
    //                index++;
    //            }
    //        }

    //        PointList.RemoveAt(PointIndex);
    //        path.RemoveAt(PointIndex);
    //        PointIndex = 0;
    //        if (path.Count <= 0)
    //        {
    //            PointPos = new Vector3(0, 0, 0);
    //            return;
    //        }
    //        double[] d = path[PointIndex];
    //        PointPos = new Vector3((float)d[0], (float)d[1], (float)d[2]);
    //    }

    //    /// <summary>
    //    /// 显示选中的巡逻点
    //    /// </summary>
    //    public void ShowSelectPoint(GameObject go)
    //    {
    //        if (go?.transform?.parent?.name == "Point")
    //        {
    //            point = go;
    //            string[] str = point.name.Split('_');
    //            PointIndex = Convert.ToInt32(str[1]) - 1;
    //            InitPointPos(PointIndex);
    //        }
    //    }

    //    public void RepaintCurrentPoint()
    //    {
    //        if (point == null)
    //        {
    //            return;
    //        }

    //        PointPos = point.transform.position;
    //    }
    }
}
