using fashion_shop.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using fashion_shop.Core.Interfaces.Services;


namespace fashion_shop.API.Controllers.Webhook;

public class NotificationController : APIController<NotificationController>
{
    private readonly INotificationService _notificationService;

    public NotificationController(
        ILogger<NotificationController> logger,
        IHttpContextAccessor httpContextAccessor,
        UserManager<User> userManager,
        INotificationService notificationService) : base(logger, httpContextAccessor, userManager)
    {
        _notificationService = notificationService;
    }

    [HttpPost("test-push-noti")]
    public async Task<string> PostAsync(string fcmToken)
    {
        await _notificationService.Test(fcmToken);

        return "Sent";
    }
}