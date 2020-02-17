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
    /// LevelInfo被赋值的时机
    /// </summary>
    public enum OpportUnity
    {
        Chose,     //在编辑某关卡时选择一个新的关卡
        FirstChose,  //第一次选择关卡
        Create,    //在编辑某关卡时创建一个新的关卡
        FirstCreate,  //第一次创建新的关卡
    }
    /// <summary>
    /// 地图编辑器的关卡数据和逻辑类
    /// </summary>
    class MapEditorLevelHandle:BaseHandle,IHandle
    {
        private MapGeneratorEditor editor;

        private static string levelConfigurePath = "Assets/LevelConfigure";  //关卡配置文件的路径
        //public int LevelIndex = 0;    //关卡下拉列表的索引   从0开始
        //public List<string> levelNameList;   //关卡列表

        public int inputLevel;     //输入的关卡Id   从1开始
        public int currLevel = -1;     //当前关卡   从1开始
        public int topLevel = 0;      //最高关卡   从1开始

        public MapEditorLevelHandle(MapGeneratorEditor editor)
        {
            this.editor = editor;
        }

        public void Init()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(levelConfigurePath);
            topLevel = directoryInfo.GetFiles("*.txt").Length;
            Debug.Log("当前最高关卡" + topLevel);
            //if (topLevel>=1)
            //{
            //    currLevel = 1;
            //}
            //MapGeneratorEditor.levelInfo = ReadConfigureInfo(currLevel);
        }

        public void Destory()
        {
            Save(GlobalHandle.levelInfo.levelId);
        }

        /// <summary>
        /// 关卡是否有效
        /// </summary>
        public void Confirm()
        {
            if (inputLevel > topLevel||inputLevel<=0)  //输入的关卡Id超出范围
            {
                GlobalHandle.Tip("暂无该关卡的信息");
            }
            else
            {
                currLevel = inputLevel;
                Debug.Log("确定之前的关卡的信息" + JsonUtility.ToJson(GlobalHandle.levelInfo));

                if (GlobalHandle.levelInfo==null)
                {
                    ReadConfigureInfo(currLevel,OpportUnity.FirstChose);
                }
                else
                {
                    ReadConfigureInfo(currLevel, OpportUnity.Chose);
                }
            }
        }

        /// <summary>
        /// 初始化关卡列表
        /// </summary>
        //public void InitLevelNameList()
        //{
        //    //levelNameList = new List<string>();
        //    //for (int i = 1; i <= topLevel; i++)
        //    //{
        //    //    levelNameList.Add("Level_" + i);
        //    //}
        //}

        /// <summary>
        /// 读取配置文本，给levelInfo赋值
        /// </summary>
        /// <param name="path"></param>
        public DRLevel ReadConfigureInfo(int level,OpportUnity opportUnity)
        {
            //之前的内容要保存
            if (opportUnity==OpportUnity.Create||opportUnity==OpportUnity.Chose)
            {
                editor.Save();
            }

            DRLevel levelInfo = null;
            if (!File.Exists(GetFileName(level)))
            {
                Debug.Log("不存在关卡_" + level + "的场景配置文件,属于新的关卡");
                levelInfo = new DRLevel();
                levelInfo.levelId = currLevel;
            }
            else
            {
                Debug.Log("读取关卡_" + level + "配置文件成功");
                string str = File.ReadAllText(GetFileName(level), System.Text.Encoding.UTF8);
                levelInfo = JsonMapper.ToObject<DRLevel>(str);
            }
            GlobalHandle.levelInfo = levelInfo;
            editor.InitData();
            return levelInfo;
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
            GlobalHandle.levelInfo.levelId = level;
            string str = JsonMapper.ToJson(GlobalHandle.levelInfo);
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
        /// 创建新的关卡
        /// </summary>
        /// <param name="path"></param>
        public void CreateNewLevel()
        {
            //LevelIndex = topLevel;
            topLevel++;
            if (currLevel != -1)  //编辑某关卡的时候新建关卡
            {
                ChangeLevel(topLevel);
            }
            else   //直接新建关卡的时候
            {
                currLevel = topLevel;
            }
            Debug.Log("新建时的之前的关卡的信息"+JsonUtility.ToJson(GlobalHandle.levelInfo));
            if (GlobalHandle.levelInfo == null)
            {
                ReadConfigureInfo(currLevel, OpportUnity.FirstCreate);
            }
            else
            {
                ReadConfigureInfo(currLevel, OpportUnity.Create);
            }
        }

        /// <summary>
        /// 切换关卡
        /// </summary>
        /// <returns></returns>
        public void ChangeLevel(int level)
        {
            Save(currLevel);    //保存之前的关卡
            currLevel = level;  //赋值新的关卡
        }

        public void AfterChangeLevel()
        {
           
        }
    }
}
