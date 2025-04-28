using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.Common;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.DTOs.Responses.User;
using fashion_shop.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace fashion_shop.API.Controllers.Admin
{
    [ApiController]
    [Tags("Settings")]
    [Route("api/settings")]
    public class SettingController : APIController<SettingController>
    {
        private readonly ISettingService _settingService;

        public SettingController(
            ILogger<SettingController> logger,
            ISettingService settingService) : base(logger)
        {
            _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
        }

        [HttpPut("basic-info")]
        public async Task<IActionResult> UpdateBasicInfo([FromBody] UpdateBasicInfoSettingRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ErrorResponse<string>("Validation failed");
            }

            await _settingService.UpdateSettingAsync<UpdateBasicInfoSettingRequest>(SettingPrefixConstants.BasicInfoPrefix, request);

            return NoContentResponse<string>("Updated successfully");
        }

        [HttpGet("basic-info")]
        public async Task<IActionResult> GetBasicInfo()
        {
            var response = await _settingService.GetSettingAsync<BasicInfoSettingResponse>(SettingPrefixConstants.BasicInfoPrefix);

            return OkResponse(response, "OK");
        }

        [HttpPut("statistic")]
        public async Task<IActionResult> UpdateStatistic([FromBody] UpdateStatisticSettingRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ErrorResponse<string>("Validation failed");
            }

            await _settingService.UpdateSettingAsync<UpdateStatisticSettingRequest>(SettingPrefixConstants.StatisticPrefix, request);

            return NoContentResponse<string>("Updated successfully");
        }

        [HttpGet("statistic")]
        public async Task<ActionResult<string>> GetStatistic()
        {
            var response = await _settingService.GetSettingAsync<StatisticSettingResponse>(SettingPrefixConstants.StatisticPrefix);

            return OkResponse(response, "OK");
        }

        [HttpGet("sync-redis")]
        public async Task<ActionResult<string>> SyncRedis()
        {
            await _settingService.SyncRedisDataAsync();

            return NoContentResponse<string>("Synchorinzed data");
        }
    }
}