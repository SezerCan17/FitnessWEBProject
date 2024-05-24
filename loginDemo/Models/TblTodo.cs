using System;
using System.Collections.Generic;

namespace loginDemo.Models;

public partial class TblTodo
{
    public int Id { get; set; }

    public string? Period { get; set; }

    public string? Difficulty { get; set; }

    public string? Category { get; set; }

    public string? Instruction { get; set; }

    public bool? IsDeleted { get; set; }

    public string? UserId { get; set; }
}
