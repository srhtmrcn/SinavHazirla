using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SinavHazirla
{
    public class LogFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var Req = HttpContext.Current == null ? null : HttpContext.Current.Request;
            var pi = Hizli.GetPersonel();

            if (Req != null && pi.ID > 0)
            {
                var ack = Req.UserHostAddress + " " + Req.RawUrl.ToString();
                var db = new DatabaseContext();
                db.Log.Add(new Log()
                {
                    Aciklama = ack,
                    PersonelID = pi.ID,
                    Tarih = DateTime.Now
                });
                db.SaveChanges();
            }

            base.OnActionExecuting(filterContext);
        }
    }
}