using Popug.Common;
using Popug.Messages.Contracts.Values;

namespace Popug.Accounts;
public record Account(int? Id, string ChipId, string Name, Roles Role, int BeakCurvature) : IEventValue
{
    public int Version => 1;
}