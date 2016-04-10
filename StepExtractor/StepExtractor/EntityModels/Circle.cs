using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StepExtractor.EntityModels
{
    class Circle : Curve, IEntityModel
    {
        /// <summary>
        /// Название сущности в STEP файле
        /// </summary>
        public const string NAME = "CIRCLE";

        /// <summary>
        /// Свойство, реализуемое из интерфейса IEntityModel, возвращающее название сущности
        /// </summary>
        public string StepName
        {
            get { return NAME; }
        }

        public Axis2Placement3D Position { get; set; }

        public double Radius { get; set; }
    }
}
