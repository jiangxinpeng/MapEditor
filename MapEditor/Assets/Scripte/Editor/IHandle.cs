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

    }
}
