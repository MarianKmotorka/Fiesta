using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Domain.Entities.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fiesta.Application.Features.Events.EventHandlers
{
    public class OnEventDeletedDeleteCloudResourcesEventHandler : INotificationHandler<EventDeletedEvent>
    {
        private readonly IImageService _imageService;
        private readonly ILogger<OnEventDeletedDeleteCloudResourcesEventHandler> _logger;

        public OnEventDeletedDeleteCloudResourcesEventHandler(IImageService imageService, ILogger<OnEventDeletedDeleteCloudResourcesEventHandler> logger)
        {
            _imageService = imageService;
            _logger = logger;
        }

        public async Task Handle(EventDeletedEvent notification, CancellationToken cancellationToken)
        {
            var @event = notification.Event;
            var folder = CloudinaryPaths.EventFolder(@event.Id);

            var result = await _imageService.DeleteFolder(folder, cancellationToken);
            if (result.Failed)
                _logger.LogError($"Folder with path {folder} failed to be deleted. Reason: {string.Join(',', result.Errors)}");
        }
    }
}
