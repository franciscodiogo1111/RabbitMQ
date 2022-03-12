using Exemplo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exemplo.Services
{
    public interface INotificationService
    {
        void NotifyUser(MessageModel model);
    }
}
