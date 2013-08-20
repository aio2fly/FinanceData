using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;

namespace FinanceData.Models
{
    public class GoogleDataModel : AbstractDataModel
    {
        public override bool MakeRequest()
        {
            /* формируем url */
            string url = "http://www.google.com/finance/historical?q=" + Quote + "&output=csv";

            /* Добавляем даты в запрос, если нужно */
            if (DateFrom > 0)
                url += "&startdate=" + HttpUtility.UrlEncode(ConvertFromUnixTimestamp(DateFrom).ToString("MMM d, yyyy"));
            if (DateTo > 0)
                url += "&enddate=" + HttpUtility.UrlEncode(ConvertFromUnixTimestamp(DateTo).ToString("MMM d, yyyy"));

            /* делаем запрос */
            WebClient client = new WebClient();
            try
            {
                ProviderAnswer = client.DownloadString(url); /*Скачиваем данные в csv формате */
            }
            catch (WebException e) /* Google возвращает http код ошибки при неудачном запросе */
            {
                Error = "Символ \"" + Quote + "\" не обнаружен на сервере";
                return false;
            }

            /* Парсим данные */
            CsvParse(ProviderAnswer);

            return true;
        }
    }
}