using Exemplo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exemplo.Services
{
    public class NotificationService : INotificationService
    {
        public void NotifyUser(MessageModel messageModel)
        {
            //FAZ ALGUMA COISA
            Console.WriteLine(messageModel.Document);
            Console.WriteLine(messageModel.Id);
        }
    }
}
