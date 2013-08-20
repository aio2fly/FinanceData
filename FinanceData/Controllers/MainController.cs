using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using FinanceData.Models;

namespace FinanceData.Controllers
{
    public class MainController : Controller
    {
        /* Начальная страница */
        public ActionResult Index()
        {
            return View();
        }

        /* Обработчик Ajax-запросов */
        public JsonResult GetData(AbstractDataModel model, string quote, int datefrom, int dateto)
        {
            /* Создаем экземпляр класса, предоставляющего ответ клиенту */
            JsonAnswer answer = new JsonAnswer();

            /* Настраиваем параметры запроса и выполняем, если запрос выполнен успешно, собираем данные в ответ */
            if (model.SetParams(quote, datefrom, dateto) && model.MakeRequest())
            {
                answer.Status = "OK";
                answer.Result = model.GetAnswer();
            }
            /* Иначе в ответ записываем ошибку */
            else
            {
                answer.Status = "Error";
                answer.Error = model.GetError();
            }

            /* Возвращаем клиенту данные в формате JSON */
            return Json(answer, JsonRequestBehavior.AllowGet);
        }

    }
}
