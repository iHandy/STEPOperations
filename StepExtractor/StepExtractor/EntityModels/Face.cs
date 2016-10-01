using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StepExtractor.EntityModels
{
    public abstract class Face
    {
        ///// <summary>
        ///// Название сущности в STEP файле
        ///// </summary>
        //public static string NAME = "FACE";

        ///// <summary>
        ///// Свойство, реализуемое из интерфейса IEntityModel, возвращающее название сущности
        ///// </summary>
        //public string StepName
        //{
        //    get { return NAME; }
        //}

        /// <summary>
        /// Имя сущности
        /// </summary>
        public string Name { get; set; }

        public List<FaceBound> Bounds { get; set; }
    }
}
