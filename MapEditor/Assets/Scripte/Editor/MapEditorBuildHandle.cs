using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using LitJson;

namespace ArrowLegend.MapEditor
{
    /// <summary>
    /// 地图编辑器的建筑物数据和逻辑类
    /// </summary>
    class MapEditorBuildHandle : BaseHandle, IHandle
    {
        private LevelCorrespondBuildInfo levelCorrespondBuildInfo;

        public int BuildBigType;      //大的类型
        public int BuildSmallType;    //小类型
        public string[] BuildBigList;
        public string[] BuildSmallList;

        List<string> keys;
        List<string[]> valus;


        public MapEditorBuildHandle()
        {
            keys = new List<string>(GlobalHandle.BuildBigTypeNameList.Keys);
            BuildBigList = keys.ToArray();

            valus = new List<string[]>(GlobalHandle.BuildBigTypeNameList.Values);
            BuildSmallList = valus[0];
        }

        public void Init()
        {
            BuildBigType = 0;
            BuildSmallType = 0;
            BuildSmallList = valus[BuildBigType];

            BindBuildInfo();
            InitBuild();
        }

        public void Destory()
        {
            SaveInfo();
        }

        /// <summary>
        /// 切换大类型
        /// </summary>
        public void ChangeToolBar()
        {
           // List<string[]> valus = new List<string[]>(GlobalHandle.BuildBigTypeNameList.Values);
            BuildSmallList = valus[BuildBigType];
            BuildSmallType = 0;
        }

        /// <summary>
        /// 把buildInfo绑定给LevelInfo
        /// </summary>
        private void BindBuildInfo()
        {
           //List<string[]> valus = new List<string[]>(GlobalHandle.BuildBigTypeNameList.Values);
            levelCorrespondBuildInfo = GlobalHandle.levelInfo.BuildInfo;
            if (levelCorrespondBuildInfo.BigTypeInfoList.Count==0)
            {
                for (int i = 0; i < BuildBigList.Length; i++)
                {
                    BigTypeEntityInfo bigTypeEntityInfo = new BigTypeEntityInfo();
                    List<SmallTypeEntityInfo> smallTypeEntityInfos = new List<SmallTypeEntityInfo>();
                    bigTypeEntityInfo.SmallTypeInfoList = smallTypeEntityInfos;

                    for (int j = 0; j < valus[i].Length; j++)
                    {
                        SmallTypeEntityInfo smallTypeEntityInfo = new SmallTypeEntityInfo();
                        smallTypeEntityInfo.Id = 100 * (i + 1) + (j + 1);
                        smallTypeEntityInfos.Add(smallTypeEntityInfo);
                    }
                    levelCorrespondBuildInfo.BigTypeInfoList.Add(bigTypeEntityInfo);
                }
            }
        }

        /// <summary>
        /// 初始化建筑
        /// </summary>
        private void InitBuild()
        {
            InitBigTypeGameObject();
            CreateInstance();
        }

        /// <summary>
        /// 初始化建筑大型类别在project视图上
        /// </summary>
        private void InitBigTypeGameObject()
        {
            GameObject build = new GameObject("Build");
            build.transform.SetParent(GameObject.Find("Level_" + GlobalHandle.levelInfo.levelId).transform);
            build.transform.localPosition = Vector3.zero;

            int count = GlobalHandle.BuildBigTypeNameList.Count;
            //List<string> keys = new List<string>(GlobalHandle.BuildBigTypeNameList.Keys);

            for (int i = 0; i < count; i++)
            {
                //挂载大类型
                GameObject big = new GameObject(keys[i]);
                big.transform.SetParent(GameObject.Find($"Level_{GlobalHandle.levelInfo.levelId}/Build").transform);
                //挂载小类型
                //List<string[]> valus = new List<string[]>(GlobalHandle.BuildBigTypeNameList.Values);
                for (int j = 0; j < valus[i].Length; j++)
                {
                    GameObject small = new GameObject(valus[i][j]);
                    small.transform.SetParent(GameObject.Find($"Level_{GlobalHandle.levelInfo.levelId}/Build/{big.name}").transform);
                }
            }
        }

        /// <summary>
        /// 创建建筑的实例
        /// </summary>
        private void CreateInstance()
        {
            List<BigTypeEntityInfo> bigTypeEntityInfos = levelCorrespondBuildInfo.BigTypeInfoList;
            for (int i = 0; i < bigTypeEntityInfos.Count; i++)
            {
                List<SmallTypeEntityInfo> smallTypeEntityInfos = bigTypeEntityInfos[i].SmallTypeInfoList;   //拿到单个大类型下的小类型列表
                for (int j = 0; j < smallTypeEntityInfos.Count; j++)
                {
                    //smallTypeEntityInfos[j].Id = 100 * (i + 1) + (j + 1);   用来标识实体的Id
                    List<TransformInfo> infoList = smallTypeEntityInfos[j].infoList;   //拿到单个小类型的所有物体列表
                    for (int m = 0; m < infoList.Count; m++)
                    {
                        InstantiateBuild(i, j, m, infoList[m]);
                    }
                }
            }
        }

        /// <summary>
        /// 实例化建筑出来
        /// </summary>
        /// <param name="i">大类型</param>
        /// <param name="j">小类型</param>
        /// <param name="m">编号</param>
        /// <param name="info">位置信息</param>
        private void InstantiateBuild(int i, int j, int m, TransformInfo info)
        {
            List<string> bigTypeName = new List<string>(GlobalHandle.BuildBigTypeNameList.Keys);
            List<string[]> smallTypeNameList = new List<string[]>(GlobalHandle.BuildBigTypeNameList.Values);

            //实例化建筑的资源的路径
            string bigName = bigTypeName[i];
            string smallName = smallTypeNameList[i][j];
            string assetName = $"Build/{bigName}/{smallName}";
            //Hierarchy界面的父节点的名字
            string fatherName = $"Level_{GlobalHandle.levelInfo.levelId}/Build/{bigName}/{smallName}";

            InstantiateEntity(assetName, fatherName, smallName, m, info);
        }

        /// <summary>
        /// 遍历Hierarchy面板  记录信息
        /// </summary>
        private void SaveInfo()
        {
            Transform build = GameObject.Find($"Level_{GlobalHandle.levelInfo.levelId}/Build").transform;
            int bigCount = build.childCount;
            for (int i = 0; i < bigCount; i++)
            {
                Transform big = build.GetChild(i);  //大类型
                int smallCount = big.childCount;
                for (int j = 0; j < smallCount; j++)
                {
                    Transform small = big.GetChild(j);   //小类型

                    int instanceCount = small.childCount;
                    List<TransformInfo> infoList = levelCorrespondBuildInfo.BigTypeInfoList[i].SmallTypeInfoList[j].infoList;
                    infoList.Clear();

                    for (int m = 0; m < instanceCount; m++)
                    {
                        Debug.Log($"保存建筑：大类型{i }小类型{j }实例序号{m}");
                        Transform instance = small.GetChild(m);
                        //if (m>= infoList.Count)
                        //{
                            infoList.Add(new TransformInfo());
                       // }
                        TransformInfo info = infoList[m];
                        info.pos = new double[] { Math.Round(instance.localPosition.x, 2), Math.Round(instance.localPosition.y, 2), Math.Round(instance.localPosition.z, 2) };
                        info.rot = new double[] { Math.Round(instance.localRotation.x, 2), Math.Round(instance.localRotation.y, 2), Math.Round(instance.localRotation.z, 2) };
                        info.scal = new double[] { Math.Round(instance.localScale.x, 2), Math.Round(instance.localScale.y, 2), Math.Round(instance.localScale.z, 2) };
                    }
                }
            }
        }


        /// <summary>
        /// 设置笔刷模板
        /// </summary>
        public void SetTemplate()
        {
            string folderName = BuildBigList[BuildBigType];
            string buildName = BuildSmallList[BuildSmallType];
            string assetName = $"Build/{folderName}/{buildName}";
            GameObject asset = Resources.Load(assetName) as GameObject;
            SetTemplate(asset, new ProductTemplateCallBack(AddBuild));
        }

        private void AddBuild(Vector3 vector3)
        {
            if (GameObject.Find("Map").transform.childCount==0)
            {
                return;
            }
            Transform parent = GameObject.Find($"Level_{GlobalHandle.levelInfo.levelId}/Build/{keys[BuildBigType]}/{valus[BuildBigType][BuildSmallType]}").transform;
            int count = parent.childCount;
            //List<TransformInfo> transformInfo = GetCurrentBuildInfo();
            //transformInfo.Add(new TransformInfo() { pos = new double[] { Math.Round(vector3.x, 2), Math.Round(vector3.y, 2), Math.Round(vector3.z, 2) } });
            TransformInfo info = new TransformInfo() { pos = new double[] { Math.Round(vector3.x, 2), Math.Round(vector3.y, 2), Math.Round(vector3.z, 2) } };
            InstantiateBuild(BuildBigType,BuildSmallType, count, info);
        }

        //private List<TransformInfo> GetCurrentBuildInfo()
        //{
        //    BigTypeEntityInfo bigTypeEntityInfo = levelCorrespondBuildInfo.BigTypeInfoList[BuildBigType];
        //    SmallTypeEntityInfo smallTypeEntityInfo = bigTypeEntityInfo.SmallTypeInfoList[BuildSmallType];
        //    //获取这个小类型的建筑列表
        //    List<TransformInfo> infoList = smallTypeEntityInfo.infoList;
        //    return infoList;
        //}
    }
}
