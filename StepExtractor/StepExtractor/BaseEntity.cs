using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StepExtractor
{
    /// <summary>
    /// Инкапсулирует базовое описание сущности
    /// </summary>
    class BaseEntity
    {
        /// <summary>
        /// Номер сущности
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// Имя сущности
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Парметры сущности - здесь содержит в одной строке все параметры (требует дальнейшей обработки)
        /// </summary>
        public EntityParam Params { get; set; }
        /// <summary>
        /// Параметры сущности, разделенные парсером. Представление List<List> необходимо, так как среди параметров встречаются наборы параметров. 
        /// Если параметр одиночный, то он будет находиться в списке один на 0 позиции.
        /// </summary>
        public List<List<EntityParam>> ParsedParams { get; set; }
    }
}
