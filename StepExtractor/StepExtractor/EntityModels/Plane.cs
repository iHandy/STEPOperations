using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StepExtractor.EntityModels
{
    class Plane : Surface, IEntityModel
    {
        /// <summary>
        /// Название сущности в STEP файле
        /// </summary>
        public const string NAME = "PLANE";

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

        public Axis2Placement3D Position { get; set; }
    }
}
