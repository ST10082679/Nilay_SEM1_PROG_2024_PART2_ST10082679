using System;
using System.Collections.Generic;

namespace Nilay_SEM1_PROG_2024_PART2_ST10082679.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Salt { get; set; } = null!;

    public string Role { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
//---------------------------------End of FIle-----------------------------------------------------//