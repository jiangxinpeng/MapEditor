using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArrowLegend.MapEditor
{
    /// <summary>
    /// 关卡对应的怪物信息
    /// </summary>
    public class LevelCorrespondEnemy
    {
        public int sumTimes;   //总波数

        public List<TimesCorrespondEnemy> timesEnemyList=new List<TimesCorrespondEnemy>();
    }
}
