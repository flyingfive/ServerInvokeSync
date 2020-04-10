using Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SSock;
using SSock.DynamicProxy;
using SSock.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestServer
{
    public partial class FrmServer : Form
    {
        private SockServer _server = null;

        public FrmServer()
        {
            InitializeComponent();
            _server = SockServerFactory.Build();
            _server.Started += _server_Started;
            _server.OnClientInvoking += _server_OnRemoteInvoking;
            _server.NewSessionConnected += _server_NewSessionConnected;
            _server.OnMessageReceived += _server_OnMessageReceived;
            //_server.SessionClosed += _server_SessionClosed;
            _server.OnClientClosed += _server_OnClientClosed;
            _server.OnClientIdentified += _server_OnClientIdentified;
        }

        private void _server_OnClientIdentified(object sender, ClientSocketEventArgs e)
        {
            UpdateClientInfoOnUI(e.Client);
        }

        private void UpdateClientInfoOnUI(SocketClientInfo client)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<SocketClientInfo>(this.UpdateClientInfoOnUI), new object[] { client });
            }
            else
            {
                var idx = lbClients.FindString(string.Format("{0}|{1}", client.SessionId, client.RemoteAddress));
                if (idx != ListBox.NoMatches)
                {
                    lbClients.Items.RemoveAt(idx);
                    lbClients.Items.Insert(idx, string.Format("{0}|{1}|{2}", client.ClientId, client.SessionId, client.RemoteAddress));
                }
            }
        }

        private void _server_OnRemoteInvoking(object sender, InvokeMessageEventArgs e)
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

        private void _server_OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {

        }

        private void _server_SessionClosed(SockSession session, SuperSocket.SocketBase.CloseReason value)
        {
            var item = lbClients.Items.OfType<string>().FirstOrDefault(x => x.StartsWith(session.SessionID));
            if (item != null)
            {
                DisplayClients(item, false);
            }
            DisplayMsg(string.Format("客户端：{0}断开连接，原因：{1}", item, value.ToString()));
        }

        private void _server_OnClientClosed(object sender, ClientClosedEventArgs e)
        {
            RemoveClient(e.Client, e.CloseReason);
        }

        private void RemoveClient(SocketClientInfo client, SuperSocket.SocketBase.CloseReason reason)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<SocketClientInfo, SuperSocket.SocketBase.CloseReason>(this.RemoveClient), new object[] { client, reason });
            }
            else
            {
                var key = string.Format("{0}|{1}", client.SessionId, client.RemoteAddress);
                var idx = lbClients.FindString(key);
                if (idx == ListBox.NoMatches)
                {
                    key = string.Format("{0}|{1}|{2}", client.ClientId, client.SessionId, client.RemoteAddress);
                    idx = lbClients.FindString(key);
                }
                if (idx != ListBox.NoMatches)
                {
                    DisplayClients(key, false);
                }
                DisplayMsg(string.Format("客户端：{0}断开连接，原因：{1}", client.ClientId, reason.ToString()));
            }
        }

        private void _server_NewSessionConnected(SockSession session)
        {
            if (session.Connected)
            {
                DisplayClients(string.Format("{0}|{1}", session.SessionID, session.RemoteEndPoint.ToString()), true);
            }
        }

        private void _server_Started(object sender, EventArgs e)
        {
            DisplayMsg("服务启动成功！");
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            _server.Start();
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

        private void btnInvoke_Click(object sender, EventArgs e)
        {
            if (lbClients.SelectedItem == null) { MessageBox.Show("请先从左上列表中选择要调用的客户端！"); return; }
            txtMsg.Clear();
            var cnt = Convert.ToInt32(numCount.Value);
            var sessionId = lbClients.SelectedItem.ToString().Split('|').Skip(1).First();
            var client = _server.GetSessionByID(sessionId);//.OnlineClients.Values.Where(s => string.Equals(s.SessionId, sessionId)).FirstOrDefault();
            if (client == null) { return; }
            var service = ProxyObjectFactory.GetInstance().CreateInterfaceProxyWithoutTarget<IConsumeDataService>(client.ClientID);//ClientSockProxy.CreateProxy<IConsumeDataService>(client.ClientId);
            decimal? a = null;
            var ticks = Convert.ToInt32(numTicks.Value);
            var getCount = Convert.ToInt32(numGet.Value);
            var list = GetTestData(cnt);
            var sw = new Stopwatch();
            for (int i = 0; i < ticks; i++)
            {
                sw.Restart();
                var data = service.GetItems(null, getCount, a, list);
                sw.Stop();
                if (data != null)
                {
                    DisplayMsg(string.Format("Server第{0}次调用，获取到：{1}条数据,耗时：{2}ms!", (i + 1).ToString(), data.Count, sw.ElapsedMilliseconds));
                }
                else
                {
                    DisplayMsg(string.Format("ERROR:Server第{0}次调用，获取失败!", (i + 1).ToString()));
                }
            }
        }

        private void DisplayClients(string client, bool add)
        {
            if (this.InvokeRequired) { this.Invoke(new Action<string, bool>(this.DisplayClients), new object[] { client, add }); }
            else
            {
                if (add)
                {

                    lbClients.Items.Add(client);
                }
                else
                {
                    lbClients.Items.Remove(client);
                }
            }
        }

        private void DisplayMsg(string msg)
        {
            if (msg.Length > 1000)
            {
                var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", DateTime.Now.ToString("yyyyMMddHHmmssms") + ".txt");
                System.IO.File.AppendAllText(file, msg);
                return;
            }
            if (this.InvokeRequired) { this.Invoke(new Action<string>(this.DisplayMsg), new object[] { msg }); }
            else
            {
                txtMsg.AppendText(string.Format("[{0}]-{1}{2}", DateTime.Now.ToLongTimeString(), msg, Environment.NewLine));
                txtMsg.ScrollToCaret();
            }
        }

        private void btnInvokeInOrder_Click(object sender, EventArgs e)
        {
            txtMsg.Clear();
            decimal? a = null;
            var cnt = Convert.ToInt32(numCount.Value);
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(lbClients.Items.OfType<string>(), (item) =>
                {
                    var sw = new Stopwatch();
                    var sessionId = item.ToString().Split('|').Skip(1).First();
                    var client = _server.GetSessionByID(sessionId);//.OnlineClients.Values.Where(s => string.Equals(s.SessionId, sessionId)).FirstOrDefault();
                    if (client != null)
                    {
                        var service = ProxyObjectFactory.GetInstance().CreateInterfaceProxyWithoutTarget<IConsumeDataService>(client.ClientID);//ClientSockProxy.CreateProxy<IConsumeDataService>(client.ClientId);
                        var ticks = Convert.ToInt32(numTicks.Value);
                        var getCount = Convert.ToInt32(numGet.Value);
                        var list = GetTestData(cnt);
                        for (int i = 0; i < ticks; i++)
                        {
                            sw.Restart();
                            var data = service.GetItems(null, getCount, a, list);
                            sw.Stop();
                            if (data != null)
                            {
                                DisplayMsg(string.Format("Server第{0}次调用{2}，获取到：{1}条数据,耗时：{3}ms!", (i + 1).ToString(), data.Count, client.ClientID, sw.ElapsedMilliseconds));
                            }
                            else
                            {
                                DisplayMsg(string.Format("ERROR:Server第{0}次调用{1}，获取失败,耗时：{2}ms!", (i + 1).ToString(), client.ClientID, sw.ElapsedMilliseconds));
                            }
                        }
                    }
                });
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (lbClients.SelectedItem == null) { return; }
            txtMsg.Clear();
            var cnt = Convert.ToInt32(numCount.Value);
            var sessionId = lbClients.SelectedItem.ToString().Split('|').Skip(1).First();
            var client = _server.GetSessionByID(sessionId);//.OnlineClients.Values.Where(s => string.Equals(s.SessionId, sessionId)).FirstOrDefault();
            if (client == null) { return; }
            var service = ProxyObjectFactory.GetInstance().CreateInterfaceProxyWithoutTarget<IConsumeDataService>(client.ClientID);
            var data = service.GetItems("aaa", 1, 2.5M, GetTestData(cnt));
            if (data != null)
            {
                DisplayMsg(string.Format("Server第{0}次调用{2}，获取到：{1}条数据!", 1.ToString(), data.Count, client.ClientID));
            }
        }
    }
}
