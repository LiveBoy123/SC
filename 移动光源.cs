//参考@被遗忘的青色剑士 科技版MOD
//由@不勤奋的儒雅 提供注释解析

//移动光源
namespace CommandBlock
{
    public class LightBlock : Block
    {
        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
        }

        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
        {
        }

        public const int Index = 600;//注意不要冲突
    }
}

namespace Game
{

    public class 手持岩浆桶移动光源行为 : SubsystemBlockBehavior, IUpdateable
    {

        public override int[] HandledBlocks
        {
            get
            {
                return new int[]
				{
                  MagmaBucketBlock.Index//岩浆桶
				};
            }

        }

        public UpdateOrder UpdateOrder
        {
            get
            {
                return 0;
            }
        }

        public override void Load(ValuesDictionary valuesDictionary)
        {
            base.Load(valuesDictionary);
            this.subsystemPlayers = base.Project.FindSubsystem<SubsystemPlayers>();
            this.subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>();
            this.subsystemTime = base.Project.FindSubsystem<SubsystemTime>();
            this.subsystemFire = base.Project.FindSubsystem<SubsystemFireBlockBehavior>();
        }

        public void Update(float dt)
        {
            bool flag = this.subsystemTime.PeriodicGameTimeEvent(0.1, 0.0);
            if (flag)//每0.1秒执行一次，防止执行过多卡死
            {
                int num = 0;
                foreach (ComponentPlayer componentPlayer in this.subsystemPlayers.ComponentPlayers)//查找全世界中的玩家（组件玩家componentplayer）
                {
                    //玩家点位
                    Point3 point = this.GetPoint(componentPlayer.ComponentBody.Position) + Point3.UnitY;
                    IInventory inventory = componentPlayer.ComponentMiner.Inventory;//玩家物品栏
                    bool flag2 = Terrain.ExtractContents(inventory.GetSlotValue(inventory.ActiveSlotIndex)) == MagmaBucketBlock.Index;//判断手持方块的index是不是岩浆桶
                    if (flag2)//如果手持方块的index是火把
                    {
                        //判断这个点是不是没有光亮方块了，没有的话就执行
                        bool flag3 = point != this.lightingPoints[num];
                        if (flag3)
                        {
                            Log.Information(base.SubsystemTerrain.Terrain.GetCellContents(point.X, point.Y, point.Z));//日志，可以删除
                            bool flag4 = base.SubsystemTerrain.Terrain.GetCellContents(point.X, point.Y, point.Z) == 0;//判断玩家点位的方块是不是空气
                            if (flag4)//如果是空气就执行
                            {
                                this.subsystemTerrain.ChangeCell(point.X, point.Y, point.Z, LightBlock.Index, true);//把这个空气置换为LightBlock
                                bool flag5 = this.lightingPoints[num] != Point3.Zero;
                                if (flag5)
                                {
                                    Point3 point2 = this.lightingPoints[num];
                                    this.subsystemTerrain.ChangeCell(point2.X, point2.Y, point2.Z, 0, true);
                                }
                                this.lightingPoints[num] = point;
                            }
                        }
                    }
                    else
                    {
                        //如果手持方块的index不是火把 --> 删除掉发光的方块
                        bool flag6 = this.lightingPoints[num] != Point3.Zero;
                        if (flag6)
                        {
                            Point3 point3 = this.lightingPoints[num];
                            this.subsystemTerrain.ChangeCell(point3.X, point3.Y, point3.Z, AirBlock.Index, true);//把这个点上的方块置换为空气
                            this.lightingPoints[num] = Point3.Zero;
                        }
                    }
                    num++;//遍历用的
                }
            }
        }


        public Point3 GetPoint(Vector3 v)
        {
            return new Point3((int)MathUtils.Round(v.X), (int)MathUtils.Round(v.Y), (int)MathUtils.Round(v.Z));
        }
        private SubsystemPlayers subsystemPlayers;

        private SubsystemTerrain subsystemTerrain;

        private SubsystemTime subsystemTime;

        private SubsystemFireBlockBehavior subsystemFire;

        private readonly Point3[] lightingPoints = new Point3[4];
    }
}

namespace Game
{

    public class 手持火把移动光源行为 : SubsystemBlockBehavior, IUpdateable
    {

        public override int[] HandledBlocks
        {
            get
            {
                return new int[]//括号里面的数字表示读取的变量的数量，可自定义
				{
                  TorchBlock.Index//火把
				};
            }

        }

        public UpdateOrder UpdateOrder
        {
            get
            {
                return 0;
            }
        }

        //ComponentPlayer.Entity.FindComponent<ComponentBody>().Position脚部
        public override void Load(ValuesDictionary valuesDictionary)
        {
            base.Load(valuesDictionary);
            this.subsystemPlayers = base.Project.FindSubsystem<SubsystemPlayers>();
            this.subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>();
            this.subsystemTime = base.Project.FindSubsystem<SubsystemTime>();
            this.subsystemFire = base.Project.FindSubsystem<SubsystemFireBlockBehavior>();
        }


        public void Update(float dt)
        {
            bool flag = this.subsystemTime.PeriodicGameTimeEvent(0.1, 0.0);
            if (flag)//每0.1秒执行一次，防止执行过多卡死
            {
                int num = 0;
                foreach (ComponentPlayer componentPlayer in this.subsystemPlayers.ComponentPlayers)//查找全世界中的玩家（组件玩家componentplayer）
                {
                    //玩家点位
                    Point3 point = this.GetPoint(componentPlayer.ComponentBody.Position) + Point3.UnitY;
                    IInventory inventory = componentPlayer.ComponentMiner.Inventory;//玩家物品栏
                    bool flag2 = Terrain.ExtractContents(inventory.GetSlotValue(inventory.ActiveSlotIndex)) == TorchBlock.Index;//判断手持方块的index是不是火把
                    if (flag2)//如果手持方块的index是火把
                    {
                        //判断这个点是不是没有光亮方块了，没有的话就执行
                        bool flag3 = point != this.lightingPoints[num];
                        if (flag3)
                        {
                            Log.Information(base.SubsystemTerrain.Terrain.GetCellContents(point.X, point.Y, point.Z));//日志，可以删除
                            bool flag4 = base.SubsystemTerrain.Terrain.GetCellContents(point.X, point.Y, point.Z) == 0;//判断玩家点位的方块是不是空气
                            if (flag4)//如果是空气就执行
                            {
                                this.subsystemTerrain.ChangeCell(point.X, point.Y, point.Z, LightBlock.Index, true);//把这个空气置换为LightBlock
                                bool flag5 = this.lightingPoints[num] != Point3.Zero;
                                if (flag5)
                                {
                                    Point3 point2 = this.lightingPoints[num];
                                    this.subsystemTerrain.ChangeCell(point2.X, point2.Y, point2.Z, 0, true);
                                }
                                this.lightingPoints[num] = point;
                            }
                        }
                    }
                    else
                    {
                        //如果手持方块的index不是火把 --> 删除掉发光的方块
                        bool flag6 = this.lightingPoints[num] != Point3.Zero;
                        if (flag6)
                        {
                            Point3 point3 = this.lightingPoints[num];
                            this.subsystemTerrain.ChangeCell(point3.X, point3.Y, point3.Z, AirBlock.Index, true);//把这个点上的方块置换为空气
                            this.lightingPoints[num] = Point3.Zero;
                        }
                    }
                    num++;//遍历用的
                }
            }
        }


        public Point3 GetPoint(Vector3 v)
        {
            return new Point3((int)MathUtils.Round(v.X), (int)MathUtils.Round(v.Y), (int)MathUtils.Round(v.Z));
        }
        private SubsystemPlayers subsystemPlayers;

        private SubsystemTerrain subsystemTerrain;

        private SubsystemTime subsystemTime;

        private SubsystemFireBlockBehavior subsystemFire;

        private readonly Point3[] lightingPoints = new Point3[4];
    }
}