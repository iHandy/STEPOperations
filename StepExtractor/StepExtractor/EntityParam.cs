using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StepExtractor
{
    class EntityParam
    {
        /// <summary>
        /// Строковое значение параметра
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// Имя, присвоенное параметру парсером
        /// </summary>
        public string replacedName { get; set; }

        /// <summary>
        /// Конструктор для инициализации полей
        /// </summary>
        /// <param name="value">Строковое значение параметра</param>
        public EntityParam(string value)
        {
            this.value = value;
        }

        /// <summary>
        /// Конструктор для инициализации полей
        /// </summary>
        /// <param name="value">Строковое значение параметра</param>
        /// <param name="replacedName">Имя, присвоенное параметру парсером</param>
        public EntityParam(string value, string replacedName)
        {
            this.value = value;
            this.replacedName = replacedName;
        }

        //Переопределение методов Equals и GetHashCode для того, 
        //чтобы осуществлять поиск и сравнение объектов в списках по присвоенному парсером имени.

        // override object.Equals
        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return replacedName.Equals(((EntityParam)obj).replacedName);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return replacedName.GetHashCode();
        }
    }
}
