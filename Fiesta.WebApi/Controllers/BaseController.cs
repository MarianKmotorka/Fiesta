﻿using Microsoft.AspNetCore.Mvc;

namespace Fiesta.WebApi.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        //private IMediator _mediator;
        //private ICurrentUserService _currentUserService;

        //protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
        //protected ICurrentUserService CurrentUserService => _currentUserService ??= HttpContext.RequestServices.GetService<ICurrentUserService>();
    }
}
