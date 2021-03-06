﻿using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ArrowLegend.MapEditor
{
    public abstract class BaseHandle
    {
        public GameObject currentGameObject;   //当前产生的gameObject

        /// <summary>
        /// 是否存在关卡
        /// </summary>
        /// <returns></returns>
        public bool IsExitLevel()
        {
            if (MapGeneratorEditor.levelInfo.levelId == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 实例化出一个物体
        /// </summary>
        public void InstantiateEntity(string assetName,string fatherName,string smallTypeName, int index, TransformInfo info)
        {
            if (GameObject.Find(fatherName) == null)
            {
                MapGeneratorEditor.Tip("先创建关卡和地图！！！！");
                return;
            }

            if (GameObject.Find(fatherName+"/"+assetName))
            {
                return;
            }
            GameObject go = Resources.Load(assetName) as GameObject;
            go = GameObject.Instantiate(go);
            go.name = smallTypeName + "_" + index;
          
            go.transform.SetParent(GameObject.Find(fatherName).transform);
            go.transform.localPosition = new Vector3((float)info.pos[0], (float)info.pos[1], (float)info.pos[2]);
            go.transform.localEulerAngles = new Vector3((float)info.rot[0], (float)info.rot[1], (float)info.rot[2]);
            if (info.scal[0]==0)  //大小没有赋值就是用初始的值
            {
                info.scal[0] = go.transform.localScale.x;
                info.scal[1] = go.transform.localScale.y;
                info.scal[2] = go.transform.localScale.z;
            }

            //currentGameObject = go;
        }

        /// <summary>
        /// 填充编辑器数据   实例编号的数据
        /// </summary>
        public void AddEntityList(List<string> list,string buildName, int index)
        {
            //初始化编辑器数据
            list.Add("类型 " + buildName + " 编号" + index);
        }

        /// <summary>
        /// 判断是否有建筑信息  有的话显示出来
        /// </summary>
        /// <returns></returns>
        public void JudgeEntityInfo( List<TransformInfo> infoList, int index, ref Vector3 pos, ref Vector3 rot, ref Vector3 scal)
        {
            if (infoList.Count == 0)  //没有建筑的时候  显示默认位置信息
            {
                ShowEntityInfo(new TransformInfo(), ref pos, ref rot, ref scal);
            }
            else
            {
                ShowEntityInfo(infoList[index], ref pos, ref rot, ref scal);
            }
        }

        /// <summary>
        /// 显示实体的位置信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <param name="scal"></param>
        private void ShowEntityInfo(TransformInfo info, ref Vector3 pos, ref Vector3 rot, ref Vector3 scal)
        {
            pos = new Vector3((float)info.pos[0], (float)info.pos[1], (float)info.pos[2]);
            rot = new Vector3((float)info.rot[0], (float)info.rot[1], (float)info.rot[2]);
            scal = new Vector3((float)info.scal[0], (float)info.scal[1], (float)info.scal[2]);
        }

        /// <summary>
        /// 设置笔刷
        /// </summary>
        public void SetTemplate(GameObject go, ProductTemplateCallBack callBack)
        {
            PaintModeGridEditor.SetTemplate(go, callBack);
        }
    }
}
