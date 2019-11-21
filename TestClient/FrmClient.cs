using Contracts;
using CSock;
using CSock.DynamicProxy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestClient
{
    public partial class FrmClient : Form
    {
        private SockClient _client = null;
        private string _businessId = "A0001";

        public FrmClient()
        {
            InitializeComponent();
            _client = new SockClient(_businessId);
            _client.OnServerInvoking += _client_OnRemoteInvoking;
            _client.OnConnected += _client_OnConnected;
            _client.OnClosed += _client_OnClosed;
        }

        private void _client_OnRemoteInvoking(object sender, InvokeMessageEventArgs e)
        {
            var routeData = e.InvokeMessage.Action.Split(new char[] { ':' });
            var serviceName = routeData.First();
            var actionName = routeData.Last();
            var service = Program._container.Resolve(serviceName, typeof(Object));
            var method = service.GetType().GetMethod(actionName);
            var args = new List<Object>();
            var parameters = JArray.Parse(e.InvokeMessage.MessageBody);
            var i = 0;
            foreach (var p in method.GetParameters())
            {
                if (p.ParameterType == typeof(string))
                {
                    var val = parameters[i].Type == JTokenType.Null ? null : parameters[i].ToString();
                    args.Add(val);
                }
                else
                {
                    args.Add(JsonConvert.DeserializeObject(parameters[i].ToString(), p.ParameterType));
                }
                i++;
            }
            var retValue = method.Invoke(service, args.ToArray());
            e.ReturnData = retValue;
        }

        private void _client_OnClosed(object sender, EventArgs e)
        {
            var client = (sender as SockClient);
            DisplayMsg(string.Format("{0}连接断开！", client.ClientID));
            var existClient = sockClients.FirstOrDefault(x => string.Equals(x.ClientID, client.ClientID));
            if (existClient != null)
            {
                sockClients.Remove(existClient);
            }
            var address = new IPEndPoint(IPAddress.Parse(txtServer.Text), 8012);
            while (!client.Connected)
            {
                Thread.Sleep(5000);
                client.Connect(address);
            }
        }

        private void _client_OnConnected(object sender, EventArgs e)
        {
            DisplayMsg(string.Format("{0}连接成功！", (sender as SockClient).ClientID));
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (_client.Connected) { return; }
            _client.Connect(new IPEndPoint(IPAddress.Parse(txtServer.Text), 8012));
        }

        private void btnSend_Click(object sender, EventArgs e)
        {

        }

        private void btnInvoke_Click(object sender, EventArgs e)
        {
            txtMsg.Clear();
            var service = ProxyObjectFactory.GetInstance().CreateInterfaceProxyWithoutTarget<IItemDataService>(_client);
            var list = new List<t_rm_payflow>();
            decimal? a = null;
            var cnt = Convert.ToInt32(numCount.Value);
            var ticks = Convert.ToInt32(numTicks.Value);
            var getCount = Convert.ToInt32(numGet.Value);
            for (int i = 0; i < cnt; i++)
            {
                list.Add(new t_rm_payflow() { card_no = "card_no" + i.ToString(), coin_no = "RMB", coin_rate = 1.0M, com_no = 0, flow_id = i, memo = "memo" + i.ToString(), pay_way = "RMB", pos_no = "pos_no" + i.ToString(), sell_way = "A", pay_amount = new Random(3).Next(10, 10000), sale_amount = new Random(5).Next(10, 10000), error_info = "error_info" + i.ToString(), error_time = DateTime.Now });
            }
            for (int i = 0; i < 1; i++)
            {
                var data = service.GetItems(getCount, "test" + i.ToString(), list);
                if (data != null)
                {
                    DisplayMsg(string.Format("Client第{0}次调用，获取到：{1}条数据!", (i + 1).ToString(), data.Count));
                }
                else
                {
                    DisplayMsg(string.Format("ERROR:Client第{0}次调用，获取失败!", (i + 1).ToString()));
                }
            }
        }

        private void DisplayMsg(string msg)
        {
            if (this.InvokeRequired) { this.Invoke(new Action<string>(this.DisplayMsg), new object[] { msg }); }
            else
            {
                txtMsg.AppendText(string.Format("[{0}]-{1}{2}", DateTime.Now.ToLongTimeString(), msg, Environment.NewLine));
                txtMsg.ScrollToCaret();
            }
        }

        private IList<SockClient> sockClients = new List<SockClient>();
        private void btnConnect2_Click(object sender, EventArgs e)
        {
            var address = new IPEndPoint(IPAddress.Parse(txtServer.Text), 8012);
            for (int i = 0; i < Convert.ToInt32(numClientCount.Value); i++)
            {
                var client = new SockClient(string.Format("B000{0}", (i + 1).ToString()));
                client.OnServerInvoking += _client_OnRemoteInvoking;
                client.OnConnected += _client_OnConnected;
                client.OnClosed += _client_OnClosed;
                client.Connect(address);
                sockClients.Add(client);
            }
        }

        private void btnInvokeInOrder_Click(object sender, EventArgs e)
        {
            txtMsg.Clear();
            var cnt = Convert.ToInt32(numCount.Value);
            var ticks = Convert.ToInt32(numTicks.Value);
            var getCount = Convert.ToInt32(numGet.Value);
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(sockClients, (client) =>
                {
                    var list = GetTestData(client.ClientID, cnt);
                    var service = ProxyObjectFactory.GetInstance().CreateInterfaceProxyWithoutTarget<IItemDataService>(client);
                    //for (int i = 0; i < cnt; i++)
                    //{
                    //    list.Add(new t_rm_payflow() { card_no = "card_no" + i.ToString(), coin_no = "RMB", coin_rate = 1.0M, com_no = 0, flow_id = i, memo = "memo" + i.ToString(), pay_way = "RMB", pos_no = "pos_no" + i.ToString(), sell_way = "A", pay_amount = new Random(3).Next(10, 10000), sale_amount = new Random(5).Next(10, 10000), error_info = "error_info" + i.ToString(), error_time = DateTime.Now });
                    //}
                    var sw = new Stopwatch();
                    for (int i = 0; i < ticks; i++)
                    {
                        sw.Restart();
                        var data = service.GetItems(getCount, "test" + i.ToString(), list);
                        sw.Stop();
                        if (data != null)
                        {
                            DisplayMsg(string.Format("{2}第{0}次调用Server，获取到：{1}条数据，耗时：{3}ms!", (i + 1).ToString(), data.Count, client.ClientID, sw.ElapsedMilliseconds));
                        }
                        else
                        {
                            DisplayMsg(string.Format("ERROR:{1}第{0}次调用，获取失败，耗时：{2}ms!", (i + 1).ToString(), client.ClientID, sw.ElapsedMilliseconds));
                        }
                        Thread.Sleep(300);
                    }
                });
            });
            //foreach (var client in sockClients)
            //{
            //    var service = ServerSockProxy.CreateProxy<IItemDataService>(client);
            //    for (int i = 0; i < cnt; i++)
            //    {
            //        list.Add(new t_rm_payflow() { card_no = "card_no" + i.ToString(), coin_no = "RMB", coin_rate = 1.0M, com_no = 0, flow_id = i, memo = "memo" + i.ToString(), pay_way = "RMB", pos_no = "pos_no" + i.ToString(), sell_way = "A", pay_amount = new Random(3).Next(10, 10000), sale_amount = new Random(5).Next(10, 10000), error_info = "error_info" + i.ToString(), error_time = DateTime.Now });
            //    }
            //    for (int i = 0; i < ticks; i++)
            //    {
            //        var data = service.GetItems(getCount, "test" + i.ToString(), list);
            //        if (data != null)
            //        {
            //            DisplayMsg(string.Format("{2}第{0}次调用，获取到：{1}条数据!", (i + 1).ToString(), data.Count, client.ClientID));
            //        }
            //        else
            //        {
            //            DisplayMsg(string.Format("ERROR:{1}第{0}次调用，获取失败!", (i + 1).ToString(), client.ClientID));
            //        }
            //    }
            //}
        }

        private List<t_rm_payflow> GetTestData(string clientId, int cnt)
        {
            var list = new List<t_rm_payflow>();
            for (int i = 0; i < cnt; i++)
            {
                list.Add(new t_rm_payflow() { card_no = clientId + "_" + i.ToString(), coin_no = "RMB", coin_rate = 1.0M, com_no = 0, flow_id = i, memo = "memo" + i.ToString(), pay_way = "RMB", pos_no = "pos_no" + i.ToString(), sell_way = "A", pay_amount = new Random(3).Next(10, 10000), sale_amount = new Random(5).Next(10, 10000), error_info = "error_info" + i.ToString(), error_time = DateTime.Now });
            }
            return list;
        }

        private void btnDisconnectAll_Click(object sender, EventArgs e)
        {
            foreach (var item in sockClients)
            {
                item.Close();
            }
            sockClients.Clear();
        }
    }
}
