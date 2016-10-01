using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StepExtractor.EntityModels
{
    public class Axis2Placement3D : RepresentationItem, IEntityModel
    {
        /// <summary>
        /// Название сущности в STEP файле
        /// </summary>
        public const string NAME = "AXIS2_PLACEMENT_3D";

        /// <summary>
        /// Свойство, реализуемое из интерфейса IEntityModel, возвращающее название сущности
        /// </summary>
        public string StepName
        {
            get { return NAME; }
        }

        /* Собственные параметры */

        /// <summary>
        /// Имя сущности
        /// </summary>
        public string Name { get; set; }

        public CartesianPoint Location { get; set; }

        public Direction Axis { get; set; }

        public Direction RefDirection { get; set; }
    }
}
