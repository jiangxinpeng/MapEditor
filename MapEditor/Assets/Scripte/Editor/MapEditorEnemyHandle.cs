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

        public TimesEnemyProduceType ProductTypeIndex = TimesEnemyProduceType.Interval;          //产生时机
        public double productTime = 10;  //默认产生间隔时间

        public int EnemyTime;          //怪物当前波次
        public List<string> TimesList = new List<string>();   //波次列表

        public int EnemyBigType;   //大的类型 
        public int EnemySmallType;       //小的类型
        public string[] EnemyBigList;
        public string[] EnemySmallList;

        List<string> keys;
        List<string[]> valus;

        public int PointIndex;    //怪物巡逻点编号
        public List<string> PointList = new List<string>();
        public Vector3 PointPos;   //巡逻点的坐标
        private GameObject point;    //选中的巡逻点的物体

        public MapEditorEnemyHandle()
        {
            keys = new List<string>(GlobalHandle.EnemyBigTypeNameList.Keys);
            EnemyBigList = keys.ToArray();

            valus = new List<string[]>(GlobalHandle.EnemyBigTypeNameList.Values);
            EnemySmallList = valus[0];
        }

        public void Init()
        {
            EnemyTime = 0;
            EnemyBigType = 0;
            EnemySmallType = 0;
            EnemySmallList = valus[EnemyBigType];


            BindEnemyInfo();
            InitEnemy();
        }

        public void Destory()
        {
            Save();
        }

        /// <summary>
        /// 把enemyInfo绑定给LevelInfo
        /// </summary>
        private void BindEnemyInfo()
        {
            //List<string[]> valus = new List<string[]>(GlobalHandle.EnemyBigTypeNameList.Values);
            levelCorrespondEnemyInfo = GlobalHandle.levelInfo.enemyTimesInfo;

            if (levelCorrespondEnemyInfo.timesEnemyList.Count == 0)  //这个关卡是新建的
            {
                levelCorrespondEnemyInfo.timesEnemyList.Add(new TimesCorrespondEnemy());
            }
            InitTimeList();

            BindEnemyTimeInfo();

        }

        private void BindEnemyTimeInfo()
        {
            for (int m = 0; m < levelCorrespondEnemyInfo.timesEnemyList.Count; m++)
            {
                levelCorrespondEnemyInfo.timesEnemyList[m].times = m + 1;
                if (levelCorrespondEnemyInfo.timesEnemyList[m].BigTypeInfoList.Count == 0)
                {
                    for (int i = 0; i < EnemyBigList.Length; i++)
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

                        levelCorrespondEnemyInfo.timesEnemyList[m].BigTypeInfoList.Add(bigTypeEntityInfo);
                    }
                }
            }
        }

        private void InitTimeList()
        {
            TimesList.Clear();
            int count = levelCorrespondEnemyInfo.timesEnemyList.Count;
            for (int i = 0; i < count; i++)
            {
                TimesList.Add($"第{i + 1}波怪物");
            }
            EnemyTime = 0;
        }

        /// <summary>
        /// 添加新的波次
        /// </summary>
        public void CreateNewTime()
        {
            levelCorrespondEnemyInfo.timesEnemyList.Add(new TimesCorrespondEnemy());
            BindEnemyTimeInfo();

            TimesList.Add($"第{TimesList.Count+1}波怪物");
            EnemyTime = TimesList.Count-1;

            CreateTimes(EnemyTime);
            EnemyBigType = 0;
            EnemySmallType = 0;
        }


        /// <summary>
        /// 初始化怪物   index表示波次
        /// </summary>
        private void InitEnemy()
        {
            InitBigTypeGameObject();
            CreateInstance();
        }

        /// <summary>
        /// 初始化怪物实体的挂载父亲
        /// </summary>
        private void InitBigTypeGameObject()
        {
            GameObject enemy = new GameObject("Enemy");
            enemy.transform.SetParent(GameObject.Find("Level_" + GlobalHandle.levelInfo.levelId).transform);
            enemy.transform.localPosition = Vector3.zero;

            for (int t = 0; t < levelCorrespondEnemyInfo.timesEnemyList.Count; t++)  //波次
            {
                CreateTimes(t);
            }
        }

        private void CreateTimes(int t)
        {
            //List<string> keys = new List<string>(GlobalHandle.EnemyBigTypeNameList.Keys);

            int count = GlobalHandle.EnemyBigTypeNameList.Count;
            GameObject times = new GameObject($"第{t + 1}波怪物");
            times.transform.SetParent(GameObject.Find($"Level_{GlobalHandle.levelInfo.levelId}/Enemy").transform);
            times.transform.localPosition = Vector3.zero;


            for (int i = 0; i < count; i++)
            {
                //挂载大类型
                GameObject big = new GameObject(keys[i]);
                big.transform.SetParent(GameObject.Find($"Level_{GlobalHandle.levelInfo.levelId}/Enemy/第{t + 1}波怪物").transform);
                //挂载小类型
                //List<string[]> valus = new List<string[]>(GlobalHandle.EnemyBigTypeNameList.Values);
                for (int j = 0; j < valus[i].Length; j++)
                {
                    GameObject small = new GameObject(valus[i][j]);
                    small.transform.SetParent(GameObject.Find($"Level_{GlobalHandle.levelInfo.levelId}/Enemy/第{t + 1}波怪物/{big.name}").transform);
                }
            }
        }

        private void CreateInstance()
        {
            int count = levelCorrespondEnemyInfo.timesEnemyList.Count;
            for (int t = 0; t < count; t++)  //i表示波次
            {
                List<BigTypeEntityInfo> bigTypeEntityInfos = levelCorrespondEnemyInfo.timesEnemyList[t].BigTypeInfoList;

                for (int i = 0; i < bigTypeEntityInfos.Count; i++)
                {
                    List<SmallTypeEntityInfo> smallTypeEntityInfos = bigTypeEntityInfos[i].SmallTypeInfoList;   //拿到单个大类型下的小类型列表

                    for (int j = 0; j < smallTypeEntityInfos.Count; j++)
                    {
                        //smallTypeEntityInfos[j].Id = 100 * (i + 1) + (j + 1);
                        List<TransformInfo> infoList = smallTypeEntityInfos[j].infoList;   //拿到单个小类型的所有物体列表
                        for (int m = 0; m < infoList.Count; m++)
                        {
                            InstantiateEnemy(t, i, j, m, infoList[m]);
                            InitPointObject(t, i, j, m, infoList[m]);  
                        }
                    }
                }

                //EnemyBigType = 0;
                //ChangeToolBar();

                //JudgeEntityInfo(GetCurrentEnemyInfo(true), 0, ref enemyPos, ref enemyRot, ref enemyScal);
                //InitPointPos(0);
            }
        }

        /// <summary>
        /// 实例化怪物出来
        /// </summary>
        /// <param name="j">小类型</param>
        /// <param name="m">编号</param>
        /// <param name="info">位置信息</param>
        private void InstantiateEnemy(int t, int i, int j, int m, TransformInfo info)
        {

            //实例化建筑
            string folderName = keys[i];
            string buildName = valus[i][j];
            string assetName = $"Enemy/{folderName}/{buildName}";

            string fatherName = $"Level_{GlobalHandle.levelInfo.levelId}/Enemy/第{t + 1}波怪物/{keys[i]}/{valus[i][j]}";

            InstantiateEntity(assetName, fatherName, buildName, m, info);
        }

        /// <summary>
        /// 初始化巡逻点
        /// </summary>
        public void InitPointObject(int t, int big, int small, int index, TransformInfo info)
        {
            GameObject asset = Resources.Load("怪物巡逻点") as GameObject;
            Transform groundParent = GameObject.Find($"Level_{GlobalHandle.levelInfo.levelId}/Enemy/第{t + 1}波怪物/{keys[big]}/{valus[big][small]}").transform;
            Transform instance = groundParent.GetChild(index);   //具体的实例

            GameObject parent = new GameObject("巡逻路径");
            parent.transform.SetParent(instance);
            parent.transform.localPosition = Vector3.zero;

            for (int i = 0; i < info.patrolList.Count; i++)
            {
                Vector3 pos = new Vector3((float)info.patrolList[i][0], (float)info.patrolList[i][1], (float)info.patrolList[i][2]);
                GameObject point = GameObject.Instantiate(asset);
                point.name = $"巡逻点_{i + 1}";
                point.transform.SetParent(parent.transform);
                //point.transform.localPosition =parent.transform.TransformPoint(pos);
                point.transform.position = pos;
            }
        }

        /// <summary>
        /// 切换波次
        /// </summary>
        public void ChangeTimes()
        {
            EnemyBigType = 0;
            ChangeToolBar();

            //InitBigTypeGameObject();
            //InitEnemy(EnemyTime);
        }

        /// <summary>
        /// 切换大类型
        /// </summary>
        public void ChangeToolBar()
        {
            //List<string[]> valus = new List<string[]>(GlobalHandle.EnemyBigTypeNameList.Values);
            EnemySmallList = valus[EnemyBigType];
            EnemySmallType = 0;
        }

        private void Save()
        {

            Transform enemy = GameObject.Find($"Level_{GlobalHandle.levelInfo.levelId}/Enemy").transform;
            int timesCount = enemy.childCount;
            if (timesCount<levelCorrespondEnemyInfo.timesEnemyList.Count)
            {
                levelCorrespondEnemyInfo.timesEnemyList.RemoveRange(timesCount, levelCorrespondEnemyInfo.timesEnemyList.Count-timesCount);
            }
            for (int t = 0; t < timesCount; t++)
            {
                //levelCorrespondEnemyInfo.timesEnemyList[t].times = t + 1;   在初始化的时候赋值了
                int timeSum=0;   //该波次的怪物总数

                Transform times = enemy.GetChild(t);
                int bigCount = enemy.GetChild(t).childCount;
                for (int i = 0; i < bigCount; i++)
                {
                    Transform big = times.GetChild(i);  //大类型
                    int smallCount = big.childCount;
                    for (int j = 0; j < smallCount; j++)
                    {
                        Transform small = big.GetChild(j);   //小类型

                        int instanceCount = small.childCount;
                        timeSum += instanceCount;
                        List<TransformInfo> infoList = levelCorrespondEnemyInfo.timesEnemyList[t].BigTypeInfoList[i].SmallTypeInfoList[j].infoList;
                        infoList.Clear();

                        for (int m = 0; m < instanceCount; m++)
                        {
                            Transform instance = small.GetChild(m);
                            //Debug.Log($"波次{t}大类型{i}小类型{j}实例{m}  总数{instanceCount}");
                            //if (m > infoList.Count)
                            //{
                                infoList.Add(new TransformInfo());
                           // }
                            TransformInfo info = infoList[m];
                            // TransformInfo info = levelCorrespondEnemyInfo.timesEnemyList[t].BigTypeInfoList[i].SmallTypeInfoList[j].infoList[m]; 
                            info.pos = new double[] { Math.Round(instance.localPosition.x, 2), Math.Round(instance.localPosition.y, 2), Math.Round(instance.localPosition.z, 2) };
                            info.rot = new double[] { Math.Round(instance.localRotation.x, 2), Math.Round(instance.localRotation.y, 2), Math.Round(instance.localRotation.z, 2) };
                            info.scal = new double[] { Math.Round(instance.localScale.x, 2), Math.Round(instance.localScale.y, 2), Math.Round(instance.localScale.z, 2) };

                            info.TypeId = instance.GetComponent<EnemyInfo>().TypeId;

                            Transform patrol = instance.Find("巡逻路径");
                            int patrolCount = patrol.childCount;
                            for (int p=0;p<patrolCount;p++)
                            {
                                Transform patrolInstance = patrol.GetChild(p);

                                if (p >= info.patrolList.Count)
                                {
                                    info.patrolList.Add(new double[3]);
                                }
                                //Vector3 worldPos = patrol.InverseTransformPoint(patrolInstance.localPosition);
                                info.patrolList[p] = new double[] { Math.Round(patrolInstance.position.x, 2), Math.Round(patrolInstance.position.y, 2), Math.Round(patrolInstance.position.z, 2) };
                            }
                        }
                    }
                }

                levelCorrespondEnemyInfo.timesEnemyList[t].Sum = timeSum;
            }
        }

        /// <summary>
        /// 保存每一波的怪物产生时间
        /// </summary>
        private void SaveEnemyProductTime()
        {
            //levelCorrespondEnemyInfo.timesEnemyList[EnemyOldTime].ProductTime = productTime;
        }

        /// <summary>
        /// 设置笔刷模板
        /// </summary>
        public void SetTemplate()
        {
            string folderName = EnemyBigList[EnemyBigType];
            string buildName = EnemySmallList[EnemySmallType];
            string assetName = $"Enemy/{folderName}/{buildName}";
            GameObject asset = Resources.Load(assetName) as GameObject;
            SetTemplate(asset, new ProductTemplateCallBack(AddBuild));
        }

        private void AddBuild(Vector3 vector3)
        {
            if (GameObject.Find("Map").transform.childCount == 0)
            {
                return;
            }

            Transform parent = GameObject.Find($"Level_{GlobalHandle.levelInfo.levelId}/Enemy/第{(EnemyTime + 1)}波怪物/{keys[EnemyBigType]}/{valus[EnemyBigType][EnemySmallType]}").transform;
            int count = parent.childCount;
            TransformInfo transformInfo = new TransformInfo(); 
            transformInfo.pos = new double[] { Math.Round(vector3.x, 2), Math.Round(vector3.y, 2), Math.Round(vector3.z, 2) };
            InstantiateEnemy(EnemyTime, EnemyBigType, EnemySmallType, count, transformInfo);
            InitPointObject(EnemyTime, EnemyBigType, EnemySmallType, count, transformInfo);
        }
    }
}
