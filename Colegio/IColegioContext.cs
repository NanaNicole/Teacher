using Colegio.Models;
using Microsoft.EntityFrameworkCore;

namespace Colegio
{
    public interface IColegioContext
    {
        DbSet<TeacherDto> Teachers { get; set; }

        void SaveChanges();
    }
}