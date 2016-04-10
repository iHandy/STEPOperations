using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StepExtractor.EntityModels
{
    /// <summary>
    /// Интерфейс, который должны наследовать все сущности. 
    /// </summary>
    public interface IEntityModel
    {
        /// <summary>
        /// Имя сущности, которое сохраняется в STEP файле
        /// </summary>
        string StepName { get; }
    }
}
