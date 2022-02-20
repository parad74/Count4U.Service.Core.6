using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Monitor.Service.Model
{
 //   public interface IChatClient
	//{
	//	Task ReceiveMessage(string user, string message);
	//	Task ReceiveNotify(string notify);
	//}
    public class SignalRtcHub : Hub
    {

        public async Task NewUser(string username)
        {
            var userInfo = new UserInfo() { userName = username, connectionId = Context.ConnectionId };
            await Clients.Others.SendAsync("NewUserArrived", JsonSerializer.Serialize(userInfo));
        }

        public async Task HelloUser(string userName, string user)
        {
            var userInfo = new UserInfo() { userName = userName, connectionId = Context.ConnectionId };
            await Clients.Client(user).SendAsync("UserSaidHello", JsonSerializer.Serialize(userInfo));
        }

        //����� � 3 �����, �� ����� ��������� ��� ������ ������������, ����� ������ p2p-�����������
        public async Task SendSignal(string signal, string user)
        {
            await Clients.Client(user).SendAsync("SendSignal", Context.ConnectionId, signal);
        }

        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            await Clients.All.SendAsync("UserDisconnect", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
