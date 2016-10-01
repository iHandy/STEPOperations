using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StepExtractor.EntityModels
{
    public class ClosedShell : IEntityModel
    {
        /// <summary>
        /// Название сущности в STEP файле
        /// </summary>
        public const string NAME = "CLOSED_SHELL";

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

        public List<Face> CfsFaces { get; set; }
    }
}
