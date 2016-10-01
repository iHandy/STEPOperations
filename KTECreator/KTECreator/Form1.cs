using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KTECreator.Sections;

namespace KTECreator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public void showResult(List<Section> list)
        {

            textBox1.Invoke(new Action(
            () =>
            {
                textBox1.Text = "";
                foreach (var item in list)
                {
                    textBox1.Text += item.Position + " - " + item.getNumber() + "(" + item.getName() + ")"
                        //+ " [" + Math.Round(item.StartPoint.X, 4) + ", " + Math.Round(item.StartPoint.Y, 4) + ", " + Math.Round(item.StartPoint.Z, 4) + "]"
                        //+ "; " + " [" + Math.Round(item.EndPoint.X, 4) + ", " + Math.Round(item.EndPoint.Y, 4) + ", " + Math.Round(item.EndPoint.Z, 4) + "]"
                        + Environment.NewLine;
                }
            }
            ));

        }

        private void Form1_DoubleClick(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void buildToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Запуск работы всех алгоритмов
            SectionsBuilder sb = new SectionsBuilder(this);
            sb.build();
        }

        internal void showMatrix(bool[,] matrix)
        {
            dataGridView1.Invoke(new Action(() =>
            {
                //Очистка dataGridView
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();

                //Инициализация dataGridView
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    dataGridView1.Columns.Add((i + 1).ToString(), (i + 1).ToString());
                    dataGridView1.Columns[i].Width = 40;
                }
                dataGridView1.Rows.Add(matrix.GetLength(0));
                for (int j = 0; j < matrix.GetLength(0); j++)
                {
                    dataGridView1.Rows[j].HeaderCell.Value = (j + 1).ToString();
                    for (int k = 0; k < matrix.GetLength(0); k++)
                    {
                        dataGridView1.Rows[j].Cells[k].Value = matrix[k, j] ? "1" : "";
                        if (k <= j)
                        {
                            dataGridView1.Rows[j].Cells[k].Style.BackColor = Color.Gray;
                        }
                    }

                }
            }));
        }
    }
}
