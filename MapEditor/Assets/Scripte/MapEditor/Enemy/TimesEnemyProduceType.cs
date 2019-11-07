using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


    /// <summary>
    /// 每一波的怪物产生时机
    /// </summary>
   public enum TimesEnemyProduceType
    {

        Defalut,
        /// <summary>
        /// 上一波结束之后
        /// </summary>
        After,

        /// <summary>
        /// 游戏开始之后的间隔时间
        /// </summary>
        Interval,
    }

