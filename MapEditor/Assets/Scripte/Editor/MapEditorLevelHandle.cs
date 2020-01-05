using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using LitJson;

namespace ArrowLegend.MapEditor
{
    /// <summary>
    /// 地图编辑器的关卡数据和逻辑类
    /// </summary>
    class MapEditorLevelHandle:BaseHandle,IHandle
    {
        private static string levelConfigurePath = "Assets/LevelConfigure";  //关卡配置文件的路径
        private int currLevel = 0;     //当前关卡   从1开始
        public int topLevel = 0;      //最高关卡   从1开始

        public int LevelIndex = 0;    //关卡下拉列表的索引   从0开始
        public List<string> levelNameList;   //关卡列表

        public void Init()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(levelConfigurePath);
            topLevel = directoryInfo.GetFiles("*.txt").Length;
            Debug.Log("当前最高关卡" + topLevel);
            if (topLevel>=1)
            {
                currLevel = 1;
            }
            MapGeneratorEditor.levelInfo = ReadConfigureInfo(currLevel);
        }

        public void Destory()
        {
            Save(currLevel);
        }

        /// <summary>
        /// 初始化关卡列表
        /// </summary>
        public void InitLevelNameList()
        {
            levelNameList = new List<string>();
            for (int i = 1; i <= topLevel; i++)
            {
                levelNameList.Add("Level_" + i);
            }
        }

        /// <summary>
        /// 读取配置文本，给levelInfo赋值
        /// </summary>
        /// <param name="path"></param>
        public DRLevel ReadConfigureInfo(int level)
        {
            if (!File.Exists(GetFileName(level)))
            {
                Debug.LogError("不存在关卡_" + level + "的场景配置文件");
                return new DRLevel();
            }
            else
            {
                Debug.Log("读取关卡_" + level + "配置文件成功");
                string str = File.ReadAllText(GetFileName(level), System.Text.Encoding.UTF8);
                DRLevel levelInfo = JsonMapper.ToObject<DRLevel>(str);
                return levelInfo;
            }
        }

        /// <summary>
        ///保存配置文本
        /// </summary>
        public void Save(int level)
        {
            if (currLevel==0)
            {
                Debug.Log("没有配置关卡");
                return;
            }
            Debug.Log("保存关卡_" + level+ "场景成功");
            if (File.Exists(GetFileName(level)))
            {
                File.Delete(GetFileName(level));
            }
            MapGeneratorEditor.levelInfo.levelId = level;
            string str = JsonMapper.ToJson(MapGeneratorEditor.levelInfo);
            File.AppendAllText(GetFileName(level), str, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// 得到配置文本路径
        /// </summary>
        /// <returns></returns>
        public string GetFileName(int level)
        {
            string fileName = "Level_" + level + ".txt";
            string outPutFileName = Path.Combine(levelConfigurePath, fileName);
            return outPutFileName;
        }

        /// <summary>
        /// 新建新的配置文本
        /// </summary>
        /// <param name="path"></param>
        public void CreateNewLevel()
        {
            LevelIndex = topLevel;

            topLevel++;
            if (currLevel != 0)
            {
                ChangeLevel(topLevel);
            }
            else   //一关都没有的时候
            {
                currLevel = topLevel;
            }

            MapGeneratorEditor.levelInfo.levelId = currLevel;
            levelNameList.Add("Level_" + topLevel);
        }

        /// <summary>
        /// 切换关卡
        /// </summary>
        /// <returns></returns>
        public void ChangeLevel(int level)
        {
            Save(currLevel);    //保存之前的关卡

            currLevel = level;  //赋值新的关卡
            MapGeneratorEditor.levelInfo = ReadConfigureInfo(level);
        }

        public void AfterChangeLevel()
        {
           
        }
    }
}
