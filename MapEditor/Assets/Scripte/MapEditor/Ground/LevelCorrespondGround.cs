using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArrowLegend.MapEditor
{
   public class LevelCorrespondGround
    {
        public int[] mapSize = new int[2];  //地图的大小

        public string groundMaterial;    //地图纹理ID

        public TransformInfo tranInfo=new TransformInfo();
    }
}
