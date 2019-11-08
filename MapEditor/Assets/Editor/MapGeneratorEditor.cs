﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ArrowLegend.MapEditor;
using LitJson;
using System.IO;
using UnityEditor.Presets;

public class MapGeneratorEditor : EditorWindow
{
    public static DRLevel levelInfo;

    private MapEditorData data;
    private MapEditorLevelHandle levelHandle;
    private MapEditorSizeAndTextureHandle sizeHandle;
    private MapEditorBuildHandle buildHandle;
    private MapEditorEnemyHandle enemyHandle;

    [MenuItem("地图编辑器/编辑地图")]
    private static void Open()
    {
        MapGeneratorEditor window = GetWindowWithRect<MapGeneratorEditor>(new Rect(0, 0, 800, 700), true, "地图编辑器", true);
    }

    private void OnEnable()
    {
        // data = new MapEditorData();   //编辑器界面的数据和逻辑
        levelHandle = new MapEditorLevelHandle();
        sizeHandle = new MapEditorSizeAndTextureHandle();
        buildHandle = new MapEditorBuildHandle();
        enemyHandle = new MapEditorEnemyHandle();
        //enemyHandle = new MapEditorHandle();  //地图配置的数据和逻辑(初始化等等)
        InitData();
        //handle.EnemyEnable();
    }

    //关闭时保存配置信息
    private void OnDestroy()
    {
        //data.Destory(enemyHandle.levelInfo);   //先注释掉数据保存
        enemyHandle.Destory();
        buildHandle.Destory();
        sizeHandle.Destory();
        levelHandle.Destory();  //最后释放
        AssetDatabase.Refresh();
    }

    //初始化数据
    private void InitData()
    {
        //data.InitData();
        levelHandle.Init();
        sizeHandle.Init();
        buildHandle.Init();
        enemyHandle.Init();
        //enemyHandle.InitData();
    }

    /// <summary>
    /// 切换关卡
    /// </summary>
    private void ChangeLevel(int level)
    {

        sizeHandle.ChangeLevel(level + 1);
        buildHandle.ChangeLevel(level + 1);
        enemyHandle.ChangeLevel(level + 1);

        levelHandle.ChangeLevel(level + 1);  //在此之后levelInfo刷新了 新的关卡的信息
        AfterChangeLevel();
    }

    /// <summary>
    /// 切换场景之后  读取了新的levelInfo
    /// </summary>
    private void AfterChangeLevel()
    {
        sizeHandle.AfterChangeLevel();
        buildHandle.AfterChangeLevel();
        enemyHandle.AfterChangeLevel();
    }

    /// <summary>
    /// 创建新关卡
    /// </summary>
    private void CreateNewLevel()
    {
        levelHandle.CreateNewLevel(); //要先初始化 设置levelInfo信息

        sizeHandle.CreateNewLevel();
        buildHandle.CreateNewLevel();
        enemyHandle.CreateNewLevel();
    }

    private void OnGUI()
    {
        LevelConfigure();
        EditorGUILayout.Space();
        MapGroundConfigure();
        EditorGUILayout.Space();
        BuildConfigure();
        GUILayout.Space(20);
        EnemyConfigure();
    }
    /// <summary>
    /// 关卡等级配置
    /// </summary>
    private void LevelConfigure()
    {
        GUILayout.BeginHorizontal();

        levelHandle.InitLevelNameList();

        EditorGUI.BeginChangeCheck();
        {
            levelHandle.LevelIndex = EditorGUILayout.Popup(levelHandle.LevelIndex, levelHandle.levelNameList.ToArray(), GUILayout.Width(80));
        }
        if (EditorGUI.EndChangeCheck())
        {
            Debug.Log("选择的序号是" + levelHandle.LevelIndex);
            ChangeLevel(levelHandle.LevelIndex);
        }
        if (GUILayout.Button("添加新关卡", GUILayout.Width(100)))
        {
            CreateNewLevel();
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
            if (!sizeHandle.IsEligible())
            {
                Tip("地图的大小不能为0");
            }
            else if (!sizeHandle.IsExitLevel())
            {
                Tip("没有创建相应的关卡");
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
        //GUILayout.Space(50);
        //EditorGUI.BeginChangeCheck();
        //{
        //    sizeHandle.mapTextureId = EditorGUILayout.IntField("贴图编号", sizeHandle.mapTextureId, GUILayout.Width(200));
        //}
        //if (EditorGUI.EndChangeCheck())
        //{
        //    Debug.Log("换了贴图编号，改变对应的texture" + sizeHandle.mapTextureId);
        //}
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// 地面建筑配置类
    /// </summary>
    private void BuildConfigure()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("地面建筑");
        EditorGUI.BeginChangeCheck();
        {
            buildHandle.BuildOldBigType = buildHandle.BuildBigType;
            buildHandle.BuildBigType = GUILayout.Toolbar(buildHandle.BuildBigType, buildHandle.ToolbarStrings, GUILayout.Width(500));
        }
        if (EditorGUI.EndChangeCheck())
        {
            Debug.Log("当前选择的toolbar" + buildHandle.BuildBigType);
            buildHandle.SaveBuildTransInfo(buildHandle.BuildIndex, 3);
            //buildHandle.ChangeToolBar();

        }
        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("选择类型", GUILayout.Width(100));
        EditorGUI.BeginChangeCheck();
        {
            buildHandle.BuildOldSmallType = buildHandle.BuildSmallType;
            buildHandle.BuildSmallType = EditorGUILayout.Popup(buildHandle.BuildSmallType, buildHandle.BuildSmallList, GUILayout.Width(150));
        }
        if (EditorGUI.EndChangeCheck())
        {
            buildHandle.SaveBuildTransInfo(buildHandle.BuildIndex, 2);
            //buildHandle.ShowBuildName();
        }

        if (GUILayout.Button("在文件夹中查看", GUILayout.Width(150)))
        {

        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("创建的建筑编号", GUILayout.Width(100));

        EditorGUI.BeginChangeCheck();
        {
            buildHandle.BuildOldIndex = buildHandle.BuildIndex;
            buildHandle.BuildIndex = EditorGUILayout.Popup(buildHandle.BuildIndex, buildHandle.BuildList.ToArray(), GUILayout.Width(150));
        }
        if (EditorGUI.EndChangeCheck())
        {
            //切换建筑物
            buildHandle.SaveBuildTransInfo(buildHandle.BuildOldIndex, 1);
        }

        if (GUILayout.Button("添加新的建筑", GUILayout.Width(150)))
        {
            buildHandle.SaveBuildTransInfo(buildHandle.BuildOldIndex, 1);
            buildHandle.AddBuild();
        }
        EditorGUILayout.EndHorizontal();

        buildHandle.BuildPos = EditorGUILayout.Vector3Field("位置", buildHandle.BuildPos, GUILayout.Width(300));
        buildHandle.BuildRot = EditorGUILayout.Vector3Field("旋转", buildHandle.BuildRot, GUILayout.Width(300));
        buildHandle.BuildScal = EditorGUILayout.Vector3Field("缩放", buildHandle.BuildScal, GUILayout.Width(300));

    }

    //private void OnSelectionChange()
    //{
    //    //实时刷新当前选择的位置
    //    if (Selection.activeGameObject != null)
    //    {
    //        Debug.LogError("当前选择的物体" + Selection.activeGameObject);
    //        buildHandle.ShowSelectionInfo(Selection.activeGameObject);
    //    }
    //}
    /// <summary>
    /// 怪物配置类
    /// </summary>
    private void EnemyConfigure()
    {
        EditorGUILayout.LabelField("怪物设置", GUILayout.Width(100));
        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        {
            enemyHandle.EnemyOldTime = enemyHandle.EnemyTime;
            enemyHandle.EnemyTime = EditorGUILayout.Popup(enemyHandle.EnemyTime, enemyHandle.timesList.ToArray(), GUILayout.Width(150));
        }
        if (EditorGUI.EndChangeCheck())
        {
            enemyHandle.SaveEnemyTransInfo(enemyHandle.EnemyIndex, 4);
        }

        if (GUILayout.Button("添加新的一波怪物配置", GUILayout.Width(150)))
        {
            enemyHandle.CreateNewTime();
        }
        EditorGUILayout.EndHorizontal();
        enemyHandle.ProductTypeIndex = (TimesEnemyProduceType)EditorGUILayout.EnumPopup("产生时机", enemyHandle.ProductTypeIndex, GUILayout.Width(300));

        EditorGUI.BeginChangeCheck();
        {
            enemyHandle.EnemyOldBigType = enemyHandle.EnemyBigType;
            enemyHandle.EnemyBigType = GUILayout.Toolbar(enemyHandle.EnemyBigType, enemyHandle.enemyToolBarString, GUILayout.Width(500));
        }
        if (EditorGUI.EndChangeCheck())
        {
            enemyHandle.SaveEnemyTransInfo(enemyHandle.EnemyIndex, 3);
        }
        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("选择类型", GUILayout.Width(100));

        EditorGUI.BeginChangeCheck();
        {
            enemyHandle.EnemyOldSmallType = enemyHandle.EnemySmallType;
            enemyHandle.EnemySmallType = EditorGUILayout.Popup(enemyHandle.EnemySmallType, enemyHandle.EnemySmallList, GUILayout.Width(150));
        }
        if (EditorGUI.EndChangeCheck())
        {
            enemyHandle.SaveEnemyTransInfo(enemyHandle.EnemyIndex, 2);
        }

        if (GUILayout.Button("在文件夹中查看", GUILayout.Width(150)))
        {

        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("创建的怪物编号", GUILayout.Width(100));

        EditorGUI.BeginChangeCheck();
        {
            enemyHandle.EnemyOldIndex = enemyHandle.EnemyIndex;
            enemyHandle.EnemyIndex = EditorGUILayout.Popup(enemyHandle.EnemyIndex, enemyHandle.EnemyList.ToArray(), GUILayout.Width(150));
        }
        if (EditorGUI.EndChangeCheck())
        {
            enemyHandle.SaveEnemyTransInfo(enemyHandle.EnemyOldIndex, 1);
        }

        if (GUILayout.Button("添加新的怪物", GUILayout.Width(150)))
        {
            enemyHandle.SaveEnemyTransInfo(enemyHandle.EnemyIndex, 1);
            enemyHandle.AddEnemy();
        }
        EditorGUILayout.EndHorizontal();

        enemyHandle.enemyPos = EditorGUILayout.Vector3Field("位置", enemyHandle.enemyPos, GUILayout.Width(300));
        enemyHandle.enemyRot = EditorGUILayout.Vector3Field("旋转", enemyHandle.enemyRot, GUILayout.Width(300));
        enemyHandle.enemyScal = EditorGUILayout.Vector3Field("缩放", enemyHandle.enemyScal, GUILayout.Width(300));
    }

    /// <summary>
    /// 玩家出入口设置
    /// </summary>
    private void InOutWardConfigure()
    {
        //EditorGUILayout.Space();
        //EditorGUILayout.LabelField("玩家出入口配置");

        //EditorGUILayout.BeginHorizontal();
        //EditorGUILayout.LabelField("选择出口类型", GUILayout.Width(100));

        //EditorGUI.BeginChangeCheck();
        //{
        //    handle.enemyTypeIndex = EditorGUILayout.Popup(handle.enemyTypeIndex, handle.enemyType, GUILayout.Width(150));
        //}
        //if (EditorGUI.EndChangeCheck())
        //{
        //    handle.enemyIndex = 0;
        //}

        //if (GUILayout.Button("在文件夹中查看", GUILayout.Width(150)))
        //{

        //}
        //EditorGUILayout.EndHorizontal();

        //EditorGUILayout.BeginHorizontal();
        //EditorGUILayout.LabelField("创建的怪物编号", GUILayout.Width(100));

        //handle.enemyIndex = EditorGUILayout.Popup(handle.enemyIndex, handle.enemy.ToArray(), GUILayout.Width(150));

        //if (GUILayout.Button("添加新的怪物", GUILayout.Width(150)))
        //{
        //    handle.AddEnemyTypeIndex(info.enemyInfoDic, handle.GetEnemyInfoIndex());
        //    handle.enemy.Add("类型" + handle.enemyType[handle.enemyTypeIndex] + "  " + "编号" + info.enemyInfoDic[handle.GetEnemyInfoIndex()].sum + "怪物");
        //}
        //EditorGUILayout.EndHorizontal();

        //List<TransformInfo> posInfo = handle.InitEnemyPos(info.enemyInfoDic[handle.GetEnemyInfoIndex()].infoList, handle.enemyIndex);
        //Debug.LogError("位置信息" + JsonMapper.ToJson(posInfo) + "编号" + handle.enemyIndex);
        //PosInfo(posInfo[handle.enemyIndex]);

    }

    /// <summary>
    /// 提示界面
    /// </summary>
    public static void Tip(string content)
    {
        EditorUtility.DisplayDialog("提示", content, "好的");
    }


}


