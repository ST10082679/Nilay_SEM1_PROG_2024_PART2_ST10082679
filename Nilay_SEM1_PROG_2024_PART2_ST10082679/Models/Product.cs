using System;
using System.Collections.Generic;

namespace Nilay_SEM1_PROG_2024_PART2_ST10082679.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public DateTime ProductDate { get; set; }

    public string Category { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
//---------------------------------End of FIle-----------------------------------------------------//