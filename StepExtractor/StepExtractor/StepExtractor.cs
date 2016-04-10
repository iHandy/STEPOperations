using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using StepExtractor.EntityModels;
using System.ComponentModel;
using System.IO;

namespace StepExtractor
{
    /// <summary>
    /// Класс для последовательной обработки STEP файла и формирования результатов.
    /// </summary>
    public sealed class StepExtractor
    {
        /// <summary>
        /// Путь к STEP файлу, инициализируется в конструкторе.
        /// </summary>
        private string mStepFilePath;

        private IExtractOperationsCallback mCallback;

        //Конвертер для получения числа с плавающей точки из строки
        private DoubleConverter doubleConverter = new DoubleConverter();

        //Объекты с результатами работы
        private Dictionary<int, BaseEntity> mEntitiesAsDictionary;
        private AdvancedBrepShapeRepresentation mRootEntityABSR;
        private TreeNode mRootTreeNode;

        /// <summary>
        /// Конструктор для создания нового объекта, к которому в дальнейшем можно обращаться за результатами.
        /// </summary>
        /// <param name="path">Путь к STEP файлу</param>
        /// <param name="callback">Callback для слежения за процессом и получения результата</param>
        public StepExtractor(string path, IExtractOperationsCallback callback)
        {
            mStepFilePath = path;
            mCallback = callback;
        }

        /// <summary>
        /// Запускает процесс парсинга и инициализации сущностей в отдельном потоке
        /// </summary>
        public void startExtraction()
        {
            Thread t = new Thread(extractionOperations);
            t.Start();
        }

        /// <summary>
        /// Набор операций, которые необходимо выполнять в отдельном потоке
        /// </summary>
        private void extractionOperations()
        {
            //Проверка полученного при инициализации пути к STEP файлу
            if (mStepFilePath == null || mStepFilePath.Length == 0)
            {
                if (mCallback != null)
                {
                    //Отправка коллбэка с ошибкой
                    mCallback.extractionFailed("File path is null or 0 length. Create new object with correct path.");
                }
                return;
            }

            //Создание объекта парсера для дальнейшей с ним работы
            StepParser stepParser = new StepParser();

            //Получение списка всех сущностей со всеми параметрами в соотвествии со STEP файлом
            List<BaseEntity> allPrimaryEntities = stepParser.getAllPrimaryEntities(mStepFilePath);

            if (mCallback != null)
            {
                //Отправка коллбэка с промежуточным прогрессом работы
                mCallback.extractionStep(28);
            }

            //Создание словаря на основании полученного выше списка. 
            mEntitiesAsDictionary = createDictionary(allPrimaryEntities);

            if (mCallback != null)
            {
                mCallback.extractionStep(43);
            }

            //Иницилизация всех сущностей и дерева TreeView
            initTreeNode(mEntitiesAsDictionary);

            if (mCallback != null)
            {
                //Отправка коллбэка об успешном завершении с необходимыми данными:
                //корневой сущностью, которая потребуется для дальнейшей обработки, и корневого элемента treeView - для отображения
                mCallback.extractionComplete(mRootEntityABSR, mRootTreeNode);
            }
        }

        /// <summary>
        /// Создание словаря на основании списка сущностей полученного парсером. Ключ - номер сущности.
        /// </summary>
        /// <param name="allPrimaryEntities">Список сущностей, полученный парсером в соотвествии со STEP файлом</param>
        /// <returns>Словарь: ключ - номер сущност, значение - объект сущности</returns>
        private Dictionary<int, BaseEntity> createDictionary(List<BaseEntity> allPrimaryEntities)
        {
            //Инициализация объекта словаря размером со список
            Dictionary<int, BaseEntity> resultDictionary = new Dictionary<int, BaseEntity>(allPrimaryEntities.Count);

            //Добавление элементов в словарь в соответствии с номерами сущностей
            foreach (BaseEntity item in allPrimaryEntities)
            {
                resultDictionary.Add(item.Number, item);
            }

            return resultDictionary;
        }

        private void initTreeNode(Dictionary<int, BaseEntity> entitiesDictionary)
        {
            //Поиск родительской сущности, которая содержит остальные сущности геометрии
            foreach (var item in entitiesDictionary)
            {
                //В качестве родительской выбрана сущность ADVANCED_BREP_SHAPE_REPRESENTATION
                if (item.Value.Name.Equals(AdvancedBrepShapeRepresentation.NAME))
                {
                    //Если название такой сущности найдено, то от этого узла начинаем инициализировать все сущности
                    mRootEntityABSR = new AdvancedBrepShapeRepresentation();
                    //Присвоение имени(title) сущности
                    mRootEntityABSR.Name = item.Value.ParsedParams[0][0].value;
                    //Инициализация вложенных параметров
                    mRootEntityABSR.Items = new List<RepresentationItem>(item.Value.ParsedParams[1].Count);

                    //Инициализация корневого узла дерева
                    mRootTreeNode = new TreeNode(mRootEntityABSR.StepName);
                    //Инициализация узла списка вложенных параметров (сущностей)
                    TreeNode paramsNode = new TreeNode("Items");
                    mRootTreeNode.Nodes.Add(paramsNode);

                    if (mCallback != null)
                    {
                        mCallback.extractionStep(49);
                    }

                    //Запуск рекурсивной инициализации сущностей для каждого вложенного параметра родительской сущности
                    foreach (var itemParam in item.Value.ParsedParams[1])
                    {
                        IEntityModel param = parseItemParam(itemParam, entitiesDictionary, paramsNode);
                        mRootEntityABSR.Items.Add((RepresentationItem)param);
                    }
                }
            }
        }

        /// <summary>
        /// Инициализация всех дочерних сущности для заданной сущности. Используется рекурсивно для каждого вложенного параметра.
        /// </summary>
        /// <param name="itemParam">Родительский параметр, для которого необходимо инициализировать параметры</param>
        /// <param name="entitiesDictionary">Словарь всех сущностей, полученных из STEP файла. Ключем должен быть номер сущности.</param>
        /// <param name="parentNode">Родительский узел дерева TreeView. Инициализация происходит параллельно.</param>
        /// <returns>Метод возвращает проинициализированную сущность до самого нижнего уровня.</returns>
        private IEntityModel parseItemParam(EntityParam itemParam, Dictionary<int, BaseEntity> entitiesDictionary, TreeNode parentNode)
        {
            if (mCallback != null)
            {
                //Примерное условное вычисление прогресса
                int addStep = (int)(50 / entitiesDictionary.Count);
                int step = 49 + (addStep < 1 ? 1 : addStep);
                mCallback.extractionStep(step < 98 ? step : 98);
            }

            //Инициализируемый параметр
            IEntityModel newParam = null;
            //Инициализируемый дочерний узел дерева
            TreeNode childNode = null;

            //Номер сущности, хранящийся в параметре
            int itemNumber;
            if (itemParam.value.StartsWith("#") && int.TryParse(itemParam.value.Substring(1), out itemNumber))
            {
                //Сущность из словаря, получаемая по номеру
                BaseEntity nextEntity;
                if (entitiesDictionary.TryGetValue(itemNumber, out nextEntity))
                {
                    //Подбор необходимого алгоритма инициализации по имени сущности из STEP файла
                    switch (nextEntity.Name)
                    {
                        //Комментарии для одного алгоритма, далее по аналогии.
                        case Axis2Placement3D.NAME:
                            //Создание объекта для соответствующего имени сущности
                            Axis2Placement3D tempParamA2P3D = new Axis2Placement3D();

                            //Создания нового узла дерева (узлы деревьев формируются параллельно инициализации сущностей)
                            childNode = new TreeNode(tempParamA2P3D.StepName);

                            //Присвоение имени (title) объекту сущности из параметров, полученных парсером из STEP файла
                            tempParamA2P3D.Name = nextEntity.ParsedParams[0][0].value;
                            //Добавление соотвествующего узла дерева
                            childNode.Nodes.Add("Label: " + tempParamA2P3D.Name);

                            //Рекурсивный вызов метода для инициализации остальных внутренних параметров
                            tempParamA2P3D.Location = (CartesianPoint)parseItemParam(nextEntity.ParsedParams[1][0], entitiesDictionary, childNode);
                            tempParamA2P3D.Axis = (Direction)parseItemParam(nextEntity.ParsedParams[2][0], entitiesDictionary, childNode);
                            tempParamA2P3D.RefDirection = (Direction)parseItemParam(nextEntity.ParsedParams[3][0], entitiesDictionary, childNode);

                            //Присвоение абстрактной переменной текущей сущности
                            newParam = tempParamA2P3D;
                            break;
                        case ManifoldSolidBrep.NAME:
                            ManifoldSolidBrep tempParamMSB = new ManifoldSolidBrep();
                            childNode = new TreeNode(tempParamMSB.StepName);
                            tempParamMSB.Name = nextEntity.ParsedParams[0][0].value;
                            childNode.Nodes.Add("Label: " + tempParamMSB.Name);
                            tempParamMSB.Outer = (ClosedShell)parseItemParam(nextEntity.ParsedParams[1][0], entitiesDictionary, childNode);
                            newParam = tempParamMSB;
                            break;
                        case ClosedShell.NAME:
                            ClosedShell tempParamCS = new ClosedShell();
                            childNode = new TreeNode(tempParamCS.StepName);
                            tempParamCS.Name = nextEntity.ParsedParams[0][0].value;
                            childNode.Nodes.Add("Label: " + tempParamCS.Name);
                            tempParamCS.CfsFaces = new List<Face>(nextEntity.ParsedParams[1].Count);
                            TreeNode childParamNode = new TreeNode("Cfs faces");
                            childNode.Nodes.Add(childParamNode);
                            foreach (var itemParamCS in nextEntity.ParsedParams[1])
                            {
                                IEntityModel param = parseItemParam(itemParamCS, entitiesDictionary, childParamNode);
                                tempParamCS.CfsFaces.Add((Face)param);
                            }
                            newParam = tempParamCS;
                            break;
                        case AdvancedFace.NAME:
                            AdvancedFace tempParamAF = new AdvancedFace();
                            childNode = new TreeNode(tempParamAF.StepName);
                            tempParamAF.Name = nextEntity.ParsedParams[0][0].value;
                            childNode.Nodes.Add("Label: " + tempParamAF.Name);
                            tempParamAF.Bounds = new List<FaceBound>(nextEntity.ParsedParams[1].Count);
                            TreeNode childParamNodeAF = new TreeNode("Bounds");
                            childNode.Nodes.Add(childParamNodeAF);
                            foreach (var itemParamAFB in nextEntity.ParsedParams[1])
                            {
                                IEntityModel param = parseItemParam(itemParamAFB, entitiesDictionary, childParamNodeAF);
                                tempParamAF.Bounds.Add((FaceBound)param);
                            }
                            tempParamAF.FaceGeometry = (Surface)parseItemParam(nextEntity.ParsedParams[2][0], entitiesDictionary, childNode);
                            tempParamAF.SameSense = nextEntity.ParsedParams[3][0].value;
                            childNode.Nodes.Add("Same sense: " + tempParamAF.SameSense);
                            newParam = tempParamAF;
                            break;
                        case FaceBound.NAME:
                            FaceBound tempParamFB = new FaceBound();
                            childNode = new TreeNode(tempParamFB.StepName);
                            tempParamFB.Name = nextEntity.ParsedParams[0][0].value;
                            childNode.Nodes.Add("Label: " + tempParamFB.Name);
                            tempParamFB.Bound = (Loop)parseItemParam(nextEntity.ParsedParams[1][0], entitiesDictionary, childNode);
                            tempParamFB.Orientation = nextEntity.ParsedParams[2][0].value;
                            childNode.Nodes.Add("Orientation: " + tempParamFB.Orientation);
                            newParam = tempParamFB;
                            break;
                        case FaceOuterBound.NAME:
                            FaceOuterBound tempParamFOB = new FaceOuterBound();
                            childNode = new TreeNode(tempParamFOB.StepName);
                            tempParamFOB.Name = nextEntity.ParsedParams[0][0].value;
                            childNode.Nodes.Add("Label: " + tempParamFOB.Name);
                            tempParamFOB.Bound = (Loop)parseItemParam(nextEntity.ParsedParams[1][0], entitiesDictionary, childNode);
                            tempParamFOB.Orientation = nextEntity.ParsedParams[2][0].value;
                            childNode.Nodes.Add("Orientation: " + tempParamFOB.Orientation);
                            newParam = tempParamFOB;
                            break;
                        case ConicalSurface.NAME:
                            ConicalSurface tempParamCoSu = new ConicalSurface();
                            childNode = new TreeNode(tempParamCoSu.StepName);
                            tempParamCoSu.Name = nextEntity.ParsedParams[0][0].value;
                            childNode.Nodes.Add("Label: " + tempParamCoSu.Name);
                            tempParamCoSu.Position = (Axis2Placement3D)parseItemParam(nextEntity.ParsedParams[1][0], entitiesDictionary, childNode);
                            tempParamCoSu.Radius = (double)doubleConverter.ConvertFromInvariantString(nextEntity.ParsedParams[2][0].value);
                            childNode.Nodes.Add("Radius: " + tempParamCoSu.Radius);
                            tempParamCoSu.SemiAngle = (double)doubleConverter.ConvertFromInvariantString(nextEntity.ParsedParams[3][0].value);
                            childNode.Nodes.Add("Semi angle: " + tempParamCoSu.SemiAngle);
                            newParam = tempParamCoSu;
                            break;
                        case CylindricalSurface.NAME:
                            CylindricalSurface tempParamCySu = new CylindricalSurface();
                            childNode = new TreeNode(tempParamCySu.StepName);
                            tempParamCySu.Name = nextEntity.ParsedParams[0][0].value;
                            childNode.Nodes.Add("Label: " + tempParamCySu.Name);
                            tempParamCySu.Position = (Axis2Placement3D)parseItemParam(nextEntity.ParsedParams[1][0], entitiesDictionary, childNode);
                            tempParamCySu.Radius = (double)doubleConverter.ConvertFromInvariantString(nextEntity.ParsedParams[2][0].value);
                            childNode.Nodes.Add("Radius: " + tempParamCySu.Radius);
                            newParam = tempParamCySu;
                            break;
                        case EdgeLoop.NAME:
                            EdgeLoop tempParamEL = new EdgeLoop();
                            childNode = new TreeNode(tempParamEL.StepName);
                            tempParamEL.Name = nextEntity.ParsedParams[0][0].value;
                            childNode.Nodes.Add("Label: " + tempParamEL.Name);
                            tempParamEL.EdgeList = new List<OrientedEdge>(nextEntity.ParsedParams[1].Count);
                            TreeNode childParamNodeEL = new TreeNode("Edge list");
                            childNode.Nodes.Add(childParamNodeEL);
                            foreach (var itemParamEL in nextEntity.ParsedParams[1])
                            {
                                IEntityModel param = parseItemParam(itemParamEL, entitiesDictionary, childParamNodeEL);
                                tempParamEL.EdgeList.Add((OrientedEdge)param);
                            }
                            newParam = tempParamEL;
                            break;
                        case OrientedEdge.NAME:
                            OrientedEdge tempParamOE = new OrientedEdge();
                            childNode = new TreeNode(tempParamOE.StepName);
                            tempParamOE.Name = nextEntity.ParsedParams[0][0].value;
                            childNode.Nodes.Add("Label: " + tempParamOE.Name);
                            tempParamOE.EdgeStart = (Vertex)parseItemParam(nextEntity.ParsedParams[1][0], entitiesDictionary, childNode);
                            tempParamOE.EdgeEnd = (Vertex)parseItemParam(nextEntity.ParsedParams[2][0], entitiesDictionary, childNode);
                            tempParamOE.EdgeElement = (Edge)parseItemParam(nextEntity.ParsedParams[3][0], entitiesDictionary, childNode);
                            tempParamOE.Orientation = nextEntity.ParsedParams[4][0].value;
                            childNode.Nodes.Add("Orientation: " + tempParamOE.Orientation);
                            newParam = tempParamOE;
                            break;
                        case Edge.NAME:
                            Edge tempParamE = new Edge();
                            childNode = new TreeNode(tempParamE.StepName);
                            tempParamE.Name = nextEntity.ParsedParams[0][0].value;
                            childNode.Nodes.Add("Label: " + tempParamE.Name);
                            tempParamE.EdgeStart = (Vertex)parseItemParam(nextEntity.ParsedParams[1][0], entitiesDictionary, childNode);
                            tempParamE.EdgeEnd = (Vertex)parseItemParam(nextEntity.ParsedParams[2][0], entitiesDictionary, childNode);
                            newParam = tempParamE;
                            break;
                        case VertexPoint.NAME:
                            VertexPoint tempParamVP = new VertexPoint();
                            childNode = new TreeNode(tempParamVP.StepName);
                            tempParamVP.Name = nextEntity.ParsedParams[0][0].value;
                            childNode.Nodes.Add("Label: " + tempParamVP.Name);
                            tempParamVP.VertexGeometry = (CartesianPoint)parseItemParam(nextEntity.ParsedParams[1][0], entitiesDictionary, childNode);
                            newParam = tempParamVP;
                            break;
                        case CartesianPoint.NAME:
                            CartesianPoint tempParamCP = new CartesianPoint();
                            childNode = new TreeNode(tempParamCP.StepName);
                            tempParamCP.Name = nextEntity.ParsedParams[0][0].value;
                            childNode.Nodes.Add("Label: " + tempParamCP.Name);
                            tempParamCP.X = (double)doubleConverter.ConvertFromInvariantString(nextEntity.ParsedParams[1][0].value);
                            childNode.Nodes.Add("X: " + tempParamCP.X);
                            tempParamCP.Y = (double)doubleConverter.ConvertFromInvariantString(nextEntity.ParsedParams[1][1].value);
                            childNode.Nodes.Add("Y: " + tempParamCP.Y);
                            tempParamCP.Z = (double)doubleConverter.ConvertFromInvariantString(nextEntity.ParsedParams[1][2].value);
                            childNode.Nodes.Add("Z: " + tempParamCP.Z);
                            newParam = tempParamCP;
                            break;
                        case EdgeCurve.NAME:
                            EdgeCurve tempParamEC = new EdgeCurve();
                            childNode = new TreeNode(tempParamEC.StepName);
                            tempParamEC.Name = nextEntity.ParsedParams[0][0].value;
                            childNode.Nodes.Add("Label: " + tempParamEC.Name);
                            tempParamEC.EdgeStart = (Vertex)parseItemParam(nextEntity.ParsedParams[1][0], entitiesDictionary, childNode);
                            tempParamEC.EdgeEnd = (Vertex)parseItemParam(nextEntity.ParsedParams[2][0], entitiesDictionary, childNode);
                            tempParamEC.EdgeGeometry = (Curve)parseItemParam(nextEntity.ParsedParams[3][0], entitiesDictionary, childNode);
                            tempParamEC.SameSense = nextEntity.ParsedParams[4][0].value;
                            childNode.Nodes.Add("Same sense: " + tempParamEC.SameSense);
                            newParam = tempParamEC;
                            break;
                        case Circle.NAME:
                            Circle tempParamCi = new Circle();
                            childNode = new TreeNode(tempParamCi.StepName);
                            tempParamCi.Name = nextEntity.ParsedParams[0][0].value;
                            childNode.Nodes.Add("Label: " + tempParamCi.Name);
                            tempParamCi.Position = (Axis2Placement3D)parseItemParam(nextEntity.ParsedParams[1][0], entitiesDictionary, childNode);
                            tempParamCi.Radius = (double)doubleConverter.ConvertFromInvariantString(nextEntity.ParsedParams[2][0].value);
                            childNode.Nodes.Add("Radius: " + tempParamCi.Radius);
                            newParam = tempParamCi;
                            break;
                        case Direction.NAME:
                            Direction tempParamDi = new Direction();
                            childNode = new TreeNode(tempParamDi.StepName);
                            tempParamDi.Name = nextEntity.ParsedParams[0][0].value;
                            childNode.Nodes.Add("Label: " + tempParamDi.Name);
                            TreeNode childParamNodeDi = new TreeNode("Direction ratios");
                            childNode.Nodes.Add(childParamNodeDi);
                            tempParamDi.DirectionRatios = new List<double>(3);
                            tempParamDi.DirectionRatios.Add((double)doubleConverter.ConvertFromInvariantString(nextEntity.ParsedParams[1][0].value));
                            tempParamDi.DirectionRatios.Add((double)doubleConverter.ConvertFromInvariantString(nextEntity.ParsedParams[1][1].value));
                            tempParamDi.DirectionRatios.Add((double)doubleConverter.ConvertFromInvariantString(nextEntity.ParsedParams[1][2].value));
                            childParamNodeDi.Nodes.Add(tempParamDi.DirectionRatios[0].ToString());
                            childParamNodeDi.Nodes.Add(tempParamDi.DirectionRatios[1].ToString());
                            childParamNodeDi.Nodes.Add(tempParamDi.DirectionRatios[2].ToString());
                            newParam = tempParamDi;
                            break;
                        case Line.NAME:
                            Line tempParamL = new Line();
                            childNode = new TreeNode(tempParamL.StepName);
                            tempParamL.Name = nextEntity.ParsedParams[0][0].value;
                            childNode.Nodes.Add("Label: " + tempParamL.Name);
                            tempParamL.Pnt = (CartesianPoint)parseItemParam(nextEntity.ParsedParams[1][0], entitiesDictionary, childNode);
                            tempParamL.Dir = (Vector)parseItemParam(nextEntity.ParsedParams[2][0], entitiesDictionary, childNode);
                            newParam = tempParamL;
                            break;
                        case Vector.NAME:
                            Vector tempParamV = new Vector();
                            childNode = new TreeNode(tempParamV.StepName);
                            tempParamV.Name = nextEntity.ParsedParams[0][0].value;
                            childNode.Nodes.Add("Label: " + tempParamV.Name);
                            tempParamV.Orientation = (Direction)parseItemParam(nextEntity.ParsedParams[1][0], entitiesDictionary, childNode);
                            tempParamV.Magnitude = (double)doubleConverter.ConvertFromInvariantString(nextEntity.ParsedParams[2][0].value);
                            childNode.Nodes.Add("Magnitude: " + tempParamV.Magnitude);
                            newParam = tempParamV;
                            break;
                        case Plane.NAME:
                            Plane tempParamPl = new Plane();
                            childNode = new TreeNode(tempParamPl.StepName);
                            tempParamPl.Name = nextEntity.ParsedParams[0][0].value;
                            childNode.Nodes.Add("Label: " + tempParamPl.Name);
                            tempParamPl.Position = (Axis2Placement3D)parseItemParam(nextEntity.ParsedParams[1][0], entitiesDictionary, childNode);
                            newParam = tempParamPl;
                            break;
                        default:
                            //Ошибка генерируется в том случае, если сущность не определена ни одним из алгоритмов => нужно создать новый класс сущности и прописать соответствующей ей алгоритм инициализации.
                            throw new InvalidDataException("Not found entity: " + nextEntity.Name);
                    }
                    //Добавление в родительский узел TreeView нового дочернего узла, проинициализированного в алгоритмах выше.
                    parentNode.Nodes.Add(childNode);
                }
            }
            return newParam;
        }
    }
}
