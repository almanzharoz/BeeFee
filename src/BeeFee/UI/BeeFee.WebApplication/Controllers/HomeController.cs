using System.Threading.Tasks;
using BeeFee.ClientApp.Services;
using BeeFee.Model.Exceptions;
using BeeFee.Model.Interfaces;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using BeeFee.Model.Services;
using BeeFee.WebApplication.Infrastructure.Services;
using BeeFee.WebApplication.Models;
using BeeFee.WebApplication.Models.Event;
using Core.ElasticSearch.Domain;
using Core.ElasticSearch.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SharpFuncExt;

namespace BeeFee.WebApplication.Controllers
{
    public class HomeController : BaseController<EventService>
    {
        private readonly EventUIService _eventUIService;
        public HomeController(EventService service, CategoryService categoryService, EventUIService eventUiService) : base(service, categoryService)
        {
            _eventUIService = eventUiService;
        }

        public async Task<IActionResult> Index(LoadEventsRequest request)
            => View(new EventFilterViewModel(request, Service.GetAllCities(), CategoryService.GetAllCategories<BaseCategoryProjection>(),
                await _eventUIService.GetEventsListRenderAsync(request)));

        public IActionResult Error()
        {
            //return View();
	        var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
	        if (exceptionFeature != null)
	        {
		        var routeWhereExceptionOccured = exceptionFeature.Path;
		        var exception = exceptionFeature.Error;

				switch (exception)
				{
					case VersionException _:
						return View(new ErrorViewModel("Этот документ был изменен другим пользователем"));
					case AddEntityException<IModel> _:
						return View(new ErrorViewModel("Произошла ошибка при добавлении"));
					case EntityAccessException<IEntity> _:
						return View(new ErrorViewModel("Ошибка доступа"));
					case EntityAlreadyExistsException _:
						return View(new ErrorViewModel("Объект уже существует"));
					case IExistsUrlException _: //Не работает с интерфейсами, видимо нужно обрабатывать каждую сущность отдельно, либо откзааться от дженерика
						return View(new ErrorViewModel("URL уже существует"));
					case RemoveEntityException _:
						return View(new ErrorViewModel("Ошибка при удалении элемента"));
					case UpdateEntityException _:
						return View(new ErrorViewModel("Невозможно обновить элемент"));
				}
	        }
		    return View(new ErrorViewModel("Произошла неизвестная ошибка"));
        }

        public IActionResult FileTest()
        {
            return View();
        }

    }
}
