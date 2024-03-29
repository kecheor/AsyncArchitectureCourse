﻿using Popug.Common.Enums;

namespace Popug.Tasks.Repository.Models;
public record Performer(int? Id, string ChipId, string Name, Roles Role, DateTime Created);
