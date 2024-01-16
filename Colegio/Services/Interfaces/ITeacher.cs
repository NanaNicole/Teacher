using Colegio.Models;

namespace Colegio.Services.Interfaces
{
    public interface ITeacher
    {
        public List<TeacherDto> GetTeachers();

        public TeacherDto GetTeacherByIdeentification(int identification);

        public bool CreateTeacher(TeacherDto estudent);
        public bool DeleteTeacher(Guid id);

        public TeacherDto FindTeacher(Guid id);
    }
}
