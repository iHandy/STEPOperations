using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StepExtractor.EntityModels
{
    class FaceBound : IEntityModel
    {
        /// <summary>
        /// Название сущности в STEP файле
        /// </summary>
        public const string NAME = "FACE_BOUND";

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

        public Loop Bound { get; set; }

        public string Orientation { get; set; }
    }
}
