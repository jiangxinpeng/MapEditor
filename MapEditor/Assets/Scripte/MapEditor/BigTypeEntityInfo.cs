
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ArrowLegend.MapEditor
{
    /// <summary>
    /// 大类型
    /// </summary>
    public class BigTypeEntityInfo
    {
        public int Id;    //大类型怪物的编号

        public int Sum;   //小类型的个数

        public List<SmallTypeEntityInfo> SmallTypeInfoList=new List<SmallTypeEntityInfo>();    //对应的信息
    }
}
