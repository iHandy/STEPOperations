using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StepExtractor.EntityModels
{
    public class Edge : IEntityModel
    {
        /// <summary>
        /// Название сущности в STEP файле
        /// </summary>
        public const string NAME = "EDGE";

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

        public Vertex EdgeStart { get; set; }

        public Vertex EdgeEnd { get; set; }
    }
}
