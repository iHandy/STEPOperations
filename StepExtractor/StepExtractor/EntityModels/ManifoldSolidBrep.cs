using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StepExtractor.EntityModels
{
    class ManifoldSolidBrep : RepresentationItem, IEntityModel
    {
        /// <summary>
        /// Название сущности в STEP файле
        /// </summary>
        public const string NAME = "MANIFOLD_SOLID_BREP";

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

        public ClosedShell Outer { get; set; }
    }
}
