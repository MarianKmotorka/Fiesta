using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Domain.Entities.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fiesta.Application.Features.Users.EventHandlers
{
    public class OnFiestaUserDeletedDeleteCloudResourcesEventHandler : INotificationHandler<FiestaUserDeletedEvent>
    {
        private readonly IImageService _imageService;
        private readonly ILogger<OnFiestaUserDeletedDeleteCloudResourcesEventHandler> _logger;

        public OnFiestaUserDeletedDeleteCloudResourcesEventHandler(IImageService imageService, ILogger<OnFiestaUserDeletedDeleteCloudResourcesEventHandler> logger)
        {
            _imageService = imageService;
            _logger = logger;
        }

        public async Task Handle(FiestaUserDeletedEvent notification, CancellationToken cancellationToken)
        {
            var user = notification.User;
            if (string.IsNullOrEmpty(user.PictureUrl) || !user.PictureUrl.Contains(_imageService.Domain))
                return;

            var path = CloudinaryPaths.ProfilePicture(user.Id);

            var result = await _imageService.Delete(path, cancellationToken);
            if (result.Failed)
                _logger.LogError($"Image with path {path} failed to be deleted. Reason: {string.Join(',', result.Errors)}");
        }
    }
}
