using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Web.Http.ModelBinding;
using System.Text.RegularExpressions;

namespace FinanceData.Models
{
    [ModelBinder(typeof(FinanceModelBinder))]
    abstract public class AbstractDataModel
    {
        protected string Quote = "";
        protected int DateFrom = 0, DateTo = 0;
        protected string ProviderAnswer;
        protected string Error = "";
        protected List<string[]> Answer;

        public string GetError()
        {
            return Error;
        }

        public List<string[]> GetAnswer()
        {
            return Answer;
        }

        /* Установка парамертов - код и даты */
        public bool SetParams(string _Quote, int _DateFrom, int _DateTo)
        {
            Quote = _Quote;
            DateFrom = _DateFrom;
            DateTo = _DateTo;

            return ValidateData();
        }

        /* Проверка на валидность */
        protected bool ValidateData()
        {
            /* Проверяем границы дат */
            if (DateFrom < 0 || DateTo < 0)
            {
                Error = "Неправильный формат даты";     //TODO
                return false;
            }

            if (DateFrom != 0 && DateTo != 0 && DateFrom >= DateTo)
            {
                Error = "Неправильный формат даты";     //TODO
                return false;
            }

            return true;
        }

        /* конвертер из UnixTime в DateTime  */
        protected static DateTime ConvertFromUnixTimestamp(int timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

        /* csv парсер */
        protected void CsvParse(string data)
        {
            /* Парсим ответ 
             * Исползуем простой парсинг csv - разбиение элементов по запятым
             * Так как сервер не возвращает csv вида "abc,",dsd,ds (с кавычками и запятыми внутри элемента)
             */
            Answer = new List<string[]>();

            foreach (string line in Regex.Split(ProviderAnswer, "\n").ToList().Where(s => !string.IsNullOrEmpty(s)))
            {
                string[] values = Regex.Split(line, ",");

                for (int i = 0; i < values.Length; i++)
                    values[i] = values[i].Trim('\"');

                Answer.Add(values);
            }
        }

        /* Функция выполнения запроса, перегружается в классах-наследниках  */
        abstract public bool MakeRequest();
    }

    /* Класс-ответ на ajax-запрос */
    public class JsonAnswer
    {
        public string Status;
        public string Error;
        public List<string[]> Result;
    }
}