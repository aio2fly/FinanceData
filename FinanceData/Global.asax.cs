using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using FinanceData.Models;

namespace FinanceData
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            /* Регистрируем новый ModelBinder */
            ModelBinders.Binders.Add(typeof(AbstractDataModel), new FinanceModelBinder());
        }
    }

    /* FinanceData model binding */
    public class FinanceModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            string Provider = bindingContext.ValueProvider.GetValue("provider").AttemptedValue;

            AbstractDataModel model;

            switch (Provider)
            {
                case "google":
                    model = new GoogleDataModel();
                    break;
                case "yahoo":
                    model = new YahooDataModel();
                    break;
                default:
                    model = new RandomDataModel();
                    break;
            }

            return model;
        }
    }
}