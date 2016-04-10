using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StepExtractor.EntityModels
{
    class FaceOuterBound : FaceBound
    {
        /// <summary>
        /// Название сущности в STEP файле
        /// </summary>
        public const string NAME = "FACE_OUTER_BOUND";

        /// <summary>
        /// Свойство, реализуемое из интерфейса IEntityModel, возвращающее название сущности
        /// </summary>
        public string StepName
        {
            get { return NAME; }
        }
    }
}
