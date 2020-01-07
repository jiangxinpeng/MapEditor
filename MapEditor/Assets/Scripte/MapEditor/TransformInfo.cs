using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ArrowLegend.MapEditor
{
    /// <summary>
    /// 每一个实体的坐标信息
    /// </summary>
    public class TransformInfo
    {
        public double[] pos = new double[3];            //位置

        public double[] rot = new double[3];             //旋转角度

        public double[] scal = new double[3];           //缩放

        public List<double[]> patrolList = new List<double[]>();   //巡逻点的坐标（对于怪物来说）
    }
}
