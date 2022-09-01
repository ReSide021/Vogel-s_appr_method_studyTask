using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FogelModel
{
    public partial class Form1 : Form
    {
        int countClmn = 0;
        int countRow = 0;
        List<int> endSum = new List<int>();
        bool endScore = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            textBox_info.Clear();
            endScore = false;
            endSum.Clear();
            countClmn = (int)numericUpDownClmn.Value;
            countRow = (int)numericUpDownRow.Value;
            if (countClmn == 0 || countRow == 0) return;
            while(dataGridView.Rows.Count > 0)
            {
                dataGridView.Rows.Clear();
            }
            while(dataGridView.Columns.Count > 0)
            {
                dataGridView.Columns.Clear();
            }
            for(int i = 0; i <= numericUpDownClmn.Value; i++)
            {
                if(i == 0)
                {
                    dataGridView.Columns.Add($"clmn{i}", "");
                }
                else
                {
                    dataGridView.Columns.Add($"clmn{i}", $"Заказчик {i}");
                }
            }
            for (int i = 0; i <= numericUpDownRow.Value; i++)
            {
                dataGridView.Rows.Add();
            }
            dataGridView[0, 0].Value = "Запасы\\Требуется";
        }

        private async void button_solve_Click(object sender, EventArgs e)
        {
            int stocks = 0;
            for(int i = 1; i <= numericUpDownRow.Value; i++)
            {
                stocks += Convert.ToInt32(dataGridView[0, i].Value);
            }
            int wanted = 0;
            for(int i = 1; i <= numericUpDownClmn.Value; i++)
            {
                wanted += Convert.ToInt32(dataGridView[i, 0].Value);
            }
            //seekMaxinMin();
            if (stocks != wanted)
            {
                MessageBox.Show("Задача не является закрытым типом");
            }
            else
            {
                while (!endScore)
                {
                    await Task.Delay(1000);
                    await seekMaxinMin();
                }
            }
        }


        private void PrintEndSum(List<int> endSum)
        {
            int summa = 0;
            foreach(var el in endSum)
            {
                summa += el;
            }
            textBox_info.Text += $"\r\n\r\nИТОГОВАЯ СУММА: {summa}\r\n\r\n";
        }

        private async Task<int> seekMaxinMin()
        {
            textBox_info.Text += $"\r\n\r\n____________Запуск новой итерации____________\r\n\r\n";
            // создание новых столбца и строки для минимумов
            var newClmn = dataGridView.Columns.Count;
            var newRow = dataGridView.Rows.Count;
            dataGridView.Columns.Add($"clmn{newClmn}", "Разность");
            dataGridView.Rows.Add();
            dataGridView[0, newRow].Value = "Разность";

            List<int> listDiffNum = new List<int>();

            // заполение новой строки
            for(int i = 1; i <= countClmn; i++)
            {
                List<int> numInClmn = new List<int>();
                bool end = false;
                for(int j = 1; j <= countRow; j++)
                {
                    if(dataGridView[i, j].Style.BackColor == Color.Empty)
                    {
                        var num = Convert.ToInt32(dataGridView[i, j].Value);
                        numInClmn.Add(num);
                       // textBox_info.Text += $"Строка {j} зафиксирована\r\n";
                    }
                    if (j == countRow) end = true;
                }
                if(end)
                {
                    if(numInClmn.Count > 1)
                    {
                        var min1 = numInClmn.Min();
                        numInClmn.Remove(min1);
                        var min2 = numInClmn.Min();
                        var res = min2 - min1;
                        dataGridView[i, newRow].Value = res;
                        listDiffNum.Add(res);
                        textBox_info.Text += $"Разность минимальных значений в столбце {i} = {res}\r\n";
                    } 
                    else if(numInClmn.Count == 1)
                    {
                        for (int j = 1; j <= countRow; j++)
                        {
                            if (dataGridView[i, j].Style.BackColor == Color.Empty)
                            {
                                var num = Convert.ToInt32(dataGridView[i, j].Value);
                                var store = Convert.ToInt32(dataGridView[0, j].Value);
                                var needed = Convert.ToInt32(dataGridView[i, 0].Value);
                                if (store <= needed)
                                {
                                    int res = num * store;
                                    endSum.Add(res);
                                    textBox_info.Text += $"Добавлено итоговое число {res}\r\n";
                                    await Task.Delay(1500);
                                    dataGridView[0, j].Value = "0";
                                    dataGridView[0, j].Style.ForeColor = Color.Yellow;
                                    dataGridView[i, j].Style.BackColor = Color.LightPink;
                                    dataGridView[i, 0].Value = needed - store;
                                    if(store == needed)
                                    {
                                        dataGridView[i, 0].Style.ForeColor = Color.Green;
                                    }
                                }
                                else
                                {
                                    int res = num * needed;
                                    endSum.Add(res);
                                    textBox_info.Text += $"Добавлено итоговое число {res}\r\n";
                                    await Task.Delay(1500);
                                    dataGridView[0, j].Value = store - needed;
                                    dataGridView[i, 0].Style.ForeColor = Color.Green;
                                    dataGridView[i, j].Style.BackColor = Color.LightPink;
                                }
                                endScore = true;
                                PrintEndSum(endSum);
                            }
                        }
                    }
                }
            }


            // заполнение нового столбца
            for(int i = 1; i <= countRow; i++)
            {
                List<int> numInRow = new List<int>();
                bool end = false;
                for (int j = 1; j <= countClmn; j++)
                {
                    if(dataGridView[j, i].Style.BackColor == Color.Empty)
                    {
                        var num = Convert.ToInt32(dataGridView[j, i].Value);
                        numInRow.Add(num);
                       // textBox_info.Text += $"Столбец {j} зафиксирован\r\n";
                    }
                    if (j == countClmn) end = true;
                }
                if(end)
                {
                    if(numInRow.Count > 1)
                    {
                        var min1 = numInRow.Min();
                        numInRow.Remove(min1);
                        var min2 = numInRow.Min();
                        var res = min2 - min1;
                        dataGridView[newClmn, i].Value = res;
                        listDiffNum.Add(res);
                        textBox_info.Text += $"Разность минимальных значений в строке {i} = {res}\r\n";
                    }
                    else if(numInRow.Count == 1)
                    {
                        for (int j = 1; j <= countClmn; j++)
                        {
                            if (dataGridView[j, i].Style.BackColor == Color.Empty)
                            {
                                var num = Convert.ToInt32(dataGridView[j, i].Value);
                                var store = Convert.ToInt32(dataGridView[0, i].Value);
                                var needed = Convert.ToInt32(dataGridView[j, 0].Value);
                                if(store <= needed)
                                {
                                    int res = num * store;
                                    endSum.Add(res);
                                    textBox_info.Text += $"Добавлено итоговое число {res}\r\n";
                                    await Task.Delay(1500);
                                    dataGridView[0, i].Value = "0";
                                    dataGridView[0, i].Style.ForeColor = Color.Yellow;
                                    dataGridView[j, i].Style.BackColor = Color.LightPink;
                                    dataGridView[j, 0].Value = needed - store;
                                    if(store == needed)
                                    {
                                        dataGridView[j, 0].Style.ForeColor = Color.Green;
                                    }
                                }
                                else
                                {
                                    int res = num * needed;
                                    endSum.Add(res);
                                    textBox_info.Text += $"Добавлено итоговое число {res}\r\n";
                                    await Task.Delay(1500);
                                    dataGridView[0, i].Value = store - needed;
                                    dataGridView[j, 0].Style.ForeColor = Color.Green;
                                    dataGridView[j, i].Style.BackColor = Color.LightPink;
                                }
                                endScore = true;
                                PrintEndSum(endSum);
                            }
                        }
                        
                    }
                }
            }

            if (endScore)
            {
                dataGridView.Rows.RemoveAt(newRow);
                dataGridView.Columns.RemoveAt(newClmn);
                return 0;
            }

            for (int i = 1; i <= countRow; i++)
            {
                if (dataGridView[newClmn, i].Style.BackColor == Color.LightPink) continue;
                if (Convert.ToInt32(dataGridView[newClmn, i].Value) == listDiffNum.Max())
                {
                    textBox_info.Text += $"Найден максимум в столбце разностей [{newClmn},{i}]\r\n";
                    dataGridView[newClmn, i].Style.BackColor = Color.LightGreen;
                    textBox_info.Text += $"Запуск поиска минимума в строке {i}\r\n";
                    seekRowOrClmn(true, i);
                    return 0;
                }
            }
            for (int i = 1; i <= countClmn; i++)
            {
                if (dataGridView[i, newRow].Style.BackColor == Color.LightPink) continue;
                if (Convert.ToInt32(dataGridView[i, newRow].Value) == listDiffNum.Max())
                {
                    textBox_info.Text += $"Найден максимум в строке разностей [{i},{newRow}]\r\n";
                    dataGridView[i, newRow].Style.BackColor = Color.LightGreen;
                    textBox_info.Text += $"Запуск поиска минимума в столбце {i}\r\n";
                    seekRowOrClmn(false, i);
                    return 0;
                }
            }
            return 0;
        }

        private void seekRowOrClmn(bool IsSeekRow, int coord)
        {
            if (IsSeekRow) // максимум в столбце справа - ищем строку
            {
                List<int> tempList = new List<int>();
                for(int i = 1; i <= countClmn; i++)
                {
                    if (dataGridView[i, coord].Style.BackColor == Color.LightPink) continue;
                    var num = Convert.ToInt32(dataGridView[i, coord].Value);
                    tempList.Add(num);
                    //textBox_info.Text += $"В tempList добавлено число {num} [{i}, {coord}]\r\n";
                }
                int saveI = 0;
                string type = "";
                for (int i = 1; i <= countClmn; i++)
                {
                    if (dataGridView[i, coord].Style.BackColor == Color.LightPink) continue;
                    if(Convert.ToInt32(dataGridView[i, coord].Value) == tempList.Min())
                    {
                        var num = Convert.ToInt32(dataGridView[i, coord].Value);
                        var store = Convert.ToInt32(dataGridView[0, coord].Value);
                        var needed = Convert.ToInt32(dataGridView[i, 0].Value);
                        if(needed <= store)
                        {
                            int res = num * needed;
                            endSum.Add(res);
                            textBox_info.Text += $"Добавлено итоговое число {res}\r\n";
                            dataGridView[0, coord].Value = store - needed;
                            dataGridView[i, 0].Style.ForeColor = Color.Green;
                            dataGridView[i, 0].Value = "0";
                            saveI = i;
                            type = "fullneeded";
                        }
                        else
                        {
                            int res = num * store;
                            endSum.Add(res);
                            textBox_info.Text += $"Добавлено итоговое число {res}\r\n";
                            dataGridView[0, coord].Value = "0";
                            dataGridView[0, coord].Style.ForeColor = Color.Yellow;
                            dataGridView[i, 0].Value = needed - store;
                            type = "nonfullneeded";
                        }
                        PrintEndSum(endSum);
                        break;
                    }
                }
                if(type == "fullneeded")
                {
                    if (saveI != 0)
                    {
                        textBox_info.Text += $"Номер закрытого столбца: {saveI}\r\n";
                        for (int i = 1; i <= countRow; i++)
                        {
                            dataGridView[saveI, i].Style.BackColor = Color.LightPink;
                           // textBox_info.Text += $"Закрыта секция (столбец) [{saveI},{i}]\r\n";
                        }
                    }
                }
                if(type == "nonfullneeded")
                {
                    if (dataGridView[0, coord].Value.ToString() == "0")
                    {
                        textBox_info.Text += $"Номер закрытой строки: {coord}\r\n";
                        for (int i = 1; i <= countClmn; i++)
                        {
                            dataGridView[i, coord].Style.BackColor = Color.LightPink;
                            //textBox_info.Text += $"Закрыта секция (строка) [{i},{coord}]\r\n";
                        }
                    }
                }
            }
            else
            {
                List<int> tempList = new List<int>();
                for (int i = 1; i <= countRow; i++)
                {
                    if (dataGridView[coord, i].Style.BackColor == Color.LightPink) continue;
                    var num = Convert.ToInt32(dataGridView[coord, i].Value);
                    tempList.Add(num);
                    //textBox_info.Text += $"В tempList добавлено число {num} [{coord}, {i}]\r\n";
                }
                int saveI = 0;
                string type = "";
                for (int i = 1; i <= countRow; i++)
                {
                    if (dataGridView[coord, i].Style.BackColor == Color.LightPink) continue;
                    if (Convert.ToInt32(dataGridView[coord, i].Value) == tempList.Min())
                    {
                        var num = Convert.ToInt32(dataGridView[coord, i].Value);
                        var store = Convert.ToInt32(dataGridView[0, i].Value);
                        var needed = Convert.ToInt32(dataGridView[coord, 0].Value);
                        if (needed <= store)
                        {
                            int res = num * needed;
                            endSum.Add(res);
                            textBox_info.Text += $"Добавлено итоговое число {res}\r\n";
                            dataGridView[0, i].Value = store - needed;
                            dataGridView[coord, 0].Style.ForeColor = Color.Green;
                            dataGridView[coord, 0].Value = "0";
                            saveI = i;
                            type = "fullneeded";
                        }
                        else
                        {
                            int res = num * store;
                            endSum.Add(res);
                            textBox_info.Text += $"Добавлено итоговое число {res}\r\n";
                            dataGridView[0, i].Value = "0";
                            dataGridView[0, i].Style.ForeColor = Color.Yellow;
                            dataGridView[coord, 0].Value = needed - store;
                            saveI = i;
                            type = "nonfullneeded";
                        }
                        PrintEndSum(endSum);
                        break;
                    }
                }
                if(type == "nonfullneeded")
                {
                    if (saveI != 0)
                    {
                        textBox_info.Text += $"Номер закрытой строки: {saveI}\r\n";
                        if (dataGridView[0, saveI].Value.ToString() == "0")
                        {
                            for (int i = 1; i <= countClmn; i++)
                            {
                                dataGridView[i, saveI].Style.BackColor = Color.LightPink;
                                textBox_info.Text += $"Закрыта секция (строка) [{i},{coord}]\r\n";
                            }
                        }
                    }
                }
                if(type == "fullneeded")
                {
                    textBox_info.Text += $"Номер закрытого столбца: {coord}\r\n";
                    for (int i = 1; i <= countRow; i++)
                    {
                        dataGridView[coord, i].Style.BackColor = Color.LightPink;
                        textBox_info.Text += $"Закрыта секция (столбец) [{saveI},{i}]\r\n";
                    }
                }
            }
        }
    }
}
