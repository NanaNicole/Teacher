using Colegio;
using Colegio.Models;
using Colegio.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Student.Producer;

public class TeacherTests
{
    [Fact]
    public void GetTeachers_ReturnsTeacherList_WhenDataExists()
    {
        // Arrange
        var data = new List<TeacherDto>
        {
            new TeacherDto {Id = Guid.NewGuid(),Name= "Prueba",Email="prueba@prueba.com" },
            new TeacherDto {Id = Guid.NewGuid(),Name= "Prueba2",Email="prueba2@prueba.com"}
        }.AsQueryable();

        Mock<DbSet<TeacherDto>> mockSet = MoqDbsetTeacher(data);

        var mockContext = new Mock<IColegioContext>();
        mockContext.Setup(c => c.Teachers).Returns(mockSet.Object);

        var mockLogger = new Mock<ILogger<Teacher>>();
        var mockProducer = new Mock<IProducer>();
        var service = new Teacher(mockLogger.Object, mockContext.Object, mockProducer.Object);

        // Act
        var result = service.GetTeachers();

        // Assert
        Assert.Equal(data.ToList(), result);
    }

    private static Mock<DbSet<TeacherDto>> MoqDbsetTeacher(IQueryable<TeacherDto> data)
    {
        var mockSet = new Mock<DbSet<TeacherDto>>();
        mockSet.As<IQueryable<TeacherDto>>().Setup(m => m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<TeacherDto>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<TeacherDto>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        mockSet.Setup(m => m.Add(It.IsAny<TeacherDto>())).Callback<TeacherDto>((s) => data.ToList().Add(s));
        mockSet.Setup(m => m.Remove(It.IsAny<TeacherDto>())).Callback<TeacherDto>((s) => data.ToList().Remove(s));
        mockSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids => data.FirstOrDefault(d => d.Id == (Guid)ids[0]));
        return mockSet;
    }

    [Fact]
    public void GetTeachers_ReturnsNull()
    {
        // Arrange
        var data = new List<TeacherDto>();

        Mock<DbSet<TeacherDto>> mockSet = MoqDbsetTeacher(data.AsQueryable());

        var mockContext = new Mock<IColegioContext>();
        mockContext.Setup(c => c.Teachers).Returns(mockSet.Object);

        var mockLogger = new Mock<ILogger<Teacher>>();
        var mockProducer = new Mock<IProducer>();
        var service = new Teacher(mockLogger.Object, mockContext.Object, mockProducer.Object);

        // Act
        var result = service.GetTeachers();

        // Assert
        Assert.Equal(data.ToList(), result);
    }

    [Fact]
    public void CreateTeacher_ReturnsTrue_WhenTeacherIsCreated()
    {
        // Arrange
        var teacher = new TeacherDto { Name = "Prueba", Email = "prueba@prueba.com" };

        var mockSet = new Mock<DbSet<TeacherDto>>();
        mockSet.Setup(m => m.Add(It.IsAny<TeacherDto>()));

        var mockContext = new Mock<IColegioContext>();
        mockContext.Setup(c => c.Teachers).Returns(mockSet.Object);

        var mockLogger = new Mock<ILogger<Teacher>>();
        var mockProducer = new Mock<IProducer>();
        mockProducer.Setup(p => p.ProduceMessage(It.IsAny<string>()));
        var service = new Teacher(mockLogger.Object, mockContext.Object, mockProducer.Object);

        // Act
        var result = service.CreateTeacher(teacher);

        // Assert
        Assert.True(result);
        mockSet.Verify(m => m.Add(It.IsAny<TeacherDto>()), Times.Once());
        mockContext.Verify(m => m.SaveChanges(), Times.Once());
    }

    [Fact]
    public void DeleteTeacher_ReturnsTrue_WhenTeacherExists()
    {
        // Arrange
        var teacherId = Guid.NewGuid();
        var teacher = new TeacherDto { Id = teacherId, Name = "Test", Email = "test@test.com" };

        var data = new List<TeacherDto> { teacher }.AsQueryable();
        var mockSet = MoqDbsetTeacher(data);

        var mockContext = new Mock<IColegioContext>();
        mockContext.Setup(c => c.Teachers).Returns(mockSet.Object);

        var mockLogger = new Mock<ILogger<Teacher>>();
        var mockProducer = new Mock<IProducer>();
        var service = new Teacher(mockLogger.Object, mockContext.Object, mockProducer.Object);

        // Act
        var result = service.DeleteTeacher(teacherId);

        // Assert
        Assert.True(result);
        mockSet.Verify(m => m.Remove(It.IsAny<TeacherDto>()), Times.Once());
        mockContext.Verify(m => m.SaveChanges(), Times.Once());
    }

    [Fact]
    public void DeleteTeacher_ReturnsFalse_WhenTeacherDoesNotExist()
    {
        // Arrange
        var teacherId = Guid.NewGuid();

        var data = new List<TeacherDto>().AsQueryable();
        var mockSet = MoqDbsetTeacher(data);

        var mockContext = new Mock<IColegioContext>();
        mockContext.Setup(c => c.Teachers).Returns(mockSet.Object);

        var mockLogger = new Mock<ILogger<Teacher>>();
        var mockProducer = new Mock<IProducer>();
        var service = new Teacher(mockLogger.Object, mockContext.Object, mockProducer.Object);

        // Act
        var result = service.DeleteTeacher(teacherId);

        // Assert
        Assert.False(result);
        mockSet.Verify(m => m.Remove(It.IsAny<TeacherDto>()), Times.Never());
        mockContext.Verify(m => m.SaveChanges(), Times.Never());
    }

    [Fact]
    public void DeleteTeacher_LogsError_WhenExceptionIsThrown()
    {
        // Arrange
        var teacherId = Guid.NewGuid();

        var mockSet = new Mock<DbSet<TeacherDto>>();
        mockSet.Setup(m => m.Find(teacherId)).Throws(new Exception("Test exception"));

        var mockContext = new Mock<IColegioContext>();
        mockContext.Setup(c => c.Teachers).Returns(mockSet.Object);

        var mockLogger = new Mock<ILogger<Teacher>>();
        var mockProducer = new Mock<IProducer>();
        var service = new Teacher(mockLogger.Object, mockContext.Object, mockProducer.Object);

        // Act
        var result = service.DeleteTeacher(teacherId);

        // Assert
        Assert.False(result);
        mockLogger.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
    }

    [Fact]
    public void GetTeacherByIdeentification_ReturnsTeacher_WhenTeacherExists()
    {
        // Arrange
        var teacherId = 123;
        var teacher = new TeacherDto { Id = Guid.NewGuid(), Name = "Prueba", Email = "prueba@prueba.com", Identification = teacherId };

        var data = new List<TeacherDto> { teacher }.AsQueryable();
        var mockSet = MoqDbsetTeacher(data);

        var mockContext = new Mock<IColegioContext>();
        mockContext.Setup(c => c.Teachers).Returns(mockSet.Object);

        var mockLogger = new Mock<ILogger<Teacher>>();
        var mockProducer = new Mock<IProducer>();
        var service = new Teacher(mockLogger.Object, mockContext.Object, mockProducer.Object);

        // Act
        var result = service.GetTeacherByIdeentification(teacherId);

        // Assert
        Assert.Equal(teacher, result);
    }


}