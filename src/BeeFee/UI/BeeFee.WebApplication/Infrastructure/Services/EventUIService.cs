using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeeFee.ClientApp.Services;
using BeeFee.WebApplication.Models.Event;

namespace BeeFee.WebApplication.Infrastructure.Services
{
    public class EventUIService
    {
        private readonly ViewRenderService _viewRenderService;
        private readonly EventService _eventService;

        public EventUIService(ViewRenderService viewRenderService, EventService eventService)
        {
            _viewRenderService = viewRenderService;
            _eventService = eventService;
        }

        public async Task<(bool AllLoaded, string Html)> GetEventsListRenderAsync(LoadEventsRequest request)
        {
            var events = await _eventService.SearchEvents(request.Text, request.City, request.Categories,
                request.StartDate,
                request.EndDate, request.MaxPrice, request.PageSize ?? 9, request.PageIndex);
            return (AllLoaded: events.Count < events.Limit, Html: await _viewRenderService.RenderToStringAsync("Home/_EventGrid", events));
        }
    }
}
