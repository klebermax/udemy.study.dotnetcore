using System;
using System.Threading.Tasks;
using Actio.Common.Commands;
using Actio.Common.Events;
using Actio.Common.Exceptions;
using Actio.Services.Activities.Services;
using Microsoft.Extensions.Logging;
using RawRabbit;

namespace Actio.Services.Activities.Handlers
{
    public class CreateActivityHandler : ICommandHandler<CreateActivity>
    {
        private IBusClient _busClient;
        private readonly IActivityService _activityService;

        private ILogger _logger;

        public CreateActivityHandler(IBusClient busClient, IActivityService activityService)
        {
            //_logger = logger;
            this._activityService = activityService;
            _busClient = busClient;
        }
        public async Task HandleAsync(CreateActivity command)
        {
            Console.WriteLine($"{DateTime.Now} - Creating activity: {command.Name}");

            try
            {
                await _activityService.AddAsync(command.Id, command.UserId, command.Category, command.Name, command.Description, command.CreatedAt);

                await _busClient.PublishAsync(new ActivityCreated(command.Id, command.UserId, command.Category, command.Name, command.Description, DateTime.Now));
            }
            catch (ActioException ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);

                await _busClient.PublishAsync(new CreateActivityRejected(command.Id, ex.Code, ex.Message));                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);

                await _busClient.PublishAsync(new CreateActivityRejected(command.Id, "error", ex.Message));                
            }
        }
    }
}