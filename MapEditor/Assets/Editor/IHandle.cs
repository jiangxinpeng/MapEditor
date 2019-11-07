using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArrowLegend.MapEditor
{
    interface IHandle
    {
        /// <summary>
        /// 初始化操作
        /// </summary>
        void Init();

        /// <summary>
        /// 销毁操作
        /// </summary>
        void Destory();

        /// <summary>
        /// 切换关卡
        /// </summary>
        /// <param name="level">切换之后的关卡名</param>
        void ChangeLevel(int level);

        /// <summary>
        /// 切换关卡之后的逻辑  levelInfo被重新赋值
        /// </summary>
        void AfterChangeLevel();

        /// <summary>
        /// 创建新的关卡配置
        /// </summary>
        void CreateNewLevel();

    }
}
