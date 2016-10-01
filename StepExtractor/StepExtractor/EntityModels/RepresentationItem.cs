using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StepExtractor.EntityModels
{
    abstract public class RepresentationItem : IEntityModel
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
    }
}
