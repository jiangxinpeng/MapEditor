using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ArrowLegend.MapEditor;
using LitJson;
using System.IO;
using UnityEditor.Presets;

public class MapGeneratorEditor : EditorWindow
{
    //private MapEditorData data;
    private MapEditorLevelHandle levelHandle;
    private MapEditorSizeAndTextureHandle sizeHandle;
    private MapEditorBuildHandle buildHandle;
    private MapEditorEscort escortHandle;
    private MapEditorEnemyHandle enemyHandle;
    private MapEditorWeatherHandle weatherHandle;

    [MenuItem("地图编辑器/编辑地图")]
    private static void Open()
    {
        MapGeneratorEditor window = GetWindowWithRect<MapGeneratorEditor>(new Rect(0, 0, 800, 800), true, "地图编辑器", false);
    }

    private void OnEnable()
    {
        GlobalHandle.levelInfo = null;

        levelHandle = new MapEditorLevelHandle(this);
        sizeHandle = new MapEditorSizeAndTextureHandle();
        buildHandle = new MapEditorBuildHandle();
        enemyHandle = new MapEditorEnemyHandle();
        escortHandle = new MapEditorEscort();
        weatherHandle = new MapEditorWeatherHandle();

        levelHandle.Init();  //这个初始化一定是排第一位的
    }

    //关闭时保存配置信息
    private void OnDestroy()
    {
        Save(); 
    }

    public void Save()
    {
        if (GlobalHandle.levelInfo == null)
        {
            Debug.Log("没有需要保存的数据");
            return;
        }
        weatherHandle.Destory();
        enemyHandle.Destory();
        buildHandle.Destory();
        sizeHandle.Destory();
        levelHandle.Destory();  //最后释放

        GlobalHandle.levelInfo = null;
        AssetDatabase.Refresh();
    }

   

    //初始化数据  调用的入口在levelInfo初始化的地方
    public void InitData()
    {
        sizeHandle.Init();
        buildHandle.Init();
        //escortHandle.Init();
        enemyHandle.Init();
        weatherHandle.Init();
    }

    private void OnGUI()
    {
        LevelConfigure();

        EditorGUILayout.Space();
        MapGroundConfigure();
        EditorGUILayout.Space();
        BuildConfigure();
        GUILayout.Space(20);
        //PathConfigure();
        GUILayout.Space(20);
        EnemyConfigure();
        GUILayout.Space(20);
        WeatherConfigure();

    }

    private void OnHierarchyChange()
    {
        
    }

    /// <summary>
    /// 关卡等级配置
    /// </summary>
    private void LevelConfigure()
    {
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("输入要编辑的关卡", GUILayout.Width(100));
        levelHandle.inputLevel = EditorGUILayout.IntField(levelHandle.inputLevel, GUILayout.Width(50));
        if (GUILayout.Button("确定", GUILayout.Width(100)))
        {
            levelHandle.Confirm();
        }

        EditorGUILayout.LabelField($"最高的关卡:  {levelHandle.topLevel}", GUILayout.Width(150));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"当前关卡:  {levelHandle.currLevel}", GUILayout.Width(150));
        if (GUILayout.Button("添加新关卡", GUILayout.Width(100)))
        {
            levelHandle.CreateNewLevel();
            AssetDatabase.Refresh();
        }
        GUILayout.EndHorizontal();
    }

    /// <summary>
    /// 地图配置
    /// </summary>
    private void MapGroundConfigure()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(5);
        if (GUILayout.Button("产生地图", GUILayout.Width(100)))
        {
            if (!IsCanClick()) return;

            if (!sizeHandle.IsEligible())
            {
                GlobalHandle.Tip("地图的大小不能为0");
            }
            else if (!sizeHandle.IsExitLevel())
            {
                GlobalHandle.Tip("没有创建相应的关卡");
            }
            else
            {
                sizeHandle.ProductGround();
            }
        }
        sizeHandle.MapSize = EditorGUILayout.Vector2IntField("设置地图大小", sizeHandle.MapSize, GUILayout.Width(150));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        MapTextureConfigure();

    }

    /// <summary>
    /// 选择地表纹理配置
    /// </summary>
    private void MapTextureConfigure()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("选择地图材质", GUILayout.Width(100)))
        {
            if (!IsCanClick()) return;

            if (sizeHandle.OpenMaterial() != null)
            {
                EditorGUIUtility.PingObject(sizeHandle.OpenMaterial());
            }
        }
        EditorGUI.BeginChangeCheck();
        {
            sizeHandle.GroundMaterial = EditorGUILayout.ObjectField(sizeHandle.GroundMaterial, typeof(Material), false, GUILayout.Width(200)) as Material;
        }
        if (EditorGUI.EndChangeCheck())
        {
            sizeHandle.SetGroundMaterial();
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// 地面建筑配置类
    /// </summary>
    private void BuildConfigure()
    {
        EditorGUILayout.LabelField("地面建筑");
        EditorGUI.BeginChangeCheck();
        {
            buildHandle.BuildBigType = GUILayout.Toolbar(buildHandle.BuildBigType, buildHandle.BuildBigList, GUILayout.Width(500));
        }
        if (EditorGUI.EndChangeCheck())
        {
            if (!IsCanClick())
            {
                buildHandle.BuildBigType = 0;
                return;
            }

            Debug.Log("当前选择的toolbar" + buildHandle.BuildBigType);
            buildHandle.ChangeToolBar();
        }
        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("选择类型", GUILayout.Width(100));
        EditorGUI.BeginChangeCheck();
        {
            buildHandle.BuildSmallType = EditorGUILayout.Popup(buildHandle.BuildSmallType, buildHandle.BuildSmallList, GUILayout.Width(150));
        }
        if (EditorGUI.EndChangeCheck())
        {
            if (!IsCanClick())
            {
                buildHandle.BuildSmallType = 0;
                return;
            }
        }

        if (GUILayout.Button("设为笔刷", GUILayout.Width(120)))
        {
            buildHandle.SetTemplate();
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// 怪物配置类
    /// </summary>
    private void EnemyConfigure()
    {
        EditorGUILayout.LabelField("怪物设置", GUILayout.Width(100));
        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        {
            enemyHandle.EnemyTime = EditorGUILayout.Popup(enemyHandle.EnemyTime, enemyHandle.TimesList.ToArray(), GUILayout.Width(150));
        }
        if (EditorGUI.EndChangeCheck())
        {
            enemyHandle.ChangeTimes();
        }

        if (GUILayout.Button("添加新的一波怪物配置", GUILayout.Width(150)))
        {
            if (!IsCanClick())
            {
                return;
            }
            enemyHandle.CreateNewTime();
        }
        EditorGUILayout.EndHorizontal();

        enemyHandle.ProductTypeIndex = (TimesEnemyProduceType)EditorGUILayout.EnumPopup("产生时机", enemyHandle.ProductTypeIndex, GUILayout.Width(300));

        enemyHandle.productTime = EditorGUILayout.DoubleField("距离上一波的时间", enemyHandle.productTime, GUILayout.Width(300));

        EditorGUI.BeginChangeCheck();
        {
            enemyHandle.EnemyBigType = GUILayout.Toolbar(enemyHandle.EnemyBigType, enemyHandle.EnemyBigList, GUILayout.Width(500));
        }
        if (EditorGUI.EndChangeCheck())
        {
            if (!IsCanClick())
            {
                enemyHandle.EnemyBigType = 0;
                return;
            }
            enemyHandle.ChangeToolBar();
          
        }
        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("选择类型", GUILayout.Width(100));

        EditorGUI.BeginChangeCheck();
        {
            enemyHandle.EnemySmallType = EditorGUILayout.Popup(enemyHandle.EnemySmallType, enemyHandle.EnemySmallList, GUILayout.Width(150));
        }
        if (EditorGUI.EndChangeCheck())
        {
            if (!IsCanClick())
            {
                enemyHandle.EnemySmallType = 0;
                return;
            }
        }

        if (GUILayout.Button("在文件夹中查看", GUILayout.Width(150)))
        {

        }

        if (GUILayout.Button("设为笔刷", GUILayout.Width(120)))
        {
            enemyHandle.SetTemplate();
        }
        EditorGUILayout.EndHorizontal();

    }

    /// <summary>
    /// 天气配置
    /// </summary>
    private void WeatherConfigure()
    {
        //EditorGUILayout.LabelField("天气设置");

        //EditorGUI.BeginChangeCheck();
        //{
        //    weatherHandle.isDay = EditorGUILayout.ToggleLeft("白天或者黑夜(勾选为白天)", weatherHandle.isDay, GUILayout.Width(200));
        //}
        //if (EditorGUI.EndChangeCheck())
        //{
        //    weatherHandle.SaveDay();
        //}
        //EditorGUI.BeginChangeCheck();
        //{
        //    weatherHandle.weatherType = (WeatherType)EditorGUILayout.EnumPopup("天气选项", weatherHandle.weatherType, GUILayout.Width(300));
        //}
        //if (EditorGUI.EndChangeCheck())
        //{
        //    weatherHandle.SaveWeather();
        //}
    }

    /// <summary>
    /// 当前关卡为-1的时候，下面的按钮的点不了的
    /// </summary>
    private bool IsCanClick()
    {
        if (levelHandle.currLevel<=0)
        {
            GlobalHandle.Tip("请选择或者创建关卡");
            return false;
        }
        return true;
    }


}


