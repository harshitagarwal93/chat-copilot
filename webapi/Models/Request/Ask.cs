﻿// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ChatCopilot.WebApi.Options;

namespace ChatCopilot.WebApi.Models.Request;

public class Ask
{
    [Required, NotEmptyOrWhitespace]
    public string Input { get; set; } = string.Empty;

    public IEnumerable<KeyValuePair<string, string>> Variables { get; set; } = Enumerable.Empty<KeyValuePair<string, string>>();
}
