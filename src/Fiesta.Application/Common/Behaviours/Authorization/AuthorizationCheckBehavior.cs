﻿using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Exceptions;
using Fiesta.Application.Common.Interfaces;
using MediatR;

namespace Fiesta.Application.Common.Behaviours.Authorization
{
    public class AuthorizationCheckBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IFiestaDbContext _db;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthorizationCheck<TRequest> _authorizationCheck;

        public AuthorizationCheckBehavior(IFiestaDbContext db, ICurrentUserService currentUserService, IAuthorizationCheck<TRequest> authorizationCheck = null)
        {
            _db = db;
            _authorizationCheck = authorizationCheck;
            _currentUserService = currentUserService;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (_authorizationCheck is null || await _authorizationCheck.IsAuthorized(request, _db, _currentUserService, cancellationToken))
                return await next();

            // if authorization fails because of expired bearer, 401 should be returned
            // e.g. EventDetail is public endpoint only with AuthorizationCheck for certain event types (private, friendsOnly)
            if (string.IsNullOrEmpty(_currentUserService.UserId))
                throw new Unauthorized401Exception();

            throw new Forbidden403Exception();
        }
    }
}
