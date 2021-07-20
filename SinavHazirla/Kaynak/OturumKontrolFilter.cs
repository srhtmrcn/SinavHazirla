using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SinavHazirla
{
    public class OturumKontrolFilter:ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var aktif = Hizli.GetPersonel();
            // oturum kontrol edildi ve YOK
            if (aktif.ID == 0)
                filterContext.Result = new RedirectResult("/personel/giris");
            
            base.OnActionExecuting(filterContext);
        }
    }
}