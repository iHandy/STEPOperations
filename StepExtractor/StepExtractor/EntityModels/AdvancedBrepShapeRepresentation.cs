using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StepExtractor.EntityModels
{
    class AdvancedBrepShapeRepresentation : IEntityModel
    {
        /// <summary>
        /// Название сущности в STEP файле
        /// </summary>
        public static string NAME = "ADVANCED_BREP_SHAPE_REPRESENTATION";

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

        public List<RepresentationItem> Items { get; set; }

        //public double Radius { get; set; }
    }
}
