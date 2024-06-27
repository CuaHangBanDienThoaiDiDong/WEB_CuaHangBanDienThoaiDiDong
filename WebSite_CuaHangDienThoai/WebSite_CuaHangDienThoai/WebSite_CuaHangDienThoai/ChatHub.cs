using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite_CuaHangDienThoai
{
    public class ChatHub : Hub
    {
        public void SendMessage(int customerId, string message)
        {
            // Gửi tin nhắn tới client với các tham số được truyền vào
            Clients.All.broadcastMessage(customerId, message);
        }

        public override System.Threading.Tasks.Task OnConnected()
        {
            // Xử lý khi một client kết nối đến hub
            return base.OnConnected();
        }

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            // Xử lý khi một client ngắt kết nối từ hub
            return base.OnDisconnected(stopCalled);
        }
    }
}



