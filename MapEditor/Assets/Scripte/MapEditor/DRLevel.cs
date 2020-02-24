using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArrowLegend.MapEditor
{
    /// <summary>
    /// 地图配置类
    /// </summary>
    public class DRLevel
    {
        public int levelId;  //关卡id

        public List<LevelCorrespondGround> GroundInfo = new List<LevelCorrespondGround>();

        public LevelCorrespondBuildInfo BuildInfo=new LevelCorrespondBuildInfo();  //关卡对应的建筑信息

        public LevelCorrespondEnemy enemyTimesInfo = new LevelCorrespondEnemy();   //关卡对应的怪物的信息

        public LevelCorrespondWeather weatherInfo;     //关卡对应的天气信息

        public List<double[]> escortList = new List<double[]>();   //护送目标的路径，如果存在护送目标的话

    }
}
