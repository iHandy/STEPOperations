using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StepExtractor.EntityModels;
using System.Windows.Forms;

namespace StepExtractor
{
    /// <summary>
    /// Интерфейс, который должен реализовывать вызывающий класс, для получения статуса и результатов
    /// </summary>
    public interface IExtractOperationsCallback
    {

        /// <summary>
        /// Позволяет отслеживать текущее состояние работы парсера
        /// </summary>
        /// <param name="completePercent">Процент выполненной работы</param>
        void extractionStep(int completePercent);

        /// <summary>
        /// Сообщает о завершении всех операций
        /// </summary>
        /// <param name="rootEntity">Корневая сущность</param>
        /// <param name="rootNode">Корневой элемент дерева</param>
        void extractionComplete(IEntityModel rootEntity, TreeNode rootNode);

        /// <summary>
        /// Сообщает о неуспешном завершении операций.
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        void extractionFailed(string message);
    }
}
