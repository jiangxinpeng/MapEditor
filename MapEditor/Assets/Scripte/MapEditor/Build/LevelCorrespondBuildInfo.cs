using System;
using System.Collections.Generic;

namespace ArrowLegend.MapEditor
{
    /// <summary>
    /// 关卡所对应的建筑配置信息
    /// </summary>
    public class LevelCorrespondBuildInfo
    {
        public int Sum;                  //大类型的类型数量

        public List<BigTypeEntityInfo> BigTypeInfoList =  new List<BigTypeEntityInfo>();
    }
}
