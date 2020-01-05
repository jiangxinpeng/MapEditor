
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ArrowLegend.MapEditor
{
    /// <summary>
    /// 地图编辑器的尺寸和地表纹理数据和逻辑类
    /// </summary>
    class MapEditorSizeAndTextureHandle : BaseHandle, IHandle
    {
        public Vector2Int MapSize = new Vector2Int(25, 10);  //地图尺寸
        public string mapTexturePath = "GroundTexture";

        private GameObject ground;
        private MeshFilter groundMeshFilter;
        public Material GroundMaterial;      //地图纹理

        private string groundMaterialName;      //地图纹理名称

        public void Init()
        {
            MapSize = new Vector2Int(MapGeneratorEditor.levelInfo.mapSize[0], MapGeneratorEditor.levelInfo.mapSize[1]);
            groundMaterialName = string.IsNullOrEmpty(MapGeneratorEditor.levelInfo.groundMaterial) ? "defalutMaterial" : MapGeneratorEditor.levelInfo.groundMaterial;
            GroundMaterial = Resources.Load($"{mapTexturePath}/{groundMaterialName}") as Material;

            //配置存在的时候
            if (MapGeneratorEditor.levelInfo.levelId != 0)
            {
                ProductGround();
            }
        }

        public void Destory()
        {
            SaveInfo();
        }

        /// <summary>
        /// 地图尺寸是否符合条件
        /// </summary>
        /// <returns></returns>
        public bool IsEligible()
        {
            if (MapSize.x <= 0 || MapSize.y <= 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 切换关卡  isDestory表示是关闭界面还是切换关卡
        /// </summary>
        public void ChangeLevel(int level)
        {
            SaveInfo();
            DestoryGround();
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        private void SaveInfo()
        {
            SetGroundMaterial();
            SetMapSize();
        }

        /// <summary>
        /// 切换到新的关卡之后的操作
        /// </summary>
        public void AfterChangeLevel()
        {
            Init();
            ProductGround();
        }

        /// <summary>
        /// 创建新关卡
        /// </summary>
        public void CreateNewLevel()
        {
            SaveInfo();
            MapSize = new Vector2Int(25, 10);  //恢复标准尺寸
            DestoryGround();
            //ProductGround();
        }

        /// <summary>
        /// 产生地图
        /// </summary>
        public void ProductGround()
        {
            if (ground == null)
            {
                if (GameObject.Find("ground_" + MapGeneratorEditor.levelInfo.levelId) != null)
                {
                     GameObject.DestroyImmediate(GameObject.Find("ground_" + MapGeneratorEditor.levelInfo.levelId));
                   // return;
                }
                ground = new GameObject("ground_" + MapGeneratorEditor.levelInfo.levelId);
                Transform transform = ground.transform;
                transform.SetParent(GameObject.Find("Ground").transform);
                transform.localPosition = Vector3.zero;
                //transform.localPosition = new Vector3((MapGeneratorEditor.levelInfo.levelId-1)*33,0,0);
                groundMeshFilter = ground.AddComponent<MeshFilter>();
                UpdateMesh(groundMeshFilter);
                ground.AddComponent<MeshRenderer>();

                InitBigTypeGameObject();
                InitEnemyParentGameObject();
            }
            else
            {
                UpdateMesh(groundMeshFilter);
            }

            SetGroundMaterial();
        }

        /// <summary>
        /// 初始化建筑大型类别在project视图上
        /// </summary>
        /// <param name="index"></param>
        private void InitBigTypeGameObject()
        {
            GameObject build = new GameObject("Build");
            build.transform.SetParent(GameObject.Find("ground_" + MapGeneratorEditor.levelInfo.levelId).transform);
            build.transform.localPosition = Vector3.zero;

            int count = MapEditorBuildHandle.BuildBigTypeFolderNameList.Length;
            for (int i = 0; i < count; i++)
            {
                GameObject go = new GameObject(MapEditorBuildHandle.BuildBigTypeFolderNameList[i]);
                go.transform.SetParent(GameObject.Find("ground_" + MapGeneratorEditor.levelInfo.levelId + "/Build").transform);
            }
        }

        /// <summary>
        /// 初始化怪物类型父类在project视图上
        /// </summary>
        private void InitEnemyParentGameObject()
        {
            GameObject build = new GameObject("Enemy");
            build.transform.SetParent(GameObject.Find("ground_" + MapGeneratorEditor.levelInfo.levelId).transform);
            build.transform.localPosition = Vector3.zero;
        }


        /// <summary>
        /// 更新网格大小
        /// </summary>
        /// <param name="meshFilter"></param>
        private void UpdateMesh(MeshFilter meshFilter)
        {
            Mesh mesh = new Mesh();

            //计算Plane大小
            Vector2 size;
            Vector2 _cellSize = new Vector2(1, 1);
            Vector2Int _gridSize = MapSize;
            size.x = _cellSize.x * _gridSize.x;
            size.y = _cellSize.y * _gridSize.y;

            //计算Plane一半大小
            Vector2 halfSize = size / 2;

            //计算顶点及UV
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();

            Vector3 vertice = Vector3.zero;
            Vector2 uv = Vector3.zero;

            for (int y = 0; y < _gridSize.y + 1; y++)
            {
                vertice.z = y * _cellSize.y - halfSize.y;//计算顶点Y轴
                uv.y = y * _cellSize.y / size.y;//计算顶点纹理坐标V

                for (int x = 0; x < _gridSize.x + 1; x++)
                {
                    vertice.x = x * _cellSize.x - halfSize.x;//计算顶点X轴
                    uv.x = x * _cellSize.x / size.x;//计算顶点纹理坐标U

                    vertices.Add(vertice);//添加到顶点数组
                    uvs.Add(uv);//添加到纹理坐标数组
                }
            }

            //顶点序列
            int a = 0;
            int b = 0;
            int c = 0;
            int d = 0;
            int startIndex = 0;
            int[] indexs = new int[_gridSize.x * _gridSize.y * 2 * 3];//顶点序列
            for (int y = 0; y < _gridSize.y; y++)
            {
                for (int x = 0; x < _gridSize.x; x++)
                {
                    //四边形四个顶点
                    a = y * (_gridSize.x + 1) + x;//0
                    b = (y + 1) * (_gridSize.x + 1) + x;//1
                    c = b + 1;//2
                    d = a + 1;//3

                    //计算在数组中的起点序号
                    startIndex = y * _gridSize.x * 2 * 3 + x * 2 * 3;

                    //左上三角形
                    indexs[startIndex] = a;//0
                    indexs[startIndex + 1] = b;//1
                    indexs[startIndex + 2] = c;//2

                    //右下三角形
                    indexs[startIndex + 3] = c;//2
                    indexs[startIndex + 4] = d;//3
                    indexs[startIndex + 5] = a;//0
                }
            }

            //
            mesh.SetVertices(vertices);//设置顶点
            mesh.SetUVs(0, uvs);//设置UV
            mesh.SetIndices(indexs, MeshTopology.Triangles, 0);//设置顶点序列
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();

            meshFilter.mesh = mesh;
        }

        /// <summary>
        /// 销毁地面
        /// </summary>
        private void DestoryGround()
        {
            ground = null;
        }

        /// <summary>
        /// 设置地面的材质
        /// </summary>
        public void SetGroundMaterial()
        {
            if (ground != null)
            {
                ground.GetComponent<MeshRenderer>().material = GroundMaterial;
            }

            MapGeneratorEditor.levelInfo.groundMaterial = GroundMaterial.name;
        }

        /// <summary>
        /// 设置地图大小
        /// </summary>
        private void SetMapSize()
        {
            MapGeneratorEditor.levelInfo.mapSize[0] = MapSize.x == 0 ? 25 : MapSize.x;
            MapGeneratorEditor.levelInfo.mapSize[1] = MapSize.y == 0 ? 10 : MapSize.y;
        }

        /// <summary>
        /// 点击打开材质界面
        /// </summary>
        public Object OpenMaterial()
        {
            Object[] textures = Resources.LoadAll(mapTexturePath);
            if (textures.Length != 0)
            {
                return textures[0];
            }
            return null;
        }

    }
}
