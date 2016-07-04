using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StepExtractor.EntityModels
{
    class BoundedSurface : Surface, IEntityModel
    {
        /// <summary>
        /// Название сущности в STEP файле
        /// </summary>
        public const string NAME = "( BOUNDED_SURFACE"; //cap - temp name

        /// <summary>
        /// Свойство, реализуемое из интерфейса IEntityModel, возвращающее название сущности
        /// </summary>
        public string StepName
        {
            get { return NAME; }
        }

        //too difficult
    }
}
