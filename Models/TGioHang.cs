using System;
using System.Collections.Generic;

namespace newltweb.Models;

public partial class TGioHang
{
    public int MaGh { get; set; }

    public string UserId { get; set; } = null!;

    public DateTime Tgtao { get; set; }

    public DateTime TgcapNhat { get; set; }

    public virtual ICollection<TSachGh> TSachGhs { get; set; } = new List<TSachGh>();
}
