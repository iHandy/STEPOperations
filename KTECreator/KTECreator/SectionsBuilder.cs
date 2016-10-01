using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StepExtractor;
using StepExtractor.EntityModels;
using KTECreator.Sections;

namespace KTECreator
{
    class SectionsBuilder : IExtractOperationsCallback
    {

        private Form1 form1;

        public SectionsBuilder(Form1 form1)
        {
            this.form1 = form1;
        }

        /// <summary>
        /// Стартовая точка запуска алгоритмов поиска КТЭ
        /// </summary>
        public void build()
        {
            //Инициализация парсера STEP файлов для получения сущностей
            StepExtractor.StepExtractor extractor = new StepExtractor.StepExtractor(@"c:\Users\Handy\YandexDisk\Учеба\БНТУ 2016-2017\Полозков\STEPSoloviev\StepExtractor\StepExtractor\STEPFiles\model.step", this);
            //Запуск парсинга. Колбэк об успешном завершении реализован в текущем классе
            extractor.startExtraction();
        }

        public void extractionComplete(StepExtractor.EntityModels.IEntityModel rootEntity, System.Windows.Forms.TreeNode rootNode)
        {
            //Инициализация списка секций, который будет заполняться в процессе анализа данных от парсера
            List<Section> sections = new List<Section>();

            //Обращение к корневому элементу модели и получение всех его дочерних элементов
            foreach (var item in ((AdvancedBrepShapeRepresentation)rootEntity).Items)
            {
                //Поиск сущности, отвечающей за геометрию
                if (item is ManifoldSolidBrep)
                {
                    //Получене дочерних элементов
                    foreach (var item2 in ((ManifoldSolidBrep)item).Outer.CfsFaces)
                    {
                        //Поиск поверхностей
                        if (item2 is AdvancedFace)
                        {
                            Surface faceGeometry = ((AdvancedFace)item2).FaceGeometry;
                            Section newSection = null;

                            //Определение типов поверхностей и инициализация локальных объектов - секций
                            if (faceGeometry is Plane)
                            {
                                //Инициализация плоской секции
                                FlatSection flatSection = new FlatSection();
                                flatSection.EntityModel = faceGeometry as Plane;
                                newSection = flatSection;
                            }
                            else if (faceGeometry is ConicalSurface)
                            {
                                //Инициализация конической секции
                                ConicalSection conicalSection = new ConicalSection();
                                conicalSection.EntityModel = faceGeometry as ConicalSurface;
                                newSection = conicalSection;
                            }
                            else if (faceGeometry is CylindricalSurface)
                            {
                                //Инициализация цилиндрической секции
                                CylindricalSection cylindricalSection = new CylindricalSection();
                                cylindricalSection.EntityModel = faceGeometry as CylindricalSurface;
                                newSection = cylindricalSection;
                            } //Список может быть продолжен другими типами секций

                            if (newSection != null)
                            {
                                //Присвоение номера секции (позиция в списке)
                                newSection.Position = sections.Count + 1;

                                //Инициализация списка всех точек граней поверхности
                                newSection.allPoints = new List<CartesianPoint>(4);
                                newSection.startPoints = new List<CartesianPoint>(4);
                                newSection.endPoints = new List<CartesianPoint>(4);

                                //Поиск всех точек граней поверхности и добавление их в список
                                List<OrientedEdge> edges = (item2.Bounds[0].Bound as EdgeLoop).EdgeList;

                                foreach (var itemEdge in edges)
                                {
                                    CartesianPoint pointStart = (itemEdge.EdgeElement.EdgeStart as VertexPoint).VertexGeometry;

                                    newSection.startPoints.Add(pointStart);

                                    CartesianPoint pointEnd = (itemEdge.EdgeElement.EdgeEnd as VertexPoint).VertexGeometry;

                                    newSection.endPoints.Add(pointEnd);
                                }

                                /*CartesianPoint point = 
                                    ((item2.Bounds[0].Bound as EdgeLoop)
                                    .EdgeList[0].EdgeElement.EdgeStart as VertexPoint).VertexGeometry;
                                newSection.StartPoint = point;

                                point = 
                                    ((item2.Bounds[0].Bound as EdgeLoop)
                                    .EdgeList[0].EdgeElement.EdgeEnd as VertexPoint).VertexGeometry;
                                newSection.EndPoint = point;*/

                                //Добавление проинициализированной секции в список всех секций
                                sections.Add(newSection);
                            }
                        }

                    }

                }
            }

            //Запрос на отображение списка секций с их порядковыми номерами
            form1.showResult(sections);

            //Инициализация и отображение матрицы сопряжения отсеков
            form1.showMatrix(initCouplingMatrix(sections));
        }

        /// <summary>
        /// Инициализация первоначальной матрицы сопряжения отсеков поверхностей детали
        /// </summary>
        /// <param name="sections">Список секций</param>
        /// <returns>Матрица сопряжения отсеков поверхностей</returns>
        private bool[,] initCouplingMatrix(List<Section> sections)
        {
            //Сохранение количества секций
            int count = sections.Count;

            //Инициализация пустой матрицы сопряжения (двумерный массив)
            bool[,] matrix = new bool[count, count];

            //Обход по секциям. Поиск сопряжений других секций с текущей
            for (int i = 0; i < count - 1; i++)
            {
                var section = sections[i];
                //Обход по следующим секциям за текущей
                for (int j = i + 1; j < count; j++)
                {
                    //Временный алгоритм получения точек и определения сопряжения
                    var section2 = sections[j];
                    bool next = false;
                    int points = 0;
                    foreach (var item in section.startPoints)
                    {
                        if (next) break;
                        foreach (var item2 in section2.endPoints)
                        {
                            if (next) break;
                            if (item.Z.Equals(item2.Z))
                            { 
                                points++;
                                if (points > 1)
                                {
                                    matrix[j, i] = true;
                                    next = true;
                                }
                            }
                        }
                    }
                    
                }
            }

            return matrix;
        }

        public void extractionFailed(string message)
        {
            System.Windows.Forms.MessageBox.Show(message);
            throw new NotImplementedException();
        }

        public void extractionStep(int completePercent)
        {
            //throw new NotImplementedException();
        }
    }
}
