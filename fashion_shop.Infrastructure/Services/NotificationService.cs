using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.API.ExternalService.Entities;
using fashion_shop.Core.DTOs.Common;
using fashion_shop.Core.Interfaces.Services;
using Firebase.Database;
using Firebase.Database.Query;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Options;

namespace fashion_shop.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly FirebaseClient _firebaseClient;
    private readonly FirebaseMessaging _messaging;

    public NotificationService(FirebaseMessaging messaging, IOptions<FirebaseSettings> options)
    {
        _messaging = messaging;
        _firebaseClient = new FirebaseClient(options.Value.DefaultConnection);
    }

    public async Task Test(string fcmToken)
    {
        var message = new Message()
        {
            Token = fcmToken,
            Notification = new Notification
            {
                Title = $"Phong-{DateTime.Now.ToString("yyyymmdd")}",
                Body = $"Nhi-{DateTime.Now.ToString("yyyymmdd")}"
            }
        };
        await _firebaseClient
           .Child("notifications")
           .Child("1")
           .PostAsync(new AdminMessage
           {
               SenderId = 1,
               Title = message.Notification.Title,
               Body = message.Notification.Body,
               CreatedAt = DateTime.UtcNow
           });

        var response = await _messaging.SendAsync(message);
    }
}