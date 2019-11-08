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

        public string[] ToolbarStrings = new string[] { "边界墙", "草地", "树木", "石头", "路刺", "水面" };

        public static string[] BuildBigTypeFolderNameList = new string[] { "BoundWall", "Grass", "Tree", "Stone", "Stab", "Water" };
        private string buildBigTypeFolderName = "BoundWall";
        private int[] smallTypeSum = new int[] { 2, 2, 2, 2, 2, 2 };  //每一个大类型下的小类型个数

        private static string[] boundaryWallTypeArray = new string[] { "boundaryWall_1", "boundaryWall_2" };
        private static string[] grassTypeArray = new string[] { "grass_1", "grass_2" };
        private static string[] treeTypeArray = new string[] { "tree_1", "tree_2" };
        private static string[] stoneTypeArray = new string[] { "stone_1", "stone_2" };
        private static string[] stabTypeArray = new string[] { "stab_1", "stab_2" };
        private static string[] waterTypeArray = new string[] { "water_1", "water_2" };

        public int BuildBigType;      //大的类型
        public int BuildSmallType;    //小类型
        public string[] BuildSmallList = boundaryWallTypeArray;

        public int BuildIndex;
        public List<string> BuildList = new List<string>();

        public Vector3 BuildPos = Vector3.zero;
        public Vector3 BuildRot = Vector3.zero;
        public Vector3 BuildScal = Vector3.zero;

        public int BuildOldBigType;      //切换类型之前旧的大类型
        public int BuildOldSmallType;    //切换类型之前旧小类型
        public int BuildOldIndex;   //切换编号之后旧的编号

        public void Init()
        {
            BindBuildInfo();
            InitBuild();
        }

        public void Destory()
        {

        }

        public void ChangeLevel(int level)
        {
            BuildList.Clear();
            BuildBigType = 0;
            BuildSmallType = 0;
            ChangeToolBar();
        }

        public void AfterChangeLevel()
        {
            BindBuildInfo();

            InitBuild();
        }

        public void CreateNewLevel()
        {
            BindBuildInfo();
            ChangeLevel(0);  //0 没有什么意义
            //InitBuild();
        }

        /// <summary>
        /// 把buildInfo绑定给LevelInfo
        /// </summary>
        private void BindBuildInfo()
        {
            levelCorrespondBuildInfo = MapGeneratorEditor.levelInfo.BuildInfo;
            if (levelCorrespondBuildInfo == null)
            {
                levelCorrespondBuildInfo = new LevelCorrespondBuildInfo();
                for (int i = 0; i < ToolbarStrings.Length; i++)
                {
                    BigTypeEntityInfo bigTypeEntityInfo = new BigTypeEntityInfo();
                    List<SmallTypeEntityInfo> smallTypeEntityInfos = new List<SmallTypeEntityInfo>();
                    bigTypeEntityInfo.SmallTypeInfoList = smallTypeEntityInfos;
                    for (int j = 0; j < smallTypeSum[i]; j++)
                    {
                        smallTypeEntityInfos.Add(new SmallTypeEntityInfo());
                    }

                    levelCorrespondBuildInfo.BigTypeInfoList.Add(bigTypeEntityInfo);
                }

                MapGeneratorEditor.levelInfo.BuildInfo = levelCorrespondBuildInfo;
            }
        }

        /// <summary>
        /// 切换大类型
        /// </summary>
        public void ChangeToolBar()
        {
            switch (BuildBigType)
            {
                case 0:
                    BuildSmallList = boundaryWallTypeArray;
                    break;
                case 1:
                    BuildSmallList = grassTypeArray;
                    break;
                case 2:
                    BuildSmallList = treeTypeArray;
                    break;
                case 3:
                    BuildSmallList = stoneTypeArray;
                    break;
                case 4:
                    BuildSmallList = stabTypeArray;
                    break;
                case 5:
                    BuildSmallList = waterTypeArray;

                    break;
            }
            buildBigTypeFolderName = BuildBigTypeFolderNameList[BuildBigType];

            BuildSmallType = 0;
            ShowBuildName();
        }

        /// <summary>
        /// 初始化建筑
        /// </summary>
        private void InitBuild()
        {
            List<BigTypeEntityInfo> bigTypeEntityInfos = levelCorrespondBuildInfo.BigTypeInfoList;

            for (int i = 0; i < bigTypeEntityInfos.Count; i++)
            {
                List<SmallTypeEntityInfo> smallTypeEntityInfos = bigTypeEntityInfos[i].SmallTypeInfoList;   //拿到单个大类型下的小类型列表
                //InitBigTypeGameObject(i);

                for (int j = 0; j < smallTypeEntityInfos.Count; j++)
                {
                    List<TransformInfo> infoList = smallTypeEntityInfos[j].infoList;   //拿到单个小类型的所有物体列表
                    for (int m = 0; m < infoList.Count; m++)
                    {
                        BuildBigType = i;
                        ChangeToolBar();
                        InstantiateBuild(i, j, m, infoList[m]);
                    }
                }
            }

            BuildBigType = 0;
            ChangeToolBar();

            JudgeBuildInfo(GetCurrentBuildInfo(true), 0);
            //ShowBuildInfo(GetCurrentBuildInfo(true)[0]);
        }

        /// <summary>
        /// 添加建筑   编辑器界面加建筑
        /// </summary>
        public void AddBuild()
        {
            //获取这个小类型的建筑列表
            List<TransformInfo> infoList = GetCurrentBuildInfo(true);

            InstantiateBuild(BuildBigType, BuildSmallType, infoList.Count, new TransformInfo());
            BuildIndex = infoList.Count;
            infoList.Add(new TransformInfo());
        }

        /// <summary>
        /// 添加建筑   笔刷加建筑
        /// </summary>
        public void AddBuild(Vector3 pos)
        {
            //获取这个小类型的建筑列表
            List<TransformInfo> infoList = GetCurrentBuildInfo(true);
            infoList.Add(new TransformInfo());
            //infoList[infoList.Count-1].pos =new double[] { pos.x, pos.y, pos.z };
            infoList[infoList.Count - 1].pos[0] = pos.x;
            infoList[infoList.Count - 1].pos[1] = pos.y;
            infoList[infoList.Count - 1].pos[2] = pos.z;
            InstantiateBuild(BuildBigType, BuildSmallType, infoList.Count-1, infoList[infoList.Count-1]);
            BuildIndex = infoList.Count-1;

            JudgeEntityInfo(infoList, BuildIndex, ref BuildPos, ref BuildRot, ref BuildRot);
        }

        /// <summary>
        /// 实例化建筑出来
        /// </summary>
        /// <param name="j">小类型</param>
        /// <param name="m">编号</param>
        /// <param name="info">位置信息</param>
        private void InstantiateBuild(int i, int j, int m, TransformInfo info)
        {
            //实例化建筑
            string folderName = BuildBigTypeFolderNameList[i];
            string buildName = BuildSmallList[j];
            string assetName = $"Build/{folderName}/{buildName}";

            string fatherName = "ground_" + MapGeneratorEditor.levelInfo.levelId + "/Build/" + folderName;

            InstantiateEntity(assetName, fatherName, buildName, m, info);

            AddBuildList(buildName, m);
        }

        /// <summary>
        /// 获得建筑信息   true表示获取的最新的  false表示获取的切换之前的
        /// </summary>
        private List<TransformInfo> GetCurrentBuildInfo(bool isNew)
        {
            int bigType = isNew ? BuildBigType : BuildOldBigType;
            int smallType = isNew ? BuildSmallType : BuildOldSmallType;
            Debug.Log("建筑大编号：" + bigType + "  " + "小编号" + smallType);

            BigTypeEntityInfo bigTypeEntityInfo = levelCorrespondBuildInfo.BigTypeInfoList[bigType];
            SmallTypeEntityInfo smallTypeEntityInfo = bigTypeEntityInfo.SmallTypeInfoList[smallType];
            //获取这个小类型的建筑列表
            List<TransformInfo> infoList = smallTypeEntityInfo.infoList;
            return infoList;
        }

        /// <summary>
        /// 显示建筑信息  编辑器界面的显示
        /// </summary>
        public void ShowBuildName()
        {
            //先清空建筑列表
            BuildList.Clear();
            BuildIndex = 0;
            //获取这个小类型的建筑列表
            List<TransformInfo> infoList = GetCurrentBuildInfo(true);
            for (int i = 0; i < infoList.Count; i++)  //
            {
                AddBuildList(BuildSmallList[BuildSmallType], i);
            }

            JudgeBuildInfo(infoList, 0);
        }

        /// <summary>
        /// 展示建筑的位置信息
        /// </summary>
        private void ShowBuildInfo(TransformInfo info)
        {
            Debug.Log("位置信息" + JsonMapper.ToJson(info));
            BuildPos = new Vector3((float)info.pos[0], (float)info.pos[1], (float)info.pos[2]);
            BuildRot = new Vector3((float)info.rot[0], (float)info.rot[1], (float)info.rot[2]);
            BuildScal = new Vector3((float)info.scal[0], (float)info.scal[1], (float)info.scal[2]);
        }

        /// <summary>
        /// 保存建筑的信息  只要发生了切换  就要保存   time是切换的时机
        /// </summary>
        public void SaveBuildTransInfo(int index, int time)
        {
            Debug.Log("保存建筑位置信息编号" + index);
            //Debug.Log($"{BuildPos[0]},{BuildPos[1]},{BuildPos[2]}");
            List<TransformInfo> builds = GetCurrentBuildInfo(false);
            Debug.Log(JsonMapper.ToJson(builds));
            //如果要保存的建筑为空，则不储存
            if (builds.Count > index)
            {
                builds[index].pos = new double[] { BuildPos[0], BuildPos[1], BuildPos[2] };
                builds[index].rot = new double[] { BuildRot[0], BuildRot[1], BuildRot[2] };
                builds[index].scal = new double[] { BuildScal[0], BuildScal[1], BuildScal[2] };
            }

            switch (time)
            {
                case 1:   //切换新的编号
                    break;
                case 2:   //切换新的小类型
                    ShowBuildName();
                    break;
                case 3:   //切换新的大类型
                    ChangeToolBar();
                    break;
            }
            //获得的新的建筑的信息是空的话  位置是默认的
            List<TransformInfo> newBuild = GetCurrentBuildInfo(true);
            JudgeBuildInfo(newBuild, BuildIndex);
        }

        /// <summary>
        /// 填充编辑器数据
        /// </summary>
        private void AddBuildList(string buildName, int index)
        {
            //初始化编辑器数据
            BuildList.Add("类型 " + buildName + " 编号" + index);
        }

        /// <summary>
        /// 判断是否有建筑信息
        /// </summary>
        /// <returns></returns>
        private void JudgeBuildInfo(List<TransformInfo> infoList, int index)
        {
            if (infoList.Count == 0)  //没有建筑的时候  显示默认位置信息
            {
                ShowBuildInfo(new TransformInfo());
            }
            else
            {
                ShowBuildInfo(infoList[index]);
            }
        }

        /// <summary>
        /// 设置笔刷模板
        /// </summary>
        public void SetTemplate()
        {
            string folderName = BuildBigTypeFolderNameList[BuildBigType];
            string buildName = BuildSmallList[BuildSmallType];
            string assetName = $"Build/{folderName}/{buildName}";
            GameObject asset = Resources.Load(assetName) as GameObject;
            SetTemplate(asset,new ProductTemplateCallBack(AddBuild));
        }

        /// <summary>
        /// 编辑器界面的显示数据展示当前选中的物体的信息  暂时先放这  不做
        /// </summary>
        public void ShowSelectionInfo(GameObject go)
        {
            if (go.transform.parent.parent.name == "Build")  //选中的是建筑信息
            {
                for (int i = 0; i < BuildBigTypeFolderNameList.Length; i++)
                {
                    if (go.transform.parent.name == BuildBigTypeFolderNameList[i])
                    {
                        BuildBigType = i;    //大类型的编号
                        ChangeToolBar();
                        for (int j = 0; j < BuildSmallList.Length; j++)
                        {
                            if (go.name.StartsWith(BuildSmallList[j]))
                            {
                                BuildSmallType = j;    //小类型的编号
                                string[] name = go.name.Split('_');
                                BuildIndex = Convert.ToInt32(name[2]);   //建筑物的编号
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}
