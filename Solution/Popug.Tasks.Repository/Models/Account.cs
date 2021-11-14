using Popug.Common;
using Popug.Messages.Contracts.Values;
namespace Popug.Tasks.Repository.Models;
public record Performer(int? Id, string ChipId, string Name, Roles Role, DateTime Created) : IEventValue
{
    public int Version => 1;
}
