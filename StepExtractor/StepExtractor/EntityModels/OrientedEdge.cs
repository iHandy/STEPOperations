using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StepExtractor.EntityModels
{
    public class OrientedEdge : Edge, IEntityModel
    {
        /// <summary>
        /// Название сущности в STEP файле
        /// </summary>
        public const string NAME = "ORIENTED_EDGE";

        /// <summary>
        /// Свойство, реализуемое из интерфейса IEntityModel, возвращающее название сущности
        /// </summary>
        public string StepName
        {
            get { return NAME; }
        }

        public Edge EdgeElement { get; set; }

        public string Orientation { get; set; }
    }
}
