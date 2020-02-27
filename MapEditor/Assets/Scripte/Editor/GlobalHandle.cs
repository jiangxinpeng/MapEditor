using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace ArrowLegend.MapEditor
{
    /// <summary>
    /// 全局的变量和操作
    /// </summary>
     class GlobalHandle
    {
        public static DRLevel levelInfo;    //静态的变量抛给外部公用

        public static Dictionary<string, string[]> BuildBigTypeNameList = new Dictionary<string, string[]>();

        //public static string[] boundaryWallTypeArray = new string[] { "boundaryWall_1", "boundaryWall_2" };
        public static string[] grassTypeArray = new string[] { "caoqiu" };
        public static string[] treeTypeArray = new string[] {  "xianrenzhang1", "xianrenzhang2" };
        public static string[] stoneTypeArray = new string[] {"shitou1", "shitou2", "shitou3", "shitou4", "shitou5" };
        public static string[] shuganTypeArray = new string[] { "kushugan01" };
        //public static string[] stabTypeArray = new string[] { "stab_1", "stab_2" };
        //public static string[] waterTypeArray = new string[] { "water_1", "water_2" };
        public static string[] enterTypeArray = new string[] { "enter_1", "enter_2" };
        public static string[] outTypeArray = new string[] { "out_1", "out_2" };
        public static string[] pointTypeArray = new string[] { "carPoint"};
        public static string[] organTypeArray = new string[] { "流沙", "捕兽夹", "地雷" };

        public static Dictionary<string, string[]> EnemyBigTypeNameList = new Dictionary<string, string[]>();
        public static string[] landTypeArray = new string[] { "野狗", "巨型野狗", "阿努比斯Boss" };
        public static string[] trapTypeArray = new string[] { "吸血蜘蛛", "吞噬怪", "粘液怪" };
        public static string[] flyTypeArray = new string[] { "毒蜂", "毒蝶", "蜈蚣" };
        public static string[] smallBossTypeArray = new string[] { "独角兽", "巨胖尸怪" };
        public static string[] bigBossTypeArray = new string[] { "黑寡妇蜘蛛" };

        static GlobalHandle()
        {
            //BuildBigTypeNameList.Add("边界墙", boundaryWallTypeArray);
            BuildBigTypeNameList.Add("草球", grassTypeArray);
            BuildBigTypeNameList.Add("仙人掌", treeTypeArray);
            BuildBigTypeNameList.Add("石头", stoneTypeArray);
            BuildBigTypeNameList.Add("枯树干", shuganTypeArray);
            //BuildBigTypeNameList.Add("路刺", stabTypeArray);
            //BuildBigTypeNameList.Add("水面", waterTypeArray);
            BuildBigTypeNameList.Add("玩家入口", enterTypeArray);
            BuildBigTypeNameList.Add("玩家出口", outTypeArray);
            BuildBigTypeNameList.Add("机关", organTypeArray);
            BuildBigTypeNameList.Add("押运车路径", pointTypeArray);


            EnemyBigTypeNameList.Add("陆地小怪", landTypeArray);
            EnemyBigTypeNameList.Add("陷阱小怪", trapTypeArray);
            EnemyBigTypeNameList.Add("飞行小怪", flyTypeArray);
            EnemyBigTypeNameList.Add("小头目", smallBossTypeArray);
            EnemyBigTypeNameList.Add("大头目", bigBossTypeArray);
        }
        /// <summary>
        /// 提示界面
        /// </summary>
        public static void Tip(string content)
        {
            EditorUtility.DisplayDialog("提示", content, "好的");
        }
    }
}
