using Popug.Tasks.Management.Services;
using System.Web.Http;

namespace Popug.Tasks.Management.Controllers
{
    [Route("api/tasks")]
    public class TasksController : ApiController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ITasksService _service;
        private readonly ILogger<TasksController> _logger;

        public TasksController(IHttpContextAccessor contextAccessor, ITasksService service, ILogger<TasksController> logger)
        {
            _contextAccessor = contextAccessor;
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IHttpActionResult> Mine(CancellationToken cancellationToken)
        {
            var popug = CurrentPopugId;
            if (popug == null)
            {
                return Unauthorized();
            }
            _logger.LogInformation($"Returning list of tasks for popug {popug}");
            var result = await _service.Mine(popug, cancellationToken);
            return result.Consume<IHttpActionResult>(t => Ok(t), err => InternalServerError());
        }

        [HttpPost]
        public async Task<IHttpActionResult> Close(string taskId, CancellationToken cancellationToken)
        {
            var popug = CurrentPopugId;
            if (popug == null)
            {

                return Unauthorized();
            }
            _logger.LogInformation($"Closing task {taskId} for popug {popug}");
            var result = await _service.Close(popug, taskId, cancellationToken);
            return result.Consume<IHttpActionResult>(t => Ok(t), err => InternalServerError());
        }

        [HttpPost]
        public async Task<IHttpActionResult> Create(string description, CancellationToken cancellationToken)
        {
            var popug = CurrentPopugId;
            if (popug == null)
            {
                return Unauthorized();
            }
            _logger.LogInformation($"Creating task for popug {popug}");
            var result = await _service.Create(popug, description, cancellationToken);
            return result.Consume<IHttpActionResult>(t => Ok(t), err => InternalServerError());
        }

        [HttpPost]
        public async Task<IHttpActionResult> Reassign(CancellationToken cancellationToken)
        {
            var popug = CurrentPopugId;
            if (popug == null)
            {
               
                return Unauthorized();
            }
            _logger.LogInformation($"Reassigning tasks by request from {popug}");
            var result = await _service.Reassign(popug, cancellationToken);
            return result.Consume<IHttpActionResult>(t => Ok(t), err => InternalServerError());
        }

        private string? CurrentPopugId
        {
            get
            {
                var popug = _contextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                _logger.LogWarning($"Recieved tasks api request from {popug ?? "unauthorized"} popug");
                return popug;
            }
        }
    }
}
