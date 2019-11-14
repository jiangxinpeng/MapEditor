
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ArrowLegend.MapEditor
{
    /// <summary>
    /// 小类型
    /// </summary>
    public class SmallTypeEntityInfo
    {
        public int Id;     //编号

        public List<TransformInfo> infoList=new List<TransformInfo>();    //对应的信息
    }
}
