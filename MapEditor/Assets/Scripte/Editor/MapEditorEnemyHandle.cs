using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ArrowLegend.MapEditor
{
    /// <summary>
    /// 地图编辑的数据处理类
    /// </summary>
    public class MapEditorEnemyHandle : BaseHandle, IHandle
    {
        private LevelCorrespondEnemy levelCorrespondEnemyInfo;

        //怪物的设置参数
        public int EnemyTime;          //怪物当前波次
        public TimesEnemyProduceType ProductTypeIndex = TimesEnemyProduceType.Defalut;          //产生时机
        public List<string> timesList = new List<string>();   //波次列表

        public string[] enemyToolBarString = new string[] { "陆地小怪", "陷阱小怪", "飞行小怪", "小头目", "大头目" };
        public static string[] landTypeArray = new string[] { "野狗", "爬虫", "口水兵" };
        public static string[] trapTypeArray = new string[] { "吸血蜘蛛", "吞噬怪", "粘液怪" };
        public static string[] flyTypeArray = new string[] { "毒蜂", "毒蝶", "蜈蚣" };
        public static string[] smallBossTypeArray = new string[] { "独角兽", "巨胖尸怪" };
        public static string[] bigBossTypeArray = new string[] { "黑寡妇蜘蛛" };

        public static string[] EnemyBigTypeFolderNameList = new string[] { "Land", "Trap", "Fly", "SmallBoss", "BigBoss" };

        public static string[] landAssetNameArray = new string[] { "dog", "crawler", "moutSailor" };
        public static string[] trapAssetNameArray = new string[] { "spider", "eatStrange", "mucusStrange" };
        public static string[] flyAssetNameArray = new string[] { "PoisonBee", "PoisonControl", "Centipede" };
        public static string[] smallBossAssetNameArray = new string[] { "Unicorn", "fatBody" };
        public static string[] bigBossAssetNameArray = new string[] { "spiderBoss" };

        private string enemyBigTypeFolderName = "Land";     //大的类型的文件夹名  用来加载资源
        private int[] smallTypeSum = new int[] { 3, 3, 3, 2, 2 };  //每一个大类型下的小类型个数

        public int EnemyBigType;   //大的类型 
        public int EnemySmallType;       //小的类型
        public string[] EnemySmallList = landTypeArray;
        public string[] EnemySmallAssetNameList = landAssetNameArray;

        public int EnemyIndex;    //怪物实例编号
        public List<string> EnemyList = new List<string>();

        public Vector3 enemyPos = Vector3.zero;
        public Vector3 enemyRot = Vector3.zero;
        public Vector3 enemyScal = Vector3.zero;

        public int EnemyOldTime;            //切换波次之前的旧波次
        public int EnemyOldBigType;         //切换之前旧的大类型 
        public int EnemyOldSmallType;       //切换之前旧的小类型
        public int EnemyOldIndex;          //切换编号之前旧的编号

        public void Init()
        {
            if (MapGeneratorEditor.levelInfo.enemyTimesInfo != null)
            {
                CreateLevelParent();
                InitBigTypeGameObject();
            }
            BindBuildInfo();
            InitEnemy(0);
        }

        public void Destory()
        {
            SaveEnemyTransInfo(EnemyIndex,3);
        }
        /// <summary>
        /// 创建每一关怪物的挂在父GameObject
        /// </summary>
        private void CreateLevelParent()
        {
            if (GameObject.Find("Level_" + GetLevelID()))
            {
                return;
            }
            GameObject go = new GameObject("Level_" + GetLevelID());
            go.transform.SetParent(GameObject.Find("Enemy").transform);
        }

        private int GetLevelID()
        {
            int id = MapGeneratorEditor.levelInfo.levelId;
            return id;
        }

        public void ChangeLevel(int level)
        {
            EnemyTime = 0;
            ChangeTimes();
        }

        public void AfterChangeLevel()
        {
            BindBuildInfo();

            CreateLevelParent();
            InitBigTypeGameObject();
            InitEnemy(EnemyTime);
        }

        public void CreateNewLevel()
        {
            BindBuildInfo();
            CreateLevelParent();
            InitBigTypeGameObject();
            ChangeLevel(0);  //0 没有什么意义
        }

        /// <summary>
        /// 添加新的波次
        /// </summary>
        public void CreateNewTime()
        {
            levelCorrespondEnemyInfo.timesEnemyList.Add(new TimesCorrespondEnemy());
            EnemyTime = timesList.Count;
            BindBuildInfo();
            InitBigTypeGameObject();
            ChangeTimes();
        }

        /// <summary>
        /// 把enemyInfo绑定给LevelInfo
        /// </summary>
        private void BindBuildInfo()
        {
            levelCorrespondEnemyInfo = MapGeneratorEditor.levelInfo.enemyTimesInfo;
            if (levelCorrespondEnemyInfo == null)
            {
                levelCorrespondEnemyInfo = new LevelCorrespondEnemy();
                MapGeneratorEditor.levelInfo.enemyTimesInfo = levelCorrespondEnemyInfo;
            }
            InitTimeList();

            if (levelCorrespondEnemyInfo.timesEnemyList.Count == 0)
            {
                levelCorrespondEnemyInfo.timesEnemyList.Add(new TimesCorrespondEnemy());
                AddTimeList();
            }

            for (int m=0;m< levelCorrespondEnemyInfo.timesEnemyList.Count;m++)
            {
                if (levelCorrespondEnemyInfo.timesEnemyList[m].BigTypeInfoList.Count==0)
                {
                    for (int i = 0; i < enemyToolBarString.Length; i++)
                    {
                        BigTypeEntityInfo bigTypeEntityInfo = new BigTypeEntityInfo();
                        List<SmallTypeEntityInfo> smallTypeEntityInfos = new List<SmallTypeEntityInfo>();
                        bigTypeEntityInfo.SmallTypeInfoList = smallTypeEntityInfos;
                        for (int j = 0; j < smallTypeSum[i]; j++)
                        {
                            SmallTypeEntityInfo smallTypeEntityInfo = new SmallTypeEntityInfo();
                            smallTypeEntityInfo.Id = 100 * (i + 1) + (j + 1);
                            smallTypeEntityInfos.Add(smallTypeEntityInfo);
                        }

                        levelCorrespondEnemyInfo.timesEnemyList[m].BigTypeInfoList.Add(bigTypeEntityInfo);
                    }
                }
            }

        }

        /// <summary>
        /// 添加编辑器上的波次显示数据
        /// </summary>
        private void AddTimeList()
        {
            Debug.Log("添加");
            timesList.Add($"第{timesList.Count + 1}波怪物配置");
        }

        /// <summary>
        /// 初始化编辑器上的波次信息   第一次进来的时候调用  切换关卡的时候调用
        /// </summary>
        private void InitTimeList()
        {
            timesList.Clear();
            for (int i = 0; i < levelCorrespondEnemyInfo.timesEnemyList.Count; i++)
            {
                AddTimeList();
            }
        }

        /// <summary>
        /// 初始化怪物实体的挂载父亲
        /// </summary>
        private void InitBigTypeGameObject()
        {
            if (GameObject.Find("Level_" + GetLevelID()+"Time_" + (EnemyTime + 1)))
            {
                return;
            }
            GameObject time = new GameObject("Time_" + (EnemyTime+1));
            time.transform.SetParent(GameObject.Find("Level_" + GetLevelID()).transform);

            time.transform.localPosition = Vector3.zero;
            int count = EnemyBigTypeFolderNameList.Length;
            for (int i = 0; i < count; i++)
            {
                GameObject go = new GameObject(EnemyBigTypeFolderNameList[i]);
                go.transform.SetParent(GameObject.Find($"Enemy/Level_{ GetLevelID()}/Time_" + (EnemyTime + 1)).transform);
            }
        }

        /// <summary>
        /// 初始化怪物   index表示波次
        /// </summary>
        private void InitEnemy(int index)
        {
            List<BigTypeEntityInfo> bigTypeEntityInfos = levelCorrespondEnemyInfo.timesEnemyList[index].BigTypeInfoList;

            for (int i = 0; i < bigTypeEntityInfos.Count; i++)
            {
                List<SmallTypeEntityInfo> smallTypeEntityInfos = bigTypeEntityInfos[i].SmallTypeInfoList;   //拿到单个大类型下的小类型列表
                //InitBigTypeGameObject(i);

                for (int j = 0; j < smallTypeEntityInfos.Count; j++)
                {
                    //smallTypeEntityInfos[j].Id = 100 * (i + 1) + (j + 1);

                    List<TransformInfo> infoList = smallTypeEntityInfos[j].infoList;   //拿到单个小类型的所有物体列表
                    for (int m = 0; m < infoList.Count; m++)
                    {
                        EnemyBigType = i;
                        ChangeToolBar();
                        InstantiateEnemy(i, j, m, infoList[m]);
                    }
                }
            }

            EnemyBigType = 0;
            ChangeToolBar();

            JudgeEntityInfo(GetCurrentEnemyInfo(true), 0, ref enemyPos, ref enemyRot, ref enemyScal);
        }

        /// <summary>
        /// 添加怪物
        /// </summary>
        public void AddEnemy()
        {
            //获取这个小类型的建筑列表
            List<TransformInfo> infoList = GetCurrentEnemyInfo(true);
            EnemyIndex = infoList.Count;
            infoList.Add(new TransformInfo());
            InstantiateEnemy(EnemyBigType, EnemySmallType, infoList.Count-1, infoList[infoList.Count - 1]);
          
        }
        /// <summary>
        /// 添加怪物   笔刷加怪物
        /// </summary>
        public void AddEnemy(Vector3 pos)
        {
            //获取这个小类型的建筑列表
            List<TransformInfo> infoList = GetCurrentEnemyInfo(true);
            infoList.Add(new TransformInfo());
            //infoList[infoList.Count-1].pos =new double[] { pos.x, pos.y, pos.z };
            infoList[infoList.Count - 1].pos[0] = pos.x;
            infoList[infoList.Count - 1].pos[1] = pos.y;
            infoList[infoList.Count - 1].pos[2] = pos.z;
            InstantiateEnemy(EnemyBigType, EnemySmallType, infoList.Count - 1, infoList[infoList.Count - 1]);
            EnemyIndex = infoList.Count - 1;

            JudgeEntityInfo(infoList, EnemyIndex, ref enemyPos, ref enemyRot, ref enemyScal);
        }

        /// <summary>
        /// 实例化建筑出来
        /// </summary>
        /// <param name="j">小类型</param>
        /// <param name="m">编号</param>
        /// <param name="info">位置信息</param>
        private void InstantiateEnemy(int i, int j, int m, TransformInfo info)
        {
            //实例化建筑
            string folderName = EnemyBigTypeFolderNameList[i];
            string buildName = EnemySmallAssetNameList[j];
            string assetName = $"Enemy/{folderName}/{buildName}";

            string fatherName = $"Enemy/Level_{ GetLevelID()}/Time_{(EnemyTime + 1)}/{folderName}";

            InstantiateEntity(assetName, fatherName, buildName, m, info);

            AddEntityList(EnemyList, EnemySmallList[j], m);
        }

        /// <summary>
        /// 切换波次
        /// </summary>
        public void ChangeTimes()
        {
            EnemyBigType = 0;
            ChangeToolBar();

            InitBigTypeGameObject();
            InitEnemy(EnemyTime);
        }

        /// <summary>
        /// 切换大类型
        /// </summary>
        public void ChangeToolBar()
        {
            switch (EnemyBigType)
            {
                case 0:
                    EnemySmallList = landTypeArray;
                    EnemySmallAssetNameList = landAssetNameArray;
                    break;
                case 1:
                    EnemySmallList = trapTypeArray;
                    EnemySmallAssetNameList = trapAssetNameArray;
                    break;
                case 2:
                    EnemySmallList = flyTypeArray;
                    EnemySmallAssetNameList = flyAssetNameArray;
                    break;
                case 3:
                    EnemySmallList = smallBossTypeArray;
                    EnemySmallAssetNameList = smallBossAssetNameArray;
                    break;
                case 4:
                    EnemySmallList = bigBossTypeArray;
                    EnemySmallAssetNameList = bigBossAssetNameArray;
                    break;
            }
            enemyBigTypeFolderName = EnemyBigTypeFolderNameList[EnemyBigType];

            EnemySmallType = 0;
            ShowEnemyName();
        }

        /// <summary>
        /// 切换小类型
        /// </summary>
        public void ChangeSmallType()
        {
            ShowEnemyName();
        }

        /// <summary>
        /// 得到波次的信息
        /// </summary>
        /// <returns></returns>
        private TimesCorrespondEnemy GetCurrentTimeEnemyInfo(int index)
        {
            return levelCorrespondEnemyInfo.timesEnemyList[index];
        }

        /// <summary>
        /// 获得怪物信息   true表示获取的最新的  false表示获取的切换之前的
        /// </summary>
        private List<TransformInfo> GetCurrentEnemyInfo(bool isNew)
        {
            int time = isNew ? EnemyTime : EnemyOldTime;
            int bigType = isNew ? EnemyBigType : EnemyOldBigType;
            int smallType = isNew ? EnemySmallType : EnemyOldSmallType;
            Debug.Log("波次"+time+"怪物大编号：" + bigType + "  " + "小编号" + smallType);

            BigTypeEntityInfo bigTypeEntityInfo = GetCurrentTimeEnemyInfo(time).BigTypeInfoList[bigType];
            SmallTypeEntityInfo smallTypeEntityInfo = bigTypeEntityInfo.SmallTypeInfoList[smallType];
            //获取这个小类型的建筑列表
            List<TransformInfo> infoList = smallTypeEntityInfo.infoList;
            return infoList;
        }

        /// <summary>
        /// 显示怪物信息  编辑器界面的显示
        /// </summary>
        private void ShowEnemyName()
        {
            //先清空建筑列表
            EnemyList.Clear();
            EnemyIndex = 0;
            //获取这个小类型的建筑列表
            List<TransformInfo> infoList = GetCurrentEnemyInfo(true);
            Debug.Log("怪物信息"+JsonMapper.ToJson(infoList));
            for (int i = 0; i < infoList.Count; i++)  //
            {
                AddEntityList(EnemyList, EnemySmallList[EnemySmallType], i);
            }
            JudgeEntityInfo( infoList, 0, ref enemyPos, ref enemyRot, ref enemyScal);
        }

        /// <summary>
        /// 保存怪物的信息  只要发生了切换  就要保存   time是切换的时机
        /// </summary>
        public void SaveEnemyTransInfo(int index, int time)
        {
            currentGameObject = null;

            List<TransformInfo> enemys = GetCurrentEnemyInfo(false);
            Debug.Log("保存的建筑信息 "+index+" "+JsonMapper.ToJson(enemys));
            Debug.Log($"{enemyPos[0]},{enemyRot[1]},{enemyScal[2]}");
            //如果要保存的建筑为空，则不储存
            if (enemys.Count > index)
            {
                enemys[index].pos = new double[] { Math.Round(enemyPos[0],2), Math.Round(enemyPos[1],2), Math.Round(enemyPos[2],2) };
                enemys[index].rot = new double[] { Math.Round(enemyRot[0],2), Math.Round(enemyRot[1],2), Math.Round(enemyRot[2],2) };
                enemys[index].scal = new double[] { Math.Round(enemyScal[0],2), Math.Round(enemyScal[1],2), Math.Round(enemyScal[2],2) };
            }

            switch (time)
            {
                case 1:   //切换新的编号就是增加新的
                    break;
                case 2:   //切换新的小类型
                    ChangeSmallType();
                    break;
                case 3:   //切换新的大类型
                    ChangeToolBar();
                    break;
                case 4:   //切换波次
                    ChangeTimes();
                    break;
            }

            //以上的操作是切换之前旧的建筑  以下是新的建筑显示
            
            //获得的新的建筑的信息是空的话  位置是默认的
            List<TransformInfo> newBuild = GetCurrentEnemyInfo(true);
            JudgeEntityInfo(newBuild,  EnemyIndex, ref enemyPos, ref enemyRot, ref enemyScal);
            Debug.Log("新的建筑信息"+JsonMapper.ToJson(newBuild));
        }

        /// <summary>
        /// 设置笔刷模板
        /// </summary>
        public void SetTemplate()
        {
            string folderName = EnemyBigTypeFolderNameList[EnemyBigType];
            string buildName = EnemySmallAssetNameList[EnemySmallType];
            string assetName = $"Enemy/{folderName}/{buildName}";
            GameObject asset = Resources.Load(assetName) as GameObject;
            SetTemplate(asset, new ProductTemplateCallBack(AddEnemy));
        }

        /// <summary>
        ///  在scene视图移动怪物的时候，位置信息同步到编辑器界面
        /// </summary>
        public void RepaintCurrentEnemy()
        {
            if (currentGameObject == null)
            {
                return;
            }

            enemyPos = currentGameObject.transform.localPosition;
            enemyRot = currentGameObject.transform.localEulerAngles;
            enemyScal = currentGameObject.transform.localScale;
        }

        public void ShowSelectionInfo(GameObject go)
        {
            if (go?.transform?.parent?.parent.name == "Enemy")  //选中的是建筑信息
            {
                for (int i = 0; i < EnemyBigTypeFolderNameList.Length; i++)
                {
                    if (go.transform.parent.name == EnemyBigTypeFolderNameList[i])
                    {
                        //先把之前的保存起来
                        SaveEnemyTransInfo(EnemyIndex, 3);

                        EnemyBigType = i;    //大类型的编号
                        ChangeToolBar();
                        for (int j = 0; j < EnemySmallList.Length; j++)
                        {
                            if (go.name.StartsWith(EnemySmallList[j]))
                            {
                                EnemySmallType = j;    //小类型的编号
                                ShowEnemyName();

                                string[] name = go.name.Split('_');
                                EnemyIndex = Convert.ToInt32(name[2]);   //建筑物的编号
                                currentGameObject = go;
                                return;
                            }
                        }
                    }
                }
            }
        }

    }
}
