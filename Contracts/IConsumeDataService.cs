using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Contracts
{
    /// <summary>
    /// 门店端的消费信息服务
    /// </summary>
    public interface IConsumeDataService
    {
        IList<t_rm_payflow> GetItems(string name, int index, decimal? amount, IList<t_bd_item_info> sendData);

    }


    public class t_rm_payflow
    {

        /// <summary>
        /// 
        /// </summary>
        public string coin_no { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal? coin_rate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal? pay_amount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string memo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? error_time { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string error_info { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string pos_no { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal? flow_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal? sale_amount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string pay_way { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string sell_way { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string card_no { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal com_no { get; set; }
    }
}
