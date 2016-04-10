using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StepExtractor;

namespace StepViewer
{
    public partial class Form1 : Form, StepExtractor.IExtractOperationsCallback
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void открытьSTEPФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Получение пути к STEP файлу при помощи диалога открытия файла
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //В случае успешного открытия, запускается выполнение всех необходимых операций
                initParser(openFileDialog1.FileName);
            }
        }

        private void initParser(string filePath)
        {
            //Инициализация объекта для работы со STEP файлом
            StepExtractor.StepExtractor stepExtractor = new StepExtractor.StepExtractor(filePath, this);

            toolStripProgressBar1.Value = 0;

            //Запуск процесса парсинга STEP файла. Так как процесс асинхронный, за ним можно наблюдать при помощи коллбэков.
            stepExtractor.startExtraction();
        }

        public void extractionComplete(StepExtractor.EntityModels.IEntityModel rootEntity, TreeNode rootNode)
        {
            //В случае успешного завершения операций над STEP файлом, в GUI потоке очищается treeView и заполняется новыми данными
            statusStrip1.Invoke(new Action(delegate()
            {
                treeView1.Nodes.Clear();
                treeView1.Nodes.Add(rootNode);
                toolStripProgressBar1.Value = 100;
            }));
        }

        public void extractionFailed(string message)
        {
            //В случае неудачного завершения операции, выводится сообщение об ошибке.
            statusStrip1.Invoke(new Action(delegate()
            {
                MessageBox.Show(message);
                toolStripProgressBar1.Value = 0;
            }));
        }

        public void extractionStep(int completePercent)
        {
            //Промежуточный результат работы парсера
            statusStrip1.Invoke(new Action(delegate() { toolStripProgressBar1.Value = completePercent; }));
        }

        private void помощьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"
    Для того, чтобы увидеть, какие сущности представлены в STEP файле:
1. Нажмите пункт меню Файл
2. Выберите Открыть STEP файл
3. Выбирете файл с расширением .STEP на диске
4. Начнется процедура извлечения информации
5. По завершении извлечения информации, будет отображено дерево сущностей.

При возникновении других вопросов, свяжитесь с разработчиком 
(контактные данные указаны в пункте меню О программе)
", "Пошаговая инструкция");
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Разработано Соловьевым Романом, студентом группы 10702412 БНТУ. 2016. E-mail: ihandy@ya.ru", "О программе");
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
