using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinanceData.Models
{
    public class RandomDataModel : AbstractDataModel
    {
        public override bool MakeRequest()
        {
            DateTime FirstDate = ConvertFromUnixTimestamp(DateFrom);
            DateTime SecondDate = ConvertFromUnixTimestamp(DateTo);

            Answer = new List<string[]>();
            /* заполняем заголовок таблицы*/
            string[] table_header = {"Дата","Открытие", "Закрытие", "Максимум", "Минимум", "Объем"};
            Answer.Add(table_header);

            /* Рандомом создаем данные */
            Random rnd = new Random();
            double value = Math.Round(rnd.Next(20, 80) + rnd.NextDouble(),2);

            SecondDate = SecondDate.AddDays(1);
            do
            {
                SecondDate = SecondDate.AddDays(-1);
                value = Math.Round(Math.Abs(value + rnd.Next(-4, 5)), 2);

                string[] line = { SecondDate.Day + "-" + SecondDate.Month + "-" + SecondDate.Year, 
                                  Math.Round(Math.Abs(value + rnd.Next(0, 2)+ rnd.NextDouble()),2).ToString(), 
                                  Math.Round(Math.Abs(value + rnd.Next(0, 2) + rnd.NextDouble()),2).ToString(), 
                                  Math.Round(Math.Abs(value + rnd.Next(0,4)+ rnd.NextDouble()),2).ToString(), 
                                  Math.Round(Math.Abs(value + rnd.Next(-2,0)),2).ToString(),
                                  rnd.Next(12000000,40000000).ToString()};
                Answer.Add(line);
            }
            while (!CompareDates(FirstDate, SecondDate));

            return true;
        }

        /* функция сравнения двух дат без времени*/
        private bool CompareDates(DateTime a, DateTime b)
        {
            if (a.Year == b.Year && a.Month == b.Month && a.Day == b.Day)
                return true;
            else
                return false;
        }
    }
}