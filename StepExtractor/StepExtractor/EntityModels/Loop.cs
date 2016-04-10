using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StepExtractor.EntityModels
{
    abstract class Loop
    {
        ///// <summary>
        ///// Название сущности в STEP файле
        ///// </summary>
        //public static string NAME = "LOOP";

        /// <summary>
        /// Свойство, реализуемое из интерфейса IEntityModel, возвращающее название сущности
        /// </summary>
        //public string StepName
        //{
        //    get { return NAME; }
        //}

        /// <summary>
        /// Имя сущности
        /// </summary>
        public string Name { get; set; }
    }
}
