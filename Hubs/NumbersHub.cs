using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Doctors
{

    public class ChatHub : Hub
    {


        #region constructor
        private readonly IConfiguration _configuration;
        private readonly IGenericRepository<ChatUser> _repository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private ChatUser _me;

        public ChatHub(IConfiguration configuration,
                UserManager<IdentityUser> userManager,
                IGenericRepository<ChatUser> repository)
        {
            _configuration = configuration;
            _userManager = userManager;
            _repository = repository;
            //var ise = ControllerBase.HttpContext.Items["userId"];
            //string exp = HttpContext.Items["userexp"].ToString();

            //var ssss = await _userManager.FindByIdAsync(ise.ToString());
        }
        #endregion


        public ConcurrentDictionary<string, List<string>> ConnectedUsers = new ConcurrentDictionary<string, List<string>>();

        private async Task ME(string Id)
        {
            //HubConnectionContext.
            //_me = await _repository.GetEntityById(Id);
        }
        public async Task AddUserToMyAcceptList(string myEmail)
        {
            //ME
            //ChatUser newUser = new ChatUser(myEmail);

            //return newUser
        }


        public async Task SendMessage(string user, string message)
        {

            //public static ConcurrentDictionary<string, List<string>> ConnectedUsers = new ConcurrentDictionary<string, List<string>>();

            //await Clients.Client("").SendAsync("ReceiveMessage", user, message);
            await Clients.All.SendAsync("MessageReceived", JsonConvert.SerializeObject(ConnectedUsers));
        }

        [Authorize]
        public async Task NewMessage( string msg)
        {
            await Clients.All.SendAsync("MessageReceived", msg);
        }

        public async Task GetNumber(string NewNumber)
        {
            #region File

            const string path = "C:\\TestFiles\\Num.txt";
            System.IO.StreamReader SR = new System.IO.StreamReader(path);
            string value = SR.ReadToEnd();
            SR.Close();
            SR.Dispose();
            int num = int.Parse(value);
            num++;
            System.IO.StreamWriter SW = new System.IO.StreamWriter(path);
            await SW.WriteAsync(char.Parse(num.ToString()));
            SW.Flush();
            SW.Close();
            SW.Dispose();

            #endregion


            //return num;
            await Clients.All.SendAsync(num.ToString());
        }
    }
}
