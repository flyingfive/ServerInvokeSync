using Contracts;
using CSock.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestClient.SocketServices
{
    [SockService]
    public class ConsumeDataService : IConsumeDataService
    {
        public IList<t_rm_payflow> GetItems(string name, int index, decimal? amount, IList<t_bd_item_info> senddata)
        {
            var list = new List<t_rm_payflow>();
            for (int i = 0; i < index; i++)
            {
                list.Add(new t_rm_payflow() { card_no = "card_no" + i.ToString(), coin_no = "RMB", coin_rate = 1.0M, com_no = 0, flow_id = i, memo = "memo" + i.ToString(), pay_way = "RMB", pos_no = "pos_no" + i.ToString(), sell_way = "A", pay_amount = new Random(3).Next(10, 10000), sale_amount = new Random(5).Next(10, 10000), error_info = "error_info" + i.ToString(), error_time = DateTime.Now });
            }
            return list;
        }
    }
}
