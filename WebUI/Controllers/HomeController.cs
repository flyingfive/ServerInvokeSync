using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Contracts;
using Newtonsoft.Json.Linq;
using SSock.DynamicProxy;

namespace WebUI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        private static object _syncObj = new object();

        public JsonResult GetData()
        {
            var total = App_Start.SocketHost.Instance.Server.SessionCount;
            if (total == 0) { return Json(new { result = "没有客户端连接" }, JsonRequestBehavior.AllowGet); }
            var index = new Random(1).Next(0, total);
            var client = App_Start.SocketHost.Instance.Server.GetSessions((s) => { return s.ClientID == ""; }).FirstOrDefault();//.OnlineClients.Values.ToArray()[index];
            if (client == null) { return Json(new { total = 0 }, JsonRequestBehavior.AllowGet); }
            var service = ProxyObjectFactory.GetInstance().CreateInterfaceProxyWithoutTarget<IConsumeDataService>(client.ClientID);
            var name = Request.Params["name"];
            var idx = Convert.ToInt32(Request.Params["index"]);
            var amount = Convert.ToDecimal(Request.Params["amount"]);
            var cnt = Convert.ToInt32(Request.Params["count"]);
            var data = GetTestData(cnt);
            var list = service.GetItems(name, idx, amount, data);
            return Json(new { client = client.ClientID, total = list.Count, rows = list }, JsonRequestBehavior.AllowGet);
        }


        private IList<t_bd_item_info> GetTestData(int cnt)
        {
            var list = new List<t_bd_item_info>();
            for (int i = 0; i < cnt; i++)
            {
                list.Add(new t_bd_item_info()
                {
                    item_no = "item_no" + i.ToString(),
                    item_subno = "item_subno" + i.ToString(),
                    abc = "abc",
                    automin_flag = "0",
                    auto_flag = "0",
                    base_price = 1.234m,
                    base_price1 = 2.45M,
                    base_price2 = 3.45M,
                    base_price3 = 23.56M,
                    branch_no = "0001",
                    branch_price = "0",
                    branch_sale = "1",
                    build_date = DateTime.Now,
                    build_man = "1111",
                    card_discount = "1",
                    change_price = "0",
                    combine_sta = "0",
                    com_flag = "1",
                    cost_compute = "0",
                    dept_clsno = "0102",
                    direct = "1",
                    display_flag = "1",
                    dpfm_type = "1",
                    en_dis = "en_dis",
                    item_brand = "PP",
                    item_brandname = "item_brandname",
                    item_clsno = "item_clsno" + i.ToString(),
                    item_counter = "item_counter" + i.ToString(),
                    item_name = "item_name" + i.ToString(),
                    item_rem = "item_rem " + i.ToString(),
                    item_size = "item_size " + i.ToString(),
                    item_stock = "1",
                    item_subname = "item_subname" + i.ToString(),
                    item_supcust = "1",
                    item_sup_flag = "1",
                    main_sale_flag = "1",
                    main_supcust = "main_supcust" + i.ToString(),
                    num1 = 0M,
                    num2 = 0M,
                    other2 = "other2-" + i.ToString(),
                    product_area = "product_area-" + i.ToString(),
                    purchase_spec = 1.0M,
                    price = 23.5M,
                    sale_min_price = 10M,
                    sale_price = 22M,
                    season_no = "season_no",
                    trans_price = 8.8M,
                    status = "1",
                    unit_no = "unit_no" + i.ToString(),
                    web_flag = "1",
                    vip_price = 11M,
                    vip_price1 = 22M,
                    vip_price2 = 33M,
                    sup_ly_rate = 0.2M,
                    purchase_tax = 1M,
                    sale_tax = 0.1M,
                    vip_acc_flag = "1",
                    vip_acc_num = 0.2M,
                    modify_date = DateTime.Now,
                });
            }
            return list;
        }
    }

}
