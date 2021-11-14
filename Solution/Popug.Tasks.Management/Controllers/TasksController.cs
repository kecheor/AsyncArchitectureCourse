using Popug.Tasks.Management.Services;
using System.Web.Http;

namespace Popug.Tasks.Management.Controllers
{
    [Route("api/tasks")]
    public class TasksController : ApiController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ITasksService _service;

        public TasksController(IHttpContextAccessor contextAccessor, ITasksService service)
        {
            _contextAccessor = contextAccessor;
            _service = service;
        }

        [HttpGet]
        public async Task<IHttpActionResult> Mine(CancellationToken cancellationToken)
        {
            var popug = CurrentPopugId;
            if (popug == null)
            {
                return Unauthorized();
            }
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
            var result = await _service.Reassign(popug, cancellationToken);
            return result.Consume<IHttpActionResult>(t => Ok(t), err => InternalServerError());
        }

        private string? CurrentPopugId => _contextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
    }
}
