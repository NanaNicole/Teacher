using Colegio.Models;
using Colegio.Services.Interfaces;
using Confluent.Kafka;
using Newtonsoft.Json;
using Student.Producer;

namespace Colegio.Services
{
    public class Teacher : ITeacher
    {

        private readonly ILogger<Teacher> _logger;
        private readonly IColegioContext _dbContext;
        private readonly IProducer _producer;

        public Teacher(ILogger<Teacher> logger, IColegioContext dbContext, IProducer producer)
        {
            _logger = logger;
            _dbContext = dbContext;
            _producer = producer;
        }

        public List<TeacherDto> GetTeachers()
        {
            List<TeacherDto> result = null;
            try
            {
                result = _dbContext.Teachers.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"error obteniendo data de profesor {ex.Message}");
            }
            return result;
        }

        public TeacherDto GetTeacherByIdeentification(int identification)
        {
            TeacherDto result = null;
            try
            {
                result = _dbContext.Teachers.Where(x => x.Identification == identification)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError($"error obteniendo data de profesor {ex.Message.ToString()}");
            }
            return result;
        }

        public bool CreateTeacher(TeacherDto estudent)
        {
            bool result = false;
            try
            {
                estudent.Id = Guid.NewGuid();
                _dbContext.Teachers.Add(estudent);
                _dbContext.SaveChanges();
                result = true;
                _producer.ProduceMessage(JsonConvert.SerializeObject(estudent));
            }
            catch (Exception ex)
            {
                _logger.LogError($"error creando data de profesor {ex.Message.ToString()}");
            }
            return result;
        }

        public bool DeleteTeacher(Guid id)
        {
            bool result = false;
            try
            {
                TeacherDto estudent = FindTeacher(id);
                if (estudent != null)
                {
                    _dbContext.Teachers.Remove(estudent);
                    _dbContext.SaveChanges();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"error obteniendo data de profesor {ex.Message.ToString()}");
            }
            return result;
        }

        public TeacherDto FindTeacher(Guid id)
        {
            return _dbContext.Teachers.Find(id);
        }
    }
}
