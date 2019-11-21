using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contracts
{
    /// <summary>
    /// 总部端的档案信息服务
    /// </summary>
    public interface IItemDataService
    {
        IList<t_bd_item_info> GetItems(decimal idx, string name, IList<t_rm_payflow> testData);
    }


    public class t_bd_item_info
    {

        /// <summary>
        /// 
        /// </summary>
        public string card_discount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string item_subno { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string item_clsno { get; set; }

        /// <summary>
        /// 商品类型
        /// 0:普通商品
        /// 1:捆绑商品
        /// 2:制单拆分
        /// 3:制单组合
        /// 6:自动转货
        /// 7:自动加工
        /// </summary>
        public string combine_sta { get; set; }

        /// <summary>
        /// 商品状态[0建档,1新品,2正常,3停购,4停售,5淘汰,6删除]
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string display_flag { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Int32? po_cycle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Int32? so_cycle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string en_dis { get; set; }

        /// <summary>
        /// 采购范围[0统配,1自采,2不限,3,直配,4自产]
        /// </summary>
        public string direct { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string change_price { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal? purchase_tax { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal? sale_tax { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal? purchase_spec { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal? shipment_spec { get; set; }

        /// <summary>
        /// 主供应商
        /// </summary>
        public string main_supcust { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal? lose_rate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string item_sup_flag { get; set; }

        /// <summary>
        /// 是否管理库存[0管理/1不管理]
        /// </summary>
        public string item_stock { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string item_counter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string abc { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string branch_price { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string cost_compute { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string vip_acc_flag { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string dpfm_type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal? trans_price { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal? vip_price1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal? vip_price2 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal? base_price4 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal? order_man_rate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string auto_flag { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string main_sale_flag { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal? trial_amt { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string trial_reg { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string branch_sale { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string automin_flag { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string com_flag { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal? vip_price { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal? sale_min_price { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal? base_price1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal? base_price2 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal? base_price3 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal? vip_acc_num { get; set; }

        /// <summary>
        /// 零售价
        /// </summary>
        public decimal sale_price { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal? num1 { get; set; }

        /// <summary>
        /// 商品有效期
        /// </summary>
        public decimal? num2 { get; set; }

        /// <summary>
        /// 计价方式 0：普通，1：计重，2：计数
        /// </summary>
        public decimal? num3 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal? sup_ly_rate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal? return_rate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal price { get; set; }

        /// <summary>
        /// 批发价
        /// </summary>
        public decimal base_price { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? trial_s_date { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? trial_e_date { get; set; }

        /// <summary>
        /// 课组
        /// </summary>
        public string dept_clsno { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal? min_opqty { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? stop_date { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string item_rem { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string branch_no { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string build_man { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string last_modi_man { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string season_no { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string other1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string other2 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string other3 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string item_supcust { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? build_date { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? modify_date { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string item_subname { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string item_brand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string item_brandname { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string unit_no { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string item_size { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string product_area { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string item_no { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string item_name { get; set; }
        /// <summary>
        /// 数据标识[0线下，1web，2在线商务平台]
        /// </summary>
        public string web_flag { get; set; }

        public t_bd_item_info() { web_flag = "1"; }
    }
}
