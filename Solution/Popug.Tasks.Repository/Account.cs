using Popug.Common;

namespace Popug.Tasks.Repository
{
    public record Account(int? Id, string ChipId, string Name, Roles Role)
    {
    }
}