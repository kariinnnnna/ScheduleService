using Moq;
using ScheduleServiceBusinessLogic.Implements;
using ScheduleServiceContracts.BindingModels;
using ScheduleServiceContracts.SearchModels;
using ScheduleServiceContracts.StorageContracts;
using ScheduleServiceContracts.ViewModels;
using ScheduleServiceDataModels.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceTest.Logic
{
    public class ScheduleItemLogicTests
    {
        [Fact]
        public void Create_ShouldThrow_WhenManualClassroomIsBusy_CaseInsensitive()
        {
            var scheduleStorageMock = new Mock<IScheduleItemStorage>();
            var groupStorageMock = new Mock<IGroupStorage>();
            var teacherStorageMock = new Mock<ITeacherStorage>();
            var lessonTimeStorageMock = new Mock<ILessonTimeStorage>();

            scheduleStorageMock
                .Setup(x => x.GetFullList())
                .Returns(new List<ScheduleItemViewModel>
                {
                    new ScheduleItemViewModel
                    {
                        Id = 11,
                        Date = new DateTime(2026, 4, 20),
                        Subject = "Математика",
                        ClassroomNumber = " 405 ",
                        StartTime = new TimeSpan(10, 0, 0),
                        EndTime = new TimeSpan(11, 30, 0)
                    }
                });

            var logic = new ScheduleItemLogic(
                scheduleStorageMock.Object,
                groupStorageMock.Object,
                teacherStorageMock.Object,
                lessonTimeStorageMock.Object);

            var model = new ScheduleItemBindingModel
            {
                Type = ScheduleItemType.Lecture,
                Date = new DateTime(2026, 4, 20),
                Subject = "Физика",
                StartTime = new TimeSpan(10, 30, 0),
                EndTime = new TimeSpan(12, 0, 0),
                ClassroomNumber = "405",
                GroupName = "ИВТ-301",
                TeacherName = "Иванов И.И."
            };

            var ex = Assert.Throws<InvalidOperationException>(() => logic.Create(model));

            Assert.Equal("Аудитория  405  уже занята на 20.04.2026 с 10:00 до 11:30.", ex.Message);
            scheduleStorageMock.Verify(x => x.Insert(It.IsAny<ScheduleItemBindingModel>()), Times.Never);
        }

        [Fact]
        public void Create_ShouldThrow_WhenLessonTimeIdDoesNotExist()
        {
            var scheduleStorageMock = new Mock<IScheduleItemStorage>();
            var groupStorageMock = new Mock<IGroupStorage>();
            var teacherStorageMock = new Mock<ITeacherStorage>();
            var lessonTimeStorageMock = new Mock<ILessonTimeStorage>();

            lessonTimeStorageMock
                .Setup(x => x.GetElement(It.Is<LessonTimeSearchModel>(m => m.Id == 99)))
                .Returns((LessonTimeViewModel?)null);

            var logic = new ScheduleItemLogic(
                scheduleStorageMock.Object,
                groupStorageMock.Object,
                teacherStorageMock.Object,
                lessonTimeStorageMock.Object);

            var model = new ScheduleItemBindingModel
            {
                Type = ScheduleItemType.Practice,
                Date = new DateTime(2026, 4, 21),
                Subject = "Программирование",
                LessonTimeId = 99,
                ClassroomNumber = "302",
                GroupName = "ПМИ-201",
                TeacherName = "Петров П.П."
            };

            var ex = Assert.Throws<InvalidOperationException>(() => logic.Create(model));

            Assert.Equal("Указанное время пары не найдено", ex.Message);
            scheduleStorageMock.Verify(x => x.Insert(It.IsAny<ScheduleItemBindingModel>()), Times.Never);
        }

        [Fact]
        public void Update_ShouldAllowSameTimeSlot_ForSameEntity()
        {
            var scheduleStorageMock = new Mock<IScheduleItemStorage>();
            var groupStorageMock = new Mock<IGroupStorage>();
            var teacherStorageMock = new Mock<ITeacherStorage>();
            var lessonTimeStorageMock = new Mock<ILessonTimeStorage>();

            var existing = new ScheduleItemViewModel
            {
                Id = 20,
                Type = ScheduleItemType.Laboratory,
                Date = new DateTime(2026, 4, 22),
                Subject = "Базы данных",
                ClassroomNumber = "501",
                StartTime = new TimeSpan(13, 0, 0),
                EndTime = new TimeSpan(14, 30, 0),
                GroupName = "ИВТ-401",
                TeacherName = "Сидоров С.С."
            };

            scheduleStorageMock
                .Setup(x => x.GetElement(It.Is<ScheduleItemSearchModel>(m => m.Id == 20)))
                .Returns(existing);

            scheduleStorageMock
                .Setup(x => x.GetFullList())
                .Returns(new List<ScheduleItemViewModel> { existing });

            var model = new ScheduleItemBindingModel
            {
                Id = 20,
                Type = ScheduleItemType.Laboratory,
                Date = new DateTime(2026, 4, 22),
                Subject = "Базы данных (обновлено)",
                StartTime = new TimeSpan(13, 0, 0),
                EndTime = new TimeSpan(14, 30, 0),
                ClassroomNumber = "501",
                GroupName = "ИВТ-401",
                TeacherName = "Сидоров С.С."
            };

            scheduleStorageMock
                .Setup(x => x.Update(model))
                .Returns(new ScheduleItemViewModel
                {
                    Id = 20,
                    Type = ScheduleItemType.Laboratory,
                    Date = new DateTime(2026, 4, 22),
                    Subject = "Базы данных (обновлено)",
                    ClassroomNumber = "501",
                    StartTime = new TimeSpan(13, 0, 0),
                    EndTime = new TimeSpan(14, 30, 0),
                    GroupName = "ИВТ-401",
                    TeacherName = "Сидоров С.С."
                });

            var logic = new ScheduleItemLogic(
                scheduleStorageMock.Object,
                groupStorageMock.Object,
                teacherStorageMock.Object,
                lessonTimeStorageMock.Object);

            var result = logic.Update(model);

            Assert.NotNull(result);
            Assert.Equal("Базы данных (обновлено)", result!.Subject);
            scheduleStorageMock.Verify(x => x.Update(model), Times.Once);
        }
    }
}
