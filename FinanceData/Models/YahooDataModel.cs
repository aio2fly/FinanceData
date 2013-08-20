using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace FinanceData.Models
{
    public class YahooDataModel : AbstractDataModel
    {
        public override bool MakeRequest()
        {
            /* формируем url */
            string url = "http://ichart.yahoo.com/table.csv?s=" + Quote;

            /* Добавляем даты в запрос, если нужно */
            /* Для Yahoo отсчет месяцев начинается с 0, отнимаем 1 день в процессе */
            if (DateFrom > 0)
            {
                DateTime temp = ConvertFromUnixTimestamp(DateFrom);
                url += "&a=" + (temp.Month - 1) + "&b=" + temp.Day + "&c=" + temp.Year;
            }
            if (DateTo > 0)
            {
                DateTime temp = ConvertFromUnixTimestamp(DateTo);
                url += "&d=" + (temp.Month - 1) + "&e=" + temp.Day + "&f=" + temp.Year;
            }
            url += "&ignore=.csv";

            /* делаем запрос */
            WebClient client = new WebClient();
            try
            {
                ProviderAnswer = client.DownloadString(url); /*Скачиваем данные в csv формате */
            }
            catch (WebException e)
            {
                Error = "Символ \"" + Quote + "\" не обнаружен на сервере"; /* Yahoo возвращает http код ошибки при неудачном запросе */
                return false;
            }

            /* Парсим данные */
            CsvParse(ProviderAnswer);

            return true;
        }
    }
}