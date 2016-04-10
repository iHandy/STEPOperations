using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseGeometryYVP.GeomObjects;

namespace StepExtractor.EntityModels
{
    class CartesianPoint : Point3D, IEntityModel
    {
        /// <summary>
        /// Название сущности в STEP файле
        /// </summary>
        public const string NAME = "CARTESIAN_POINT";

        /// <summary>
        /// Свойство, реализуемое из интерфейса IEntityModel, возвращающее название сущности
        /// </summary>
        public string StepName
        {
            get { return NAME; }
        }

        /// <summary>
        /// Имя сущности
        /// </summary>
        public string Name { get; set; }
    }
}
