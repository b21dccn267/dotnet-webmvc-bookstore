using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace newltweb.Models;

public partial class TUser : IdentityUser
{
    public string? Password { get; set; }

    public string? Role { get; set; }

    public string? Username { get; set; }

    public string? IdKhachHang { get; set; }

    public string? IdNhanVien { get; set; }

    public virtual TKhachHang? IdKhachHangNavigation { get; set; }

    public virtual TNhanVien? IdNhanVienNavigation { get; set; }
}
