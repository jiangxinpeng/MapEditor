
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
        private List<LevelCorrespondGround> groundInfo;

        public Vector2Int MapSize = new Vector2Int(1, 1);  //地图尺寸
        public string mapTexturePath = "GroundTexture";

        private GameObject groundParent;     //所有ground的父节点
        private GameObject currentGround;
        private MeshFilter groundMeshFilter;
        public Material GroundMaterial;      //地图纹理
        public string CurrGroundName = "null";        //当前ground的名字

        private string groundMaterialName;      //地图纹理名称

        public void Init()
        {
            BindGroundInfo();
            InitGround();
            Debug.Log("地图的关卡：" + GlobalHandle.levelInfo.levelId);
        }

        public void Destory()
        {
            SaveInfo();
            //Debug.Log("删除了第一关 创建新的第二关"+ GlobalHandle.levelInfo.levelId);
            Object.DestroyImmediate(GameObject.Find("Level_" + GlobalHandle.levelInfo.levelId));
        }

        private void BindGroundInfo()
        {
            groundInfo = GlobalHandle.levelInfo.GroundInfo;

            if (groundInfo.Count <= 0)
            {
                groundInfo.Add(new LevelCorrespondGround());
            }
        }

        private void InitGround()
        {
            if (GameObject.Find("Level_" + GlobalHandle.levelInfo.levelId) == null)
            {
               // GameObject.DestroyImmediate(GameObject.Find("Level_" + GlobalHandle.levelInfo.levelId));
                GameObject level = new GameObject("Level_" + GlobalHandle.levelInfo.levelId);
                level.transform.SetParent(GameObject.Find("Map").transform);
                level.transform.localPosition = Vector3.zero;

                groundParent = new GameObject("Ground");
                groundParent.transform.SetParent(level.transform);
                groundParent.transform.localPosition = Vector3.zero;
            }

            for (int i=0;i<groundInfo.Count;i++)
            {
                MapSize = new Vector2Int(groundInfo[i].mapSize[0], groundInfo[i].mapSize[1]);
                DefaultMapSize();
                groundMaterialName = string.IsNullOrEmpty(groundInfo[i].groundMaterial) ? "defalutMaterial" : groundInfo[i].groundMaterial;
                GroundMaterial = Resources.Load($"{mapTexturePath}/{groundMaterialName}") as Material;

                CreateGroundInstance(i);
            }
        }

        private void DefaultMapSize()
        {
            MapSize.x = MapSize.x == 0 ? 1 : MapSize.x;
            MapSize.y = MapSize.y == 0 ? 1 : MapSize.y;
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
        /// 保存数据
        /// </summary>
        private void SaveInfo()
        {
            int count = groundParent.transform.childCount;
            groundInfo.Clear();
            for (int i=0;i<count;i++)
            {
                Transform son= groundParent.transform.GetChild(i);
                int x = (int)son.GetComponent<MeshRenderer>().bounds.size.x;
                int y = (int)son.GetComponent<MeshRenderer>().bounds.size.z;

                groundInfo.Add(new LevelCorrespondGround());

                groundInfo[i].mapSize = new int[] { x, y };
                groundInfo[i].groundMaterial = son.GetComponent<MeshRenderer>().sharedMaterial.name;
                groundInfo[i].tranInfo.pos = new double[] { son.position.x, son.position.y, son.position.z };
                groundInfo[i].tranInfo.rot = new double[] { son.localEulerAngles.x, son.localEulerAngles.y, son.localEulerAngles.z };

            }
        }

        public void ProductGround()
        {
            groundInfo.Add(new LevelCorrespondGround());
            int index = groundInfo.Count-1;
            groundInfo[index].mapSize = new int[] {1,1};
            groundInfo[index].groundMaterial = "defalutMaterial";

            MapSize = new Vector2Int(groundInfo[index].mapSize[0], groundInfo[index].mapSize[1]);
            DefaultMapSize();
            GroundMaterial = Resources.Load($"{mapTexturePath}/{ groundInfo[index].groundMaterial}") as Material;

            CreateGroundInstance(index);
        }

        /// <summary>
        /// 产生地图
        /// </summary>index  ground的编号</summary>
        private void CreateGroundInstance(int index)
        {
            currentGround = new GameObject("ground_"+(index+1));
            Transform transform = currentGround.transform;
            transform.SetParent(groundParent.transform);
            transform.localPosition = Vector3.zero;
            //transform.localPosition = new Vector3((MapGeneratorEditor.levelInfo.levelId-1)*33,0,0);
            groundMeshFilter = currentGround.AddComponent<MeshFilter>();
            UpdateMesh(groundMeshFilter);
            currentGround.AddComponent<MeshRenderer>();
            SetGroundMaterial();

            double[] pos = groundInfo[index].tranInfo.pos;
            double[] rot = groundInfo[index].tranInfo.rot;
            currentGround.transform.position = new Vector3((float)pos[0], (float)pos[1],(float)pos[2]);
            currentGround.transform.localEulerAngles = new Vector3((float)rot[0], (float)rot[1], (float)rot[2]);

            CurrGroundName = currentGround.name;
        }

        public void ChangeSize()
        {
            groundMeshFilter = currentGround.GetComponent<MeshFilter>();
            UpdateMesh(groundMeshFilter);
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
            currentGround = null;
        }

        /// <summary>
        /// 设置地面的材质
        /// </summary>
        public void SetGroundMaterial()
        {
            if (currentGround != null)
            {
                currentGround.GetComponent<MeshRenderer>().material = GroundMaterial;
            }
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

        public void SelectChange(GameObject ground)
        {
            if (ground!=null&&ground.transform.parent.name=="Ground")
            {
                currentGround = ground;
                CurrGroundName = ground.name;
                int x = (int)ground.GetComponent<MeshRenderer>().bounds.size.x;
                int y = (int)ground.GetComponent<MeshRenderer>().bounds.size.z;
                MapSize = new Vector2Int(x,y);
                groundMaterialName = ground.GetComponent<MeshRenderer>().sharedMaterial.name;
                GroundMaterial = Resources.Load($"{mapTexturePath}/{ groundMaterialName}") as Material;

                Debug.Log("切换" + CurrGroundName);
            }
        }

    }
}
