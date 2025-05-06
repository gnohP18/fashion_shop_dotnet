using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.Interfaces.Services;

public interface INotificationService
{
    Task Test(string fcmToken);
}