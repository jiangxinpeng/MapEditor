﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArrowLegend.MapEditor
{
    /// <summary>
    /// 每一波怪物的信息
    /// </summary>
   public class TimesCorrespondEnemy
    {
        public int times;      //当前波次

        public TimesEnemyProduceType Procedure;  //产生的时机

        public double ProductTime;   //产生的时间，游戏开始之后多少s

        public int Sum;   //怪物总数量

        public List<BigTypeEntityInfo> BigTypeInfoList = new List<BigTypeEntityInfo>();   //怪物类型对应的怪物实例  计算方式  大类型*100+小类型ID = int
    }
}
