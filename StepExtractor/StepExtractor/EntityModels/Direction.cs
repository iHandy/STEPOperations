using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StepExtractor.EntityModels
{
    class Direction : IEntityModel
    {
        /// <summary>
        /// Название сущности в STEP файле
        /// </summary>
        public const string NAME = "DIRECTION";

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

        private List<double> directionRatios;

        /// <summary>
        /// Список коэффициентов направления. Должен содержать 3 значения.
        /// </summary>
        public List<double> DirectionRatios
        {
            get
            {
                return directionRatios;
            }
            set
            {
                /*if (value.Count != 3)
                {
                    throw new ArgumentOutOfRangeException("DirectionRatios", "Must be 3 values! Список должен содержать 3 значения!");
                }
                else
                {*/
                    directionRatios = value;
                //}
            }
        }
    }
}
